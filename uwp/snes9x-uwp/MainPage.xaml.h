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


        property double Amount;
        property bool IsFiltered;

    private:
        void canvas_Draw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args);
        void canvas_Update(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args);
        void canvas_CreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args);
        Windows::Foundation::IAsyncAction^ canvas_CreateResourcesAsync(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args);

    private:
        CanvasBitmap^ _bitmap;
        Array<byte>^ _bitmapBytes;

        PixelShaderEffect^ _effect;
        CropEffect^ _crop;
        CanvasBitmap^ _hqxLut;

        static const int s_height;
        static const int s_width;

        EmuCore^ _core;
        void canvas_Loaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OnSizeChanged(Platform::Object ^sender, Windows::UI::Xaml::SizeChangedEventArgs ^e);
        void OnActivated(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::WindowActivatedEventArgs ^args);
        void OnKeyDown(CoreWindow^ sender, KeyEventArgs^ args);
        void OnKeyUp(CoreWindow^ sender, KeyEventArgs^ args);

        void ReportButtons();

    private:
        bool _A;
        bool _B;
        bool _X;
        bool _Y;
        bool _L;
        bool _R;
        bool _Start;
        bool _Select;
        bool _Up;
        bool _Down;
        bool _Left;
        bool _Right;
        void ScreenshotButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ToggleSwitch_Toggled(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        Microsoft::Graphics::Canvas::CanvasImageInterpolation _interpolation;
        void Slider_ValueChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e);
        void MenuButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SaveStateButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LoadStateButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);


    };
}
