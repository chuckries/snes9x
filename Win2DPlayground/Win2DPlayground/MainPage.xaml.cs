using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2DPlayground
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Circle> _circles = new List<Circle>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {

        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            lock (_circles)
            {
                _circles.ForEach(c => c.Update(sender.Size));
            }
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            lock (_circles)
            {
                _circles.ForEach(c => c.Draw(args.DrawingSession));
            }
        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var position = e.GetCurrentPoint(canvas).Position;

            lock(_circles)
            {
                _circles.Add(new Circle()
                {
                    Position = position.ToVector2(),
                    Radius = 25.0f,
                    Color = Colors.Blue
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    class Circle
    {
        static Vector2 GRAVITY = new Vector2(0.0f, 0.5f);

        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
        public Vector2 Velocity { get; private set; }

        public void Update(Size size)
        {
            Vector2 newPosition = GRAVITY + Velocity + Position;
            Velocity = newPosition - Position;
            Position = newPosition;

            float bottom = size.ToVector2().Y - Radius;
            if (Position.Y > bottom)
            {
                Position = new Vector2(Position.X, bottom);
                Velocity *= new Vector2(1.0f, -0.95f);
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            ds.DrawCircle(Position, Radius, Colors.Black);
            ds.FillCircle(Position, Radius, Color);
        }
    }
}
