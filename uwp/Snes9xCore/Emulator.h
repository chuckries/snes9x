#pragma once

namespace Snes9xCore
{
    interface class IRom;
    ref class CoreSettings;

    public ref class Surface sealed
    {
    public:
        Surface(int width, int height, int pitch, const Array<byte>^ bytes)
        {
            Width = width;
            Height = height;
            Pitch = pitch;
            Bytes = bytes;
        }

        property int Width;
        property int Height;
        property int Pitch;
        property Array<byte>^ Bytes;
    };

    public ref class CoreEmulator sealed
    {
    private:
        CoreEmulator();

        // Public Properties
    public:
        static property CoreEmulator^ Instance
        {
            CoreEmulator^ get() { return g_Emulator; }
        }

        // The Current settings
        property CoreSettings^ Settings
        {
            CoreSettings^ get() { return _settings; }
        }

        // Pulbic Methods
    public:
        bool Init();
        bool LoadRomMem(const Array<byte>^ romBytes);

        Surface^ Update();

        bool SaveState(String^ path);
        bool LoadState(String^ path);

        bool SaveSRAM(String^ path);
        bool LoadSRAM(String^ path);

    internal:
        void SetResolution(int width, int height);

    private:
        static void ConvertDepth16to32(Surface^ source, Surface^ destination);

    private:
        static CoreEmulator^ g_Emulator;

        IRom^ _rom;
        CoreSettings^ _settings;

        Surface^ _snesScreen;
        Surface^ _renderedScreen;
    };
}