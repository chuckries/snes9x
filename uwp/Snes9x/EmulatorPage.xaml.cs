using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9x.Common;
using Snes9xCore;
using System;
using System.Threading.Tasks;
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
        private CanvasBitmap _bitmap;

        public EmulatorPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            canvas.Paused = true;

            IRomFile rom = e.Parameter as IRomFile;
            if (rom != null)
            {
                await Emulator.Instance.LoadRomAsync(rom);
                canvas.Paused = false;
            }
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _bitmap = CanvasBitmap.CreateFromBytes(sender, new byte[0], 0, 0, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, sender.Dpi, CanvasAlphaMode.Ignore);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            Emulator.Instance.Update();
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            Surface surface = Emulator.Instance.Screen;
            if (_bitmap.SizeInPixels.Width != surface.Width || _bitmap.SizeInPixels.Height != surface.Height)
            {
                _bitmap = CanvasBitmap.CreateFromBytes(sender, surface.Bytes, surface.Width, surface.Height, _bitmap.Format);
            }
            else
            {
                _bitmap.SetPixelBytes(surface.Bytes);
            }

            args.DrawingSession.DrawImage(_bitmap, new Rect(new Point(0.0f, 0.0f), sender.Size), _bitmap.Bounds, 1.0f, CanvasImageInterpolation.Linear);
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

        private async void ScreenshotButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string fileName = $"{Emulator.Instance.Rom.Name}-{DateTime.Now.ToString("yyyy-MM-ddHHmmss")}.png";

            await canvas.RunOnGameLoopThreadAsync(() => {
                Task.Run(async () =>
                {
                    var file = await Emulator.Instance.Directories.ScreenshotsFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        await _bitmap.SaveAsync(fileStream, CanvasBitmapFileFormat.Png, 1.0f);
                    }
                }).Wait();
            });
        }

        private async void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            await canvas.RunOnGameLoopThreadAsync(() =>
            {
                Emulator.Instance.SaveState();
            });
            button.IsEnabled = true;
        }

        private async void LoadButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            await canvas.RunOnGameLoopThreadAsync(() =>
            {
                Emulator.Instance.LoadState();
            });
            button.IsEnabled = true;
        }
    }
}
