using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Snes9xCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Snes9x.Common
{
    class Renderer
    {
        Surface _surface = null;
        CanvasBitmap _emulatorTexture;

        // Effects
        CropEffect _cropEffect; // crops _snesTexture to the appropriate size
        DpiCompensationEffect _dpiEffect; // no op dpi effect, emulator texture is always in pixels, stop the automatically inserted dpi effect because it does linear interopolation
        Transform2DEffect _scaleEffect; // scales the output to the final size

        public Renderer()
        {
        }

        public void CreateResources(ICanvasResourceCreatorWithDpi resourceCreator, CanvasCreateResourcesReason reason)
        {
            int width = 512;
            int height = 438;
            _emulatorTexture = CanvasBitmap.CreateFromBytes(resourceCreator, new byte[width * height * 4], width, height, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96, CanvasAlphaMode.Ignore);

            _dpiEffect = new DpiCompensationEffect()
            {
                Source = _emulatorTexture,
                SourceDpi = new Vector2(resourceCreator.Dpi)
            };

            _cropEffect = new CropEffect()
            {
                Source = _dpiEffect
            };

            _scaleEffect = new Transform2DEffect()
            {
                Source = _cropEffect,
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor
            };
        }

        public void Draw(CanvasDrawingSession ds, Size targetSize)
        {
            _surface = Emulator.Instance.Surface;
            if (_surface != null)
            {
                _emulatorTexture.SetPixelBytes(_surface.Bytes, 0, 0, _surface.Width, _surface.Height);
                Size size = new Size(ds.ConvertPixelsToDips(_surface.Width), ds.ConvertPixelsToDips(_surface.Height));
                _cropEffect.SourceRectangle = new Rect(new Point(0, 0), size);
                _scaleEffect.TransformMatrix = GetDisplayTransform(size.ToVector2(), targetSize.ToVector2());

                ds.DrawImage(_scaleEffect);
            }
        }

        private Matrix3x2 GetDisplayTransform(Vector2 sourceSize, Vector2 destSize)
        {
            Vector2 scale = destSize / sourceSize;
            Vector2 offset = Vector2.Zero;

            if (scale.X > scale.Y)
            {
                scale.X = scale.Y;
                offset.X = (destSize.X - sourceSize.X * scale.X) / 2;
            }
            else
            {
                scale.Y = scale.X;
                offset.Y = (destSize.Y - sourceSize.Y * scale.Y) / 2;
            }

            return Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation(offset);
        }

        public async Task SaveScreenshotAsync(string fileName)
        {
            using (CanvasRenderTarget renderTarget = new CanvasRenderTarget(
                _emulatorTexture,
                800,
                700,
                _emulatorTexture.Dpi,
                _emulatorTexture.Format,
                _emulatorTexture.AlphaMode
                ))
            {
                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.DrawImage(
                        _emulatorTexture,
                        new Rect(new Point(0, 0), renderTarget.Size),
                        new Rect(new Point(0, 0), new Size((float)_surface.Width, (float)_surface.Height)),
                        1,
                        CanvasImageInterpolation.NearestNeighbor
                        );
                }

                await renderTarget.SaveAsync(fileName, CanvasBitmapFileFormat.Bmp, 1.0f);
            }
        }
    }
}
