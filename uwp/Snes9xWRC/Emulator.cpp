#include "pch.h"
#include "Emulator.h"
#include "Renderer.h"
#include "Rom.h"
#include "Snes9xCore.h"
#include "Settings.h"

#define SNES_WIDTH                  256
#define SNES_HEIGHT                 224
#define SNES_HEIGHT_EXTENDED        239
#define MAX_SNES_WIDTH              (SNES_WIDTH * 2)
#define MAX_SNES_HEIGHT             (SNES_HEIGHT_EXTENDED * 2)

namespace Snes9xWRC
{
    Emulator^ Emulator::g_Emulator = ref new Emulator();

    Emulator::Emulator()
        : _settings(ref new CoreSettings())
        , _renderer(ref new Snes9xWRC::Renderer())
        , _snesScreen(ref new Array<byte>(MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 2))
        , _bitmapBytes(ref new Array<byte>(MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 4))
    {
    }

    void Emulator::Init()
    {
        Core::InitMemory();
        Core::InitApu();

        _renderer->Init();

        Core::InitSound(0, 0);
    }

    IAsyncOperation<bool>^ Emulator::LoadRomAsync(IRom^ rom)
    {
        return create_async([=]()
        {
            return create_task(rom->GetBytesAsync()).then([=](Object^ obj)
            {
                Array<byte>^ bytes = safe_cast<IBoxArray<byte>^>(obj)->Value;
                return bytes != nullptr && Core::LoadRomMem(bytes->begin(), bytes->Length);
            });
        });
    }

    bool Emulator::LoadRomMem(const Array<byte>^ romBytes)
    {
        return Core::LoadRomMem(romBytes->begin(), romBytes->Length);
    }

    void Emulator::MainLoop()
    {
        Core::MainLoop();
    }

    void Emulator::CreateDeviceResources(ICanvasResourceCreatorWithDpi^ creator)
    {
        _bitmap = CanvasBitmap::CreateFromBytes(creator, _bitmapBytes, MAX_SNES_WIDTH, MAX_SNES_HEIGHT, Windows::Graphics::DirectX::DirectXPixelFormat::B8G8R8A8UIntNormalized, creator->Dpi, CanvasAlphaMode::Ignore);
    }

    void Emulator::Draw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args)
    {
        memset(_bitmapBytes->begin(), 0xff, _bitmapBytes->Length);

        ConvertDepth();

        _bitmap->SetPixelBytes(_bitmapBytes, 0, 0, _width, _height);

        Rect dstRect(Point(0.0f, 0.0f), sender->Size);
        Rect srcRect(Point(0.0f, 0.0f), Size(_bitmap->ConvertPixelsToDips(_width), _bitmap->ConvertPixelsToDips(_height)));

        args->DrawingSession->DrawImage(_bitmap, dstRect, srcRect);
    }

    bool Emulator::DeInitUpdate(uint32_t width, uint32_t height)
    {
        _width = width;
        _height = height;
        return true;
    }

    void Emulator::ConvertDepth()
    {
        uint16_t* src = (uint16_t*)_snesScreen->begin();
        uint32_t* dst = (uint32_t*)_bitmapBytes->begin();

        for (uint32_t row = 0; row < _height; row++)
        {
            for (uint32_t col = 0; col < _width; col++)
            {
                uint16_t pixel = *(src + col);

                uint32_t r = (pixel & 0xF800) >> 11;
                uint32_t g = (pixel & 0x07E0) >> 5;
                uint32_t b = (pixel & 0x001F);

                r = r * 255 / 31;
                g = g * 255 / 63;
                b = b * 255 / 31;

                *dst++ = (r << 16) | (g << 8) | b;
            }
            src += MAX_SNES_WIDTH;
        }
    }
}