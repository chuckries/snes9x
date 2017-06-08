using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Snes9x.Core;
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
        object _surfaceLock = new object();
        Surface _surface = null;
        CanvasBitmap _emulatorTexture;

        // Effects
        CropEffect _cropEffect; // crops _snesTexture to the appropriate size
        DpiCompensationEffect _dpiEffect; // no op dpi effect, emulator texture is always in pixels, stop the automatically inserted dpi effect because it does linear interopolation
        Transform2DEffect _scaleEffect; // scales the output to the final size

        public Renderer()
        {
        }

        public void SetSurface(Surface surface)
        {
            lock (_surfaceLock)
            {
                _surface = surface;
            }
        }

        public void CreateResources(ICanvasResourceCreatorWithDpi resourceCreator, CanvasCreateResourcesReason reason)
        {
            int width = 512;
            int height = 448;
            _emulatorTexture = CanvasBitmap.CreateFromBytes(resourceCreator, new byte[width * height * 4], width, height, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96, CanvasAlphaMode.Ignore);

            _dpiEffect = new DpiCompensationEffect()
            {
                Source = _emulatorTexture,
                SourceDpi = new Vector2(resourceCreator.Dpi)
            };

            _cropEffect = new CropEffect()
            {
                Source = _dpiEffect,
                BorderMode = EffectBorderMode.Hard
            };

            _scaleEffect = new Transform2DEffect()
            {
                Source = _cropEffect,
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor
            };
        }

        public void Draw(CanvasDrawingSession ds, Size targetSize)
        {
            lock (_surfaceLock)
            {
                if (_surface != null)
                {
                    _emulatorTexture.SetPixelBytes(_surface.Bytes, 0, 0, _surface.Width, _surface.Height);
                    Size size = new Size(
                        ds.ConvertPixelsToDips(_surface.Width),
                        ds.ConvertPixelsToDips(_surface.Height)
                        );
                    _cropEffect.SourceRectangle = new Rect(new Point(0, 0), size);
                    _scaleEffect.TransformMatrix = GetDisplayTransform(size.ToVector2(), targetSize.ToVector2());

                    ds.DrawImage(_scaleEffect);
                }
            }
        }

        private Matrix3x2 GetDisplayTransform(Vector2 sourceSize, Vector2 destSize, double? aspectRatio = null)
        {
            Vector2 aspectScale = Vector2.One;
            Vector2 aspectSize = sourceSize;
            if (aspectRatio.HasValue)
            {
                double currentRatio = (double)sourceSize.X / (double)sourceSize.Y;
                aspectScale = new Vector2((float)(aspectRatio / currentRatio), 1);
                aspectSize *= aspectScale;
            }

            Vector2 stretchScale = destSize / aspectSize;
            Vector2 offset = Vector2.Zero;

            if (stretchScale.X > stretchScale.Y)
            {
                stretchScale.X = stretchScale.Y;
                offset.X = (destSize.X - aspectSize.X * stretchScale.X) / 2;
            }
            else
            {
                stretchScale.Y = stretchScale.X;
                offset.Y = (destSize.Y - aspectSize.Y * stretchScale.Y) / 2;
            }

            return Matrix3x2.CreateScale(aspectScale * stretchScale) * Matrix3x2.CreateTranslation(offset);
        }
    }
}
