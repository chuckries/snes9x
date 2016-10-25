//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include <random>
#include "EmuCore.h"
#include <ctime>
#include <d2d1.h>
#include <ppl.h>

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

const int MainPage::s_width = 50;
const int MainPage::s_height = 50;

MainPage::MainPage()
    : _bitmapBytes(ref new Array<byte>(MAX_SNES_WIDTH * MAX_SNES_HEIGHT * 4))
{
    InitializeComponent();
    Window::Current->CoreWindow->Activated += ref new TypedEventHandler<CoreWindow ^, WindowActivatedEventArgs ^>(this, &snes9x_uwp::MainPage::OnActivated);
    Window::Current->CoreWindow->KeyDown += ref new TypedEventHandler<CoreWindow ^, KeyEventArgs ^>(this, &snes9x_uwp::MainPage::OnKeyDown);
    Window::Current->CoreWindow->KeyUp += ref new TypedEventHandler<CoreWindow ^, KeyEventArgs ^>(this, &snes9x_uwp::MainPage::OnKeyUp);

    _interpolation = CanvasImageInterpolation::Linear;
}

int myGlobalInt = 0;

void MainPage::canvas_Draw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args)
{
    memset((void*)_bitmapBytes->begin(), 0, _bitmapBytes->Length);
    //_bitmap->SetPixelBytes(_core->Screen);
    uint16_t* src = (uint16_t*)_core->Screen->begin();
    uint32_t* dst = (uint32_t*)_bitmapBytes->begin();

    for (int i = 0; i < MAX_SNES_WIDTH * MAX_SNES_HEIGHT; i++)
    {
        uint16_t pixel = *src++;

        unsigned long r = (pixel & 0x7C00) >> 10;
        unsigned long g = (pixel & 0x03E0) >> 5;
        unsigned long b = (pixel & 0x001F);

        r = r * 255 / 31;
        g = g * 255 / 31;
        b = b * 255 / 31;

        *dst++ = 0xFF000000 | (r << 16) | (g << 8) | b;

        //*dst++ = (((((pixel) >> 11)) << /*RedShift+3*/  19) | \
        //    ((((pixel) >> 5) & 0x3f) << /*GreenShift+2*/10) | \
        //    (((pixel) & 0x1f) << /*BlueShift+3*/ 3));
    }
    _bitmap->SetPixelBytes(_bitmapBytes);
    Rect srcRect(Point(0.0f, 0.0f), Size(_bitmap->ConvertPixelsToDips(_core->Width), _bitmap->ConvertPixelsToDips(_core->Height)));

    ICanvasImage^ imageToDraw = _bitmap;
    //if (IsFiltered)
    if (true)
    {
        _effect->Properties->Insert("textureX", (float)MAX_SNES_WIDTH);
        _effect->Properties->Insert("textureY", (float)MAX_SNES_HEIGHT);
        _effect->Properties->Insert("scale", 4.0f);

        _crop->SourceRectangle = Rect(0.0f, 0.0f, srcRect.Width * 4.0f, srcRect.Height * 4.0f);

        imageToDraw = _crop;
    }

    args->DrawingSession->DrawImage(imageToDraw, Rect(Point(0.0f, 0.0f), sender->Size), imageToDraw->GetBounds(sender), 1.0f, CanvasImageInterpolation::Linear);
}


void MainPage::canvas_Update(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args)
{
    ReportButtons();
    S9xMainLoop();
}

void MainPage::ReportButtons()
{
    S9xReportButton(0, _X);
    S9xReportButton(1, _A);
    S9xReportButton(2, _B);
    S9xReportButton(3, _Y);
    S9xReportButton(4, _L);
    S9xReportButton(5, _R);
    S9xReportButton(6, _Select);
    S9xReportButton(7, _Start);
    S9xReportButton(8, _Up);
    S9xReportButton(9, _Down);
    S9xReportButton(10, _Left);
    S9xReportButton(11, _Right);

    if (Gamepad::Gamepads->Size > 0)
    {
        Gamepad^ gamepad = Gamepad::Gamepads->GetAt(0);

        auto reading = gamepad->GetCurrentReading();
        auto buttons = reading.Buttons;

        S9xReportButton(0, ((buttons & GamepadButtons::Y) == GamepadButtons::Y));
        S9xReportButton(1, ((buttons & GamepadButtons::B) == GamepadButtons::B));
        S9xReportButton(2, ((buttons & GamepadButtons::A) == GamepadButtons::A));
        S9xReportButton(3, ((buttons & GamepadButtons::X) == GamepadButtons::X));
        S9xReportButton(4, ((buttons & GamepadButtons::LeftShoulder) == GamepadButtons::LeftShoulder));
        S9xReportButton(5, ((buttons & GamepadButtons::RightShoulder) == GamepadButtons::RightShoulder));
        S9xReportButton(6, ((buttons & GamepadButtons::View) == GamepadButtons::View));
        S9xReportButton(7, ((buttons & GamepadButtons::Menu) == GamepadButtons::Menu));
        S9xReportButton(8, ((buttons & GamepadButtons::DPadUp) == GamepadButtons::DPadUp));
        S9xReportButton(9, ((buttons & GamepadButtons::DPadDown) == GamepadButtons::DPadDown));
        S9xReportButton(10, ((buttons & GamepadButtons::DPadLeft) == GamepadButtons::DPadLeft));
        S9xReportButton(11, ((buttons & GamepadButtons::DPadRight) == GamepadButtons::DPadRight));

        if (Gamepad::Gamepads->Size > 1)
        {
            gamepad = Gamepad::Gamepads->GetAt(1);

            reading = gamepad->GetCurrentReading();
            buttons = reading.Buttons;

            S9xReportButton(12, ((buttons & GamepadButtons::Y) == GamepadButtons::Y));
            S9xReportButton(13, ((buttons & GamepadButtons::B) == GamepadButtons::B));
            S9xReportButton(14, ((buttons & GamepadButtons::A) == GamepadButtons::A));
            S9xReportButton(15, ((buttons & GamepadButtons::X) == GamepadButtons::X));
            S9xReportButton(16, ((buttons & GamepadButtons::LeftShoulder) == GamepadButtons::LeftShoulder));
            S9xReportButton(17, ((buttons & GamepadButtons::RightShoulder) == GamepadButtons::RightShoulder));
            S9xReportButton(18, ((buttons & GamepadButtons::View) == GamepadButtons::View));
            S9xReportButton(19, ((buttons & GamepadButtons::Menu) == GamepadButtons::Menu));
            S9xReportButton(20, ((buttons & GamepadButtons::DPadUp) == GamepadButtons::DPadUp));
            S9xReportButton(21, ((buttons & GamepadButtons::DPadDown) == GamepadButtons::DPadDown));
            S9xReportButton(22, ((buttons & GamepadButtons::DPadLeft) == GamepadButtons::DPadLeft));
            S9xReportButton(23, ((buttons & GamepadButtons::DPadRight) == GamepadButtons::DPadRight));
        }
    }
}


void MainPage::canvas_CreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args)
{
args->TrackAsyncAction(canvas_CreateResourcesAsync(sender, args));
}

IAsyncAction^ MainPage::canvas_CreateResourcesAsync(CanvasAnimatedControl ^ sender, CanvasCreateResourcesEventArgs ^ args)
{
    _bitmap = CanvasBitmap::CreateFromBytes(sender, _bitmapBytes, MAX_SNES_WIDTH, MAX_SNES_HEIGHT, Windows::Graphics::DirectX::DirectXPixelFormat::B8G8R8A8UIntNormalized, sender->Dpi, CanvasAlphaMode::Ignore);
    _core = ref new EmuCore();
    App::Core = _core;
    _crop = ref new CropEffect();
    return create_async([this, sender]()
    {
        return create_task(_core->Init()).then([this, sender](bool success)
        {
            return create_task(StorageFile::GetFileFromApplicationUriAsync(ref new Uri("ms-appx:///Shader.bin"))).then([this](task<StorageFile^> t)
            {
                try
                {
                    auto shaderFile = t.get();
                    return create_task(FileIO::ReadBufferAsync(shaderFile));
                }
                catch (COMException^ e)
                {
                    __debugbreak();
                }
            }).then([this](IBuffer^ buffer)
            {
                auto reader = DataReader::FromBuffer(buffer);

                Array<byte>^ bytes = ref new Array<byte>(buffer->Length);
                reader->ReadBytes(bytes);
                _effect = ref new PixelShaderEffect(bytes);

                return create_task(StorageFile::GetFileFromApplicationUriAsync(ref new Uri("ms-appx:///hq4x.data")));
            }).then([this](StorageFile^ lutFile)
            {
                return create_task(FileIO::ReadBufferAsync(lutFile));
            }).then([this, sender](IBuffer^ buffer) {
                _hqxLut = CanvasBitmap::CreateFromBytes(sender, buffer, 256, 256, Windows::Graphics::DirectX::DirectXPixelFormat::R8G8B8A8UIntNormalized, sender->Dpi, CanvasAlphaMode::Ignore);

                _effect->Source1 = _bitmap;
                _effect->Source1Interpolation = CanvasImageInterpolation::NearestNeighbor;
                _effect->Source2 = _hqxLut;
                _effect->Source2Interpolation = CanvasImageInterpolation::NearestNeighbor;

                _crop = ref new CropEffect();
                _crop->Source = _effect;
            }).then([this, success]()
            {
                if (success)
                {
                    canvas->Paused = false;
                }
            });
        });
    });
}


void snes9x_uwp::MainPage::canvas_Loaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    canvas->Paused = true;
}


void snes9x_uwp::MainPage::OnSizeChanged(Platform::Object ^sender, Windows::UI::Xaml::SizeChangedEventArgs ^e)
{
    float aspectRatio = 8.0f / 7.0f;

    float scaledWidth = e->NewSize.Height * aspectRatio;
    float scaledHeight = e->NewSize.Width / aspectRatio;
    if (scaledWidth > e->NewSize.Width)
    {
        canvas->Width = e->NewSize.Width;
        canvas->Height = scaledHeight;
    }
    else
    {
        canvas->Width = scaledWidth;
        canvas->Height = e->NewSize.Height;
    }
}


void snes9x_uwp::MainPage::OnActivated(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::WindowActivatedEventArgs ^args)
{
    if (args->WindowActivationState == Windows::UI::Core::CoreWindowActivationState::Deactivated)
    {
        canvas->Paused = true;
    }
    else
    {
        canvas->Paused = false;
    }
}

void snes9x_uwp::MainPage::OnKeyDown(CoreWindow^ sender, KeyEventArgs^ args)
{
    switch (args->VirtualKey)
    {
    case Windows::System::VirtualKey::C:        _A = true; args->Handled = true; break;
    case Windows::System::VirtualKey::X:        _B = true; args->Handled = true; break;
    case Windows::System::VirtualKey::D:        _X = true; args->Handled = true; break;
    case Windows::System::VirtualKey::S:        _Y = true; args->Handled = true; break;
    case Windows::System::VirtualKey::E:        _R = true; args->Handled = true; break;
    case Windows::System::VirtualKey::W:        _L = true; args->Handled = true; break;
    case Windows::System::VirtualKey::Up:       _Up = true; args->Handled = true; break;
    case Windows::System::VirtualKey::Down:     _Down = true; args->Handled = true; break;
    case Windows::System::VirtualKey::Left:     _Left = true; args->Handled = true; break;
    case Windows::System::VirtualKey::Right:    _Right = true; args->Handled = true; break;
    case Windows::System::VirtualKey::Enter:    _Start = true; args->Handled = true; break;
    case Windows::System::VirtualKey::Shift:    _Select = true; args->Handled = true; break;
    }
}

void snes9x_uwp::MainPage::OnKeyUp(CoreWindow^ sender, KeyEventArgs^ args)
{
    switch (args->VirtualKey)
    {
    case Windows::System::VirtualKey::C:        _A = false; args->Handled = true; break;
    case Windows::System::VirtualKey::X:        _B = false; args->Handled = true; break;
    case Windows::System::VirtualKey::D:        _X = false; args->Handled = true; break;
    case Windows::System::VirtualKey::S:        _Y = false; args->Handled = true; break;
    case Windows::System::VirtualKey::E:        _R = false; args->Handled = true; break;
    case Windows::System::VirtualKey::W:        _L = false; args->Handled = true; break;
    case Windows::System::VirtualKey::Up:       _Up = false; args->Handled = true; break;
    case Windows::System::VirtualKey::Down:     _Down = false; args->Handled = true; break;
    case Windows::System::VirtualKey::Left:     _Left = false; args->Handled = true; break;
    case Windows::System::VirtualKey::Right:    _Right = false; args->Handled = true; break;
    case Windows::System::VirtualKey::Enter:    _Start = false; args->Handled = true; break;
    case Windows::System::VirtualKey::Shift:    _Select = false; args->Handled = true; break;
    }
}

void snes9x_uwp::MainPage::ScreenshotButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    std::wstring fileName(_core->RomName->Data());
    fileName.append(L"-");
    std::time_t now = std::time(nullptr);
    fileName.append(std::to_wstring(now));
    fileName.append(L".png");

    create_task(KnownFolders::PicturesLibrary->CreateFileAsync(ref new Platform::String(fileName.c_str()), CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file)
    {
        return create_task(file->OpenAsync(FileAccessMode::ReadWrite));
    }).then([this](IRandomAccessStream^ stream) {
        canvas->RunOnGameLoopThreadAsync(ref new DispatchedHandler([this, stream]() {
            auto screenshotBitmap = CanvasBitmap::CreateFromBytes(canvas, _bitmap->GetPixelBytes(0, 0, _core->Width, _core->Height), _core->Width, _core->Height, _bitmap->Format);

            create_task(screenshotBitmap->SaveAsync(stream, CanvasBitmapFileFormat::Png, 1.0f)).wait();
        }));
    });
}


void snes9x_uwp::MainPage::ToggleSwitch_Toggled(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto toggleSwith = safe_cast<ToggleSwitch^>(sender);
    if (toggleSwith->IsOn)
    {
        _interpolation = CanvasImageInterpolation::Linear;
    }
    else
    {
        _interpolation = CanvasImageInterpolation::HighQualityCubic;
    }
}


void snes9x_uwp::MainPage::Slider_ValueChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e)
{
    Slider^ slider = safe_cast<Slider^>(sender);
}


void snes9x_uwp::MainPage::MenuButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    canvas->Paused = true;
    PauseMenu->Visibility = Windows::UI::Xaml::Visibility::Visible;
}


void snes9x_uwp::MainPage::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    PauseMenu->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    canvas->Paused = false;
}


void snes9x_uwp::MainPage::SaveStateButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    canvas->RunOnGameLoopThreadAsync(ref new DispatchedHandler([=]() {
        App::Core->SaveState();
    }));
}


void snes9x_uwp::MainPage::LoadStateButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    canvas->RunOnGameLoopThreadAsync(ref new DispatchedHandler([=]() {
        App::Core->LoadState();
    }));
}
