using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9xWRC;
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Snes9x
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmulatorPage : Page
    {
        private Emulator Emulator;

        private CanvasBitmap _bitmap;

        public EmulatorPage()
        {
            this.InitializeComponent();
            Emulator = App.Emulator;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            canvas.Paused = true;

            IRom rom = e.Parameter as IRom;
            if (rom != null)
            {
                if (await Emulator.LoadRomAsync(rom))
                {
                    canvas.Paused = false;
                }
            }

            base.OnNavigatedTo(e);
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _bitmap = CanvasBitmap.CreateFromBytes(sender, new byte[0], 0, 0, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, sender.Dpi, CanvasAlphaMode.Ignore);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            Emulator.MainLoop();

            //_random.NextBytes(_bitmapBytes);
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            Surface surface = Emulator.Renderer.GetRenderedSurface();
            if (_bitmap.SizeInPixels.Width != surface.Width || _bitmap.SizeInPixels.Height != surface.Height)
            {
                _bitmap = CanvasBitmap.CreateFromBytes(sender, surface.Bytes, surface.Width, surface.Height, _bitmap.Format);
            }
            else
            {
                _bitmap.SetPixelBytes(surface.Bytes);
            }

            args.DrawingSession.DrawImage(_bitmap, new Rect(new Point(0.0f, 0.0f), sender.Size), _bitmap.Bounds, 1.0f, CanvasImageInterpolation.Linear);

            //Emulator.Draw(sender, args);
            //_bitmap.SetPixelBytes(_bitmapBytes);
            //Rect srcRect = _bitmap.Bounds;
            //Rect dstRect = new Rect(new Point(0, 0), sender.Size);
            //args.DrawingSession.DrawImage(_bitmap, dstRect, srcRect, 1, CanvasImageInterpolation.NearestNeighbor);
        }

        private void Page_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void ContentPresenter_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            double aspectRatio = 8.0 / 7.0;

            double scaledWidth = e.NewSize.Height * aspectRatio;
            double scaledHeight = e.NewSize.Width / aspectRatio;

            if (scaledWidth > e.NewSize.Width)
            {
                canvas.Width = e.NewSize.Width;
                canvas.Height = scaledHeight;
            }
            else
            {
                canvas.Width = scaledWidth;
                canvas.Height = e.NewSize.Height;
            }
        }

        private async void ToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            if (toggle != null)
            {
                if (canvas != null)
                {
                    bool isOn = toggle.IsOn;

                    await canvas.RunOnGameLoopThreadAsync(() =>
                    {
                        Emulator.Settings.DisplayFrameRate = isOn;
                    });
                }
            }
        }

        private void ScreenshotButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }
    }
}
