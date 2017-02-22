using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ShaderTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CanvasRenderTarget _target;

        CanvasBitmap _texture;
        CanvasBitmap _lut;

        PixelShaderEffect _pass1;
        PixelShaderEffect _pass2;

        PixelShaderEffect _singlePass;

        PixelShaderEffect _xbr;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(canvas_CreateResourcesAsync(sender, args).AsAsyncAction());
        }

        private async Task canvas_CreateResourcesAsync(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            var textureFile = await KnownFolders.PicturesLibrary.GetFileAsync("zelda.bmp");
            using (var textureFileStream = await textureFile.OpenReadAsync())
            {
                _texture = await CanvasBitmap.LoadAsync(sender, textureFileStream, sender.Dpi, CanvasAlphaMode.Ignore);
            }

            _lut = CanvasBitmap.CreateFromBytes(sender, File.ReadAllBytes("Shaders/hq4x.data"), 256, 256, Windows.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized, sender.Dpi, CanvasAlphaMode.Ignore);

            _target = new CanvasRenderTarget(sender, _texture.Size);

            _pass1 = new PixelShaderEffect(File.ReadAllBytes("Shaders/Pass1.bin"))
            {
                Source1 = _texture,
                Source1Interpolation = CanvasImageInterpolation.NearestNeighbor,
                Source1Mapping = SamplerCoordinateMapping.Offset,

                MaxSamplerOffset = 1
            };

            _pass2 = new PixelShaderEffect(File.ReadAllBytes("Shaders/Pass2.bin"))
            {
                Source1 = _target,
                Source1Interpolation = CanvasImageInterpolation.NearestNeighbor,

                Source2 = _texture,
                Source2Interpolation = CanvasImageInterpolation.NearestNeighbor,

                Source3 = _lut,
                Source3Interpolation = CanvasImageInterpolation.NearestNeighbor
            };

            _pass2.Properties["scale"] = 4.0f;
            _pass2.Properties["textureSize"] = new Vector2(_texture.SizeInPixels.Width, _texture.SizeInPixels.Height);

            _singlePass = new PixelShaderEffect(File.ReadAllBytes("Shaders/SinglePass.bin"))
            {
                Source1 = _texture,
                Source1Interpolation = CanvasImageInterpolation.NearestNeighbor,

                Source2 = _lut,
                Source2Interpolation = CanvasImageInterpolation.NearestNeighbor,
            };

            _singlePass.Properties["scale"] = 4.0f;
            _singlePass.Properties["textureSize"] = new Vector2(_texture.SizeInPixels.Width, _texture.SizeInPixels.Height);

            _xbr = new PixelShaderEffect(File.ReadAllBytes("Shaders/5xbr.bin"))
            {
                Source1 = _texture,
                Source1Interpolation = CanvasImageInterpolation.NearestNeighbor
            };

            _xbr.Properties["textureSize"] = new Vector2(_texture.SizeInPixels.Width, _texture.SizeInPixels.Height);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {

        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            using (var session = _target.CreateDrawingSession())
            {
                session.DrawImage(_pass1);
            }

            args.DrawingSession.DrawImage(_pass2);
            //args.DrawingSession.DrawImage(_pass2, new Rect(_texture.Size.Width * 4, 0, _texture.Size.Width * 4, _texture.Size.Height * 4), new Rect(0, 0, _texture.Size.Width * 4, _texture.Size.Height * 4));
        }
    }
}
