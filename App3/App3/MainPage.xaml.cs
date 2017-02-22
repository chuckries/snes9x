using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }

    public class ChessGrid : Panel
    {
        int _rows = 3;
        int _columns = 3;

        protected override Size MeasureOverride(Size avilableSize)
        {
            return avilableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size childSize = new Size(finalSize.Width / _columns, finalSize.Height / _rows);

            double num = finalSize.Width - 1;

            Rect childRect = new Rect(0, 0, childSize.Width, childSize.Height);
            foreach (var child in Children)
            {
                child.Arrange(new Rect(Math.Round(childRect.X), Math.Round(childRect.Y), Math.Round(childRect.Width), Math.Round(childRect.Height)));
                childRect.X += childSize.Width;
                if (childRect.X >= num)
                {
                    childRect.X = 0;
                    childRect.Y += childRect.Height;
                }
            }
            return finalSize;
        }
    }

}
