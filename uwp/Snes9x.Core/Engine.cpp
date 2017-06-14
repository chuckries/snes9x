#include "pch.h"
#include "Engine.h"
#include "Snes9xWrapper.h"
#include "Settings.h"
#include <experimental\filesystem>
#include <robuffer.h>

using namespace Platform;
using namespace Windows::Storage;
using namespace Microsoft::WRL;

#define SNES_WIDTH                  256
#define SNES_HEIGHT                 224
#define SNES_HEIGHT_EXTENDED        239
#define MAX_SNES_WIDTH              (SNES_WIDTH * 2)
#define MAX_SNES_HEIGHT             (SNES_HEIGHT_EXTENDED * 2)

namespace Snes9x { namespace Core
{
    Engine^ Engine::g_Emulator = ref new Engine();

    Engine::Engine()
        : _settings(ref new CoreSettings())
        , _currentRom(nullptr)
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

    bool Engine::Init(StorageFolder^ savesFolder)
    {
        bool bOk = true;

        _savesFolder = savesFolder;

        S9xWrapper::InitMemory();
        bOk = bOk && S9xWrapper::InitApu();


        bOk = bOk && S9xWrapper::InitGraphics((uint16_t*)GetBufferByteAccess(_snesScreen->Bytes), _snesScreen->Pitch);

        bOk = bOk && S9xWrapper::InitSound(128, 0);

        S9xWrapper::InitControllers();

        // redirect stdout, stderr
        String^ stdoutPath = ApplicationData::Current->LocalCacheFolder->Path + L"//stdout.txt";
        String^ stderrPath = ApplicationData::Current->LocalCacheFolder->Path + L"//stderr.txt";
        FILE* stdoutFile = nullptr;
        FILE* stderrFile = nullptr;
        _wfreopen_s(&stdoutFile, stdoutPath->Data(), L"w", stdout);
        _wfreopen_s(&stderrFile, stderrPath->Data(), L"w", stderr);

        return bOk;
    }

    IAsyncAction^ Engine::LoadRomAsync(StorageFile^ romFile)
    {
        return create_async([=]()
        {
            Lock lock(_engineMutex);
            //if (_currentRom != nullptr)
            //{
            //    S9xWrapper::SaveSRAM(CW2A(GetSavePath()->Data()));
            //}

            if (S9xWrapper::LoadRom(CW2A(romFile->Path->Data())))
            {
                _currentRom = romFile;
                //S9xWrapper::LoadSRAM(CW2A(GetSavePath()->Data()));
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

    bool Engine::SaveState(String^ path)
    {
        Lock lock(_engineMutex);
        return S9xWrapper::SaveState(CW2A(path->Data()));
    }

    bool Engine::LoadState(String^ path)
    {
        Lock lock(_engineMutex);
        return S9xWrapper::LoadState(CW2A(path->Data()));
    }

    IAsyncAction^ Engine::SaveSramAsync()
    {
        return create_async([this]()
        {
            Lock lock(_engineMutex);
            S9xWrapper::SaveSRAM(CW2A(GetSavePath()->Data()));
        });
    }

    IAsyncAction^ Engine::LoadSramAsync()
    {
        return create_async([this]()
        {
            Lock lock(_engineMutex);
            S9xWrapper::LoadSRAM(CW2A(GetSavePath()->Data()));
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

    String^ Engine::GetSavePath()
    {
        if (_currentRom == nullptr) return nullptr;
        std::experimental::filesystem::path path(_savesFolder->Path->Data());
        path = path.append(_currentRom->DisplayName->Data()).replace_extension(".srm");
        return ref new String(path.c_str());
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