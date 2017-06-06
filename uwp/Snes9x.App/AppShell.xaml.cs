using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9x.Common;
using Snes9x.Core;
using Snes9x.ViewModels;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Snes9x
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public static AppShell Current;

        public MainViewModel ViewModel { get; } = new MainViewModel();

        public AppShell()
        {
            this.InitializeComponent();
            Current = this;

            Loaded += async (s, e) =>
            {
                await ViewModel.InitializeAsync();
            };

            Unloaded += AppShell_Unloaded;
        }

        private void AppShell_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private async void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadGame();
        }

        private void CanvasAnimatedControl_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _renderer.CreateResources(sender, args.Reason);
        }

        private void CanvasAnimatedControl_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            Surface surface = Engine.Instance.Update();
            _renderer.SetSurface(surface);
        }

        private void CanvasAnimatedControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            _renderer.Draw(args.DrawingSession, sender.Size);
        }

        private Renderer _renderer = new Renderer();
    }
}
