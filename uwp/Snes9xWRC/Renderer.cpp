#include "pch.h"
#include "Renderer.h"
#include "Snes9xCore.h"

#define SNES_WIDTH                  256
#define SNES_HEIGHT                 224
#define SNES_HEIGHT_EXTENDED        239
#define MAX_SNES_WIDTH              (SNES_WIDTH * 2)
#define MAX_SNES_HEIGHT             (SNES_HEIGHT_EXTENDED * 2)

namespace Snes9xWRC
{
    Renderer::Renderer()
    {
        _snesScreen = ref new Surface(
            MAX_SNES_WIDTH,
            MAX_SNES_HEIGHT,
            MAX_SNES_WIDTH * 2,
            ref new Array<byte>(MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 2)
        );
        _renderedScreen = ref new Surface(0, 0, 0, nullptr);
    }

    void Renderer::Init()
    {
        Core::InitGraphics((uint16_t*)_snesScreen->Bytes->begin(), _snesScreen->Pitch);
    }

    Surface^ Renderer::GetRenderedSurface()
    {
        if (_renderedScreen->Width != _snesScreen->Width || _renderedScreen->Height != _snesScreen->Height)
        {
            _renderedScreen->Width = _snesScreen->Width;
            _renderedScreen->Height = _snesScreen->Height;
            _renderedScreen->Pitch = _snesScreen->Width * 4;
            _renderedScreen->Bytes = ref new Array<byte>(_snesScreen->Width * _snesScreen->Height * 4);
        }

        ConvertDepth16to32(_snesScreen, _renderedScreen);

        return _renderedScreen;
    }

    void Renderer::SetResolution(int width, int height)
    {
        _snesScreen->Width = width;
        _snesScreen->Height = height;
    }

    void Renderer::ConvertDepth16to32(Surface^ source, Surface^ destination)
    {
        uint16_t* src = (uint16_t*)source->Bytes->begin();
        uint32_t* dst = (uint32_t*)destination->Bytes->begin();

        for (uint32_t line = 0; line < source->Height && line < destination->Height; line++)
        {
            for (uint32_t x = 0; x < source->Width && x < destination->Width; x++)
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