#pragma once

namespace Snes9xWRC
{

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

    public ref class Renderer sealed
    {
    public:
        Renderer();

        void Init();
        Surface^ GetRenderedSurface();
        void SetResolution(int width, int height);

    private:
        static void ConvertDepth16to32(Surface^ source, Surface^ destination);

    private:
        Surface^ _snesScreen;
        Surface^ _renderedScreen;
    };

}