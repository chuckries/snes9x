#include "pch.h"
#include "Engine.h"
#include "Snes9xWrapper.h"
#include "LibSnes9x.h"
#include "Settings.h"
#include <experimental\filesystem>
#include <robuffer.h>

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Storage;
using namespace Microsoft::WRL;

namespace Snes9x { namespace Core
{
    Engine^ Engine::g_Emulator = ref new Engine();

    Engine::Engine()
        : _currentRom(nullptr)
    {
        int size = MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 2;
        Buffer^ buffer = ref new Buffer(size);
        buffer->Length = size;
        _snesScreen = ref new Surface(
            MAX_SNES_WIDTH,
            MAX_SNES_HEIGHT,
            MAX_SNES_WIDTH * 2,
            buffer
        );
        _renderedScreen = ref new Surface(
            MAX_SNES_WIDTH,
            MAX_SNES_HEIGHT,
            MAX_SNES_HEIGHT * 4,
            ref new Buffer(MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 4)
        );
    }

    void Engine::Init(StorageFolder^ savesFolder)
    {
        memset(&Settings, 0, sizeof(Settings));
        Settings.MouseMaster = false;
        Settings.SuperScopeMaster = false;
        Settings.JustifierMaster = false;
        Settings.MultiPlayer5Master = false;
        Settings.FrameTimePAL = 20000;
        Settings.FrameTimeNTSC = 16667;
        Settings.SixteenBitSound = true;
        Settings.Stereo = true;
        Settings.SoundPlaybackRate = 32000;
        Settings.SoundInputRate = 32000;
        Settings.SupportHiRes = true;
        Settings.Transparency = true;
        Settings.AutoDisplayMessages = true;
        Settings.InitialInfoStringTimeout = 120;
        Settings.HDMATimingHack = 100;
        Settings.BlockInvalidVRAMAccessMaster = true;
        Settings.DisplayFrameRate = false;
        Settings.AutoSaveDelay = 30;
        Settings.UpAndDown = false;

        _savesFolder = savesFolder;

        ThrowIfFalse(Memory.Init());
        ThrowIfFalse(S9xInitAPU());

        GFX.Screen = (uint16_t*)GetBufferByteAccess(_snesScreen->Bytes);
        GFX.Pitch = _snesScreen->Pitch;
        ThrowIfFalse(S9xGraphicsInit());

        ThrowIfFalse(S9xInitSound(128, 0));

        S9xSetController(0, CTL_JOYPAD, 0, 0, 0, 0);
        S9xSetController(1, CTL_JOYPAD, 1, 0, 0, 0);
        S9xUnmapAllControls();
        S9xSetupDefaultKeymap();

        // redirect stdout, stderr
        Platform::String^ stdoutPath = ApplicationData::Current->LocalCacheFolder->Path + L"//stdout.txt";
        Platform::String^ stderrPath = ApplicationData::Current->LocalCacheFolder->Path + L"//stderr.txt";
        FILE* stdoutFile = nullptr;
        FILE* stderrFile = nullptr;
        _wfreopen_s(&stdoutFile, stdoutPath->Data(), L"w", stdout);
        _wfreopen_s(&stderrFile, stderrPath->Data(), L"w", stderr);
    }

    IAsyncAction^ Engine::LoadRomAsync(StorageFile^ romFile)
    {
        return create_async([=]()
        {
            Lock lock(_engineMutex);

            if (S9xWrapper::LoadRom(CW2A(romFile->Path->Data())))
            {
                _currentRom = romFile;
            }
            else
            {
                throw ref new FailureException("Failed to load ROM");
            }
        });
    }

    Surface^ Engine::Update()
    {
        Lock lock(_engineMutex);
        S9xWrapper::MainLoop();

        return _renderedScreen;
    }

    bool Engine::SaveState(Platform::String^ path)
    {
        Lock lock(_engineMutex);
        return S9xWrapper::SaveState(CW2A(path->Data()));
    }

    bool Engine::LoadState(Platform::String^ path)
    {
        Lock lock(_engineMutex);
        return S9xWrapper::LoadState(CW2A(path->Data()));
    }

    IAsyncOperation<IBuffer^>^ Engine::SaveSramAsync()
    {
        return create_async([this]()
        {
            Lock lock(_engineMutex);

            if (!HasSram())
            {
                return task_from_result<IBuffer^>(nullptr);
            }

            int size = GetSramByteCount();

            if (!size)
            {
                return task_from_result<IBuffer^>(nullptr);
            }

            IBuffer^ sramBuffer = CryptographicBuffer::CreateFromByteArray(ArrayReference<byte>(Memory.SRAM, size));

            return create_task(_savesFolder->CreateFileAsync(Platform::String::Concat(_currentRom->DisplayName, ".srm"), CreationCollisionOption::OpenIfExists))
            .then([this, sramBuffer](StorageFile^ file)
            {
                return FileIO::WriteBufferAsync(file, sramBuffer);
            }, task_continuation_context::use_arbitrary())
            .then([this, sramBuffer]()
            {
                return sramBuffer;
            }, task_continuation_context::use_arbitrary());
        });
    }

    IAsyncAction^ Engine::LoadSramAsync()
    {
        return create_async([this]()
        {
             return create_task(_savesFolder->GetFileAsync(Platform::String::Concat(_currentRom->DisplayName, ".srm")))
             .then([this](StorageFile^ file)
             {
                 return FileIO::ReadBufferAsync(file);
             }, task_continuation_context::use_arbitrary())
             .then([this](IBuffer^ sramBuffer)
             {
                 Lock lock(_engineMutex);

                 int size = GetSramByteCount();

                 Memory.ClearSRAM();
                 byte* sramBytes = GetBufferByteAccess(sramBuffer);

                 memcpy(Memory.SRAM, sramBytes, sramBuffer->Length);

                 if (sramBuffer->Length - size == 512)
                 {
                     memmove(Memory.SRAM, Memory.SRAM + 512, size);
                 }
             }, task_continuation_context::use_arbitrary())
             .then([this](task<void> t)
             {
                 try
                 {
                     t.get();
                 }
                 catch (COMException^ e)
                 {
                     if (e->HResult == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND))
                     {
                         // do nothing
                     }
                 }
             });
        });
    }

    void Engine::SetResolution(int width, int height)
    {
        _snesScreen->Width = width;
        _snesScreen->Height = height;

        _renderedScreen->Width = width;
        _renderedScreen->Height = height;
        _renderedScreen->Pitch = width * 4;
        _renderedScreen->Bytes->Length = width * height * 4;
        ConvertDepth16to32(_snesScreen, _renderedScreen);
    }

    void Engine::OnSramChanged()
    {
        SramChanged(this);
    }

    void Engine::ConvertDepth16to32(Surface^ source, Surface^ destination)
    {
        uint16_t* src = (uint16_t*)GetBufferByteAccess(source->Bytes);
        uint32_t* dst = (uint32_t*)GetBufferByteAccess(destination->Bytes);

        for (int line = 0; line < source->Height; line++)
        {
            for (int x = source->Width - 1; x >= 0; x--)
            {
                uint16_t pixel = *(src + x);

                uint32_t r = (pixel & 0xF800) >> 11;
                uint32_t g = (pixel & 0x07E0) >> 5;
                uint32_t b = (pixel & 0x001F);

                r = r * 255 / 31;
                g = g * 255 / 63;
                b = b * 255 / 31;

                *(dst + x) = 0xFF000000 | (r << 16) | (g << 8) | b;

            }

            src = (uint16_t*)(((uint8_t*)src) + source->Pitch);
            dst = (uint32_t*)(((uint8_t*)dst) + destination->Pitch);
        }
    }

    Platform::String^ Engine::GetSavePath()
    {
        if (_currentRom == nullptr) return nullptr;
        std::experimental::filesystem::path path(_savesFolder->Path->Data());
        path = path.append(_currentRom->DisplayName->Data()).replace_extension(".srm");
        return ref new Platform::String(path.c_str());
    }

    byte* Engine::GetBufferByteAccess(IBuffer^ buffer)
    {
        ComPtr<IUnknown> pUnk = reinterpret_cast<IUnknown*>(buffer);
        ComPtr<IBufferByteAccess> pByteAccess;
        pUnk.As(&pByteAccess);
        byte* pBytes = nullptr;
        pByteAccess->Buffer(&pBytes);
        return pBytes;
    }
} }