//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

namespace snes9x_uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public ref class MainPage sealed
    {
    public:
        MainPage();

    private:
        void canvas_Draw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args);
        void canvas_Update(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args);
        void canvas_CreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args);

    private:
        CanvasBitmap^ _bitmap;
        Array<byte>^ _bitmapBytes;

        static const int s_height;
        static const int s_width;
    };
}
