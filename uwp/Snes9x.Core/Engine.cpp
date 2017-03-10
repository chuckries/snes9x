#include "pch.h"
#include "Engine.h"
#include "Snes9xWrapper.h"
#include "Settings.h"
#include "StringUtil.h"

using namespace Platform;
using namespace Windows::Storage;

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
    {
        _snesScreen = ref new Surface(
            MAX_SNES_WIDTH,
            MAX_SNES_HEIGHT,
            MAX_SNES_WIDTH * 2,
            ref new Array<byte>(MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 2)
        );
        _renderedScreen = ref new Surface(0, 0, 0, nullptr);
    }

    bool Engine::Init()
    {
        bool bOk = true;

        S9xWrapper::InitMemory();
        bOk = bOk && S9xWrapper::InitApu();

        bOk = bOk && S9xWrapper::InitGraphics((uint16_t*)_snesScreen->Bytes->begin(), _snesScreen->Pitch);

        bOk = bOk && S9xWrapper::InitSound(128, 0);

        S9xWrapper::InitControllers();

        return bOk;
    }

    bool Engine::LoadRomMem(IBuffer^ buffer)
    {
        DataReader^ reader = DataReader::FromBuffer(buffer);
        Array<byte>^ bytes = ref new Array<byte>(buffer->Length);
        reader->ReadBytes(bytes);
        return S9xWrapper::LoadRomMem(bytes->begin(), bytes->Length);
    }

    Surface^ Engine::Update()
    {
        S9xWrapper::MainLoop();

        if (_renderedScreen->Width != _snesScreen->Width || _renderedScreen->Height != _snesScreen->Height)
        {
            _renderedScreen->Width = _snesScreen->Width;
            _renderedScreen->Height = _snesScreen->Height;
            _renderedScreen->Pitch = _snesScreen->Width * 4;
            _renderedScreen->Bytes = ref new Array<byte>(_renderedScreen->Height * _renderedScreen->Pitch);
        }

        ConvertDepth16to32(_snesScreen, _renderedScreen);

        return _renderedScreen;
    }

    bool Engine::SaveState(String^ path)
    {
        return S9xWrapper::SaveState(WideToUtf8(path->Data()));
    }

    bool Engine::LoadState(String^ path)
    {
        return S9xWrapper::LoadState(WideToUtf8(path->Data()));
    }

    bool Engine::SaveSRAM(String^ path)
    {
        return S9xWrapper::SaveSRAM(WideToUtf8(path->Data()));
    }

    bool Engine::LoadSRAM(String^ path)
    {
        return S9xWrapper::LoadSRAM(WideToUtf8(path->Data()));
    }

    void Engine::SetResolution(int width, int height)
    {
        _snesScreen->Width = width;
        _snesScreen->Height = height;
    }

    void Engine::ConvertDepth16to32(Surface^ source, Surface^ destination)
    {
        uint16_t* src = (uint16_t*)source->Bytes->begin();
        uint32_t* dst = (uint32_t*)destination->Bytes->begin();

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
} }