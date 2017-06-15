#pragma once

namespace Snes9x { namespace Core
{
    public ref class Surface sealed
    {
    public:
        Surface(int width, int height, int pitch, IBuffer^ bytes)
        {
            Width = width;
            Height = height;
            Pitch = pitch;
            Bytes = bytes;
        }

        property int Width;
        property int Height;
        property int Pitch;
        property IBuffer^ Bytes;
    };

    ref class Engine;
    public delegate void SramChangedEventHandler(Engine^ engine);

    public ref class Engine sealed
    {
    private:
        Engine();

        // Public Events
    public:
        event SramChangedEventHandler^ SramChanged;

        // Public Properties
    public:
        static property Engine^ Instance
        {
            Engine^ get() { return g_Emulator; }
        }

        property Windows::Storage::StorageFile^ CurrentRom
        {
            Windows::Storage::StorageFile^ get() { return _currentRom; }
        }

        // Represents if a rom is loaded and update/save etc can be called
        property bool Active
        {
            bool get() { return _currentRom != nullptr; }
        }

        // Public Methods
    public:
        void Init(Windows::Storage::StorageFolder^ savesFolder);
        IAsyncAction^ LoadRomAsync(Windows::Storage::StorageFile^ romFile);

        Surface^ Update();

        bool SaveState(Platform::String^ path);
        bool LoadState(Platform::String^ path);

        IAsyncOperation<IBuffer^>^ SaveSramAsync();
        IAsyncAction^ LoadSramAsync();
        //bool LoadSRAM(String^ path);

        Platform::String^ GetSavePath();

    internal:
        void SetResolution(int width, int height);
        void OnSramChanged();

    private:
        static void ConvertDepth16to32(Surface^ source, Surface^ destination);
        static byte* GetBufferByteAccess(IBuffer^ buffer);

    private:
        static Engine^ g_Emulator;

        Surface^ _snesScreen;
        Surface^ _renderedScreen;

        std::mutex _engineMutex;
        typedef std::lock_guard<std::mutex> Lock;

        Windows::Storage::StorageFolder^ _savesFolder;
        Windows::Storage::StorageFile^ _currentRom;
    };
} }