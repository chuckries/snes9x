#pragma once

namespace Snes9xWRC
{
    interface class IRom;
    ref class Renderer;
    ref class CoreSettings;

    public ref class Emulator sealed
    {
    private:
        Emulator();

        // Public Properties
    public:
        static property Emulator^ Instance
        {
            Emulator^ get() { return g_Emulator; }
        }

        property Renderer^ Renderer
        {
            Snes9xWRC::Renderer^ get() { return _renderer; }
        }

        // The currently loaded Rom
        property IRom^ Rom
        {
            IRom^ get() { return _rom; }
        }

        // The Current settings
        property CoreSettings^ Settings
        {
            CoreSettings^ get() { return _settings; }
        }

        // Pulbic Methods
    public:
        void Init();
        IAsyncOperation<bool>^ LoadRomAsync(IRom^ rom);
        bool LoadRomMem(const Array<byte>^ romBytes);

        void MainLoop();

        void CreateDeviceResources(ICanvasResourceCreatorWithDpi^ resourceCreator);
        void Draw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args);

        // Internal Methods
    internal:
        bool DeInitUpdate(uint32_t width, uint32_t height);

        // Private Methods
    private:
        void ConvertDepth();

    private:
        static Emulator^ g_Emulator;
        Snes9xWRC::Renderer^ _renderer;
        IRom^ _rom;
        CoreSettings^ _settings;
        Array<byte>^ _snesScreen; // Rendered SNES screen, 16 bit colors, maximum of 512 * 478 pixels, 2 bytse per pixel
        Array<byte>^ _bitmapBytes; // bytes for final bitmap to be drawn to screen, depth converted and filtered snes screen
        CanvasBitmap^ _bitmap; // The bitmap we will draw to the canvas, contains the depth converterd and filtered snes screen
        uint32_t _width; // current width of _snesScreen in pixels
        uint32_t _height; // current heigh of _scnesscreen in pixels
    };
}