#include "pch.h"
#include "Emulator.h"
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

namespace Snes9xCore
{
    CoreEmulator^ CoreEmulator::g_Emulator = ref new CoreEmulator();

    CoreEmulator::CoreEmulator()
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

    bool CoreEmulator::Init()
    {
        bool bOk = true;

        S9xWrapper::InitMemory();
        bOk = bOk && S9xWrapper::InitApu();

        bOk = bOk && S9xWrapper::InitGraphics((uint16_t*)_snesScreen->Bytes->begin(), _snesScreen->Pitch);

        bOk = bOk && S9xWrapper::InitSound(128, 0);

        return bOk;
    }

    bool CoreEmulator::LoadRomMem(const Array<byte>^ romBytes)
    {
        return S9xWrapper::LoadRomMem(romBytes->begin(), romBytes->Length);
    }

    void CoreEmulator::MainLoop()
    {
        S9xWrapper::MainLoop();
    }

    bool CoreEmulator::SaveState(String^ path)
    {
        return S9xWrapper::SaveState(WideToUtf8(path->Data()));
    }

    bool CoreEmulator::LoadState(String^ path)
    {
        return S9xWrapper::LoadState(WideToUtf8(path->Data()));
    }

    void CoreEmulator::SetResolution(int width, int height)
    {
        _snesScreen->Width = width;
        _snesScreen->Height = height;
    }

    Surface^ CoreEmulator::GetRenderedSurface()
    {
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

    void CoreEmulator::ConvertDepth16to32(Surface^ source, Surface^ destination)
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
}