//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include <random>


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

const int MainPage::s_width = 50;
const int MainPage::s_height = 50;

MainPage::MainPage()
    : _bitmapBytes(ref new Array<byte>(s_width * s_height * 4))
{
    InitializeComponent();
}

void MainPage::canvas_Draw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args)
{
    _bitmap->SetPixelBytes(_bitmapBytes);
    args->DrawingSession->DrawImage(_bitmap, Rect(Point(0, 0), sender->Size), _bitmap->Bounds, 1, CanvasImageInterpolation::NearestNeighbor);
}


void MainPage::canvas_Update(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args)
{
    std::random_device rd;
    std::default_random_engine generator(rd());
    std::uniform_int_distribution<> distribution(0, 255);

    for (uint32_t i = 0; i < _bitmapBytes->Length; i++)
    {
        _bitmapBytes[i] = distribution(generator);
    }
}


void MainPage::canvas_CreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args)
{
    _bitmap = CanvasBitmap::CreateFromBytes(sender, _bitmapBytes, s_width, s_height, Windows::Graphics::DirectX::DirectXPixelFormat::B8G8R8A8UIntNormalized, sender->Dpi, CanvasAlphaMode::Ignore);
}
