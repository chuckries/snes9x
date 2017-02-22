using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9xWRC;
using System;
using System.IO;
using System.Numerics;
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
        private Emulator Emulator;

        private CanvasBitmap _bitmap;
        private CanvasBitmap _hqxLut;

        private PixelShaderEffect _xbr;
        private PixelShaderEffect _hqx;

        public bool UseFilter { get; set; }

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
            _hqxLut = CanvasBitmap.CreateFromBytes(sender, File.ReadAllBytes("hq4x.data"), 256, 256, Windows.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized, 96.0f, CanvasAlphaMode.Ignore);
            _xbr = new PixelShaderEffect(File.ReadAllBytes("5xbr.bin"))
            {
                Source1 = _bitmap,
                Source1Interpolation = CanvasImageInterpolation.NearestNeighbor,
            };

            _hqx = new PixelShaderEffect(File.ReadAllBytes("SinglePass.bin"))
            {
                Source1 = _bitmap,
                Source1Interpolation = CanvasImageInterpolation.NearestNeighbor,

                Source2 = _hqxLut,
                Source2Interpolation = CanvasImageInterpolation.NearestNeighbor,
            };

            _hqx.Properties["scale"] = 4.0f;
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            Emulator.MainLoop();
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            Surface surface = Emulator.Renderer.GetRenderedSurface();
            if (_bitmap.SizeInPixels.Width != surface.Width || _bitmap.SizeInPixels.Height != surface.Height)
            {
                _bitmap = CanvasBitmap.CreateFromBytes(sender, surface.Bytes, surface.Width, surface.Height, _bitmap.Format);
                _xbr.Source1 = _bitmap;
                _xbr.Properties["textureSize"] = _bitmap.Size.ToVector2();

                _hqx.Source1 = _bitmap;
                _hqx.Properties["textureSize"] = _bitmap.Size.ToVector2();
            }
            else
            {
                _bitmap.SetPixelBytes(surface.Bytes);
            }

            ICanvasImage imageToRender = _bitmap;
            Rect rect = new Rect(0, 0, _bitmap.Size.Width, _bitmap.Size.Height);
            if (UseFilter)
            {
                imageToRender = _hqx;
                rect.Width *= 4.0;
                rect.Height *= 4.0;
            }


            args.DrawingSession.DrawImage(imageToRender, new Rect(new Point(0.0f, 0.0f), sender.Size), rect, 1.0f, CanvasImageInterpolation.NearestNeighbor);
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
            string fileName = $"{Emulator.Rom.Name}-{DateTime.Now.ToString("yyyy-MM-ddHHmmss")}.bmp";

            await canvas.RunOnGameLoopThreadAsync(() => {
                Task.Run(async () =>
                {
                    var file = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        await _bitmap.SaveAsync(fileStream, CanvasBitmapFileFormat.Bmp, 1.0f);
                    }
                }).Wait();
            });
        }
    }
}
