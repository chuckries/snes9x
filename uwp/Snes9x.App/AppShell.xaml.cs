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

            Loaded += AppShell_Loaded;
            Unloaded += AppShell_Unloaded;
        }

        private async void AppShell_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.Activated += CoreWindow_Activated;

            await ViewModel.InitializeAsync();
        }

        private void AppShell_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void CoreWindow_Activated(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                ViewModel.SetPause(PauseFlags.Activate);
            }
            else
            {
                ViewModel.ClearPause(PauseFlags.Activate);
            }
        }

        private async void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadGame();
            canvas.Focus(FocusState.Programmatic);
        }

        private void CanvasAnimatedControl_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _renderer.CreateResources(sender, args.Reason);
        }

        private void CanvasAnimatedControl_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            ViewModel.Update();
            _renderer.SetSurface(ViewModel.Surface);
        }

        private void CanvasAnimatedControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            _renderer.Draw(args.DrawingSession, sender.Size);
        }

        private Renderer _renderer = new Renderer();

        private void canvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!e.KeyStatus.WasKeyDown)
            {
                e.Handled = ViewModel.HandleKeyPress(e.OriginalKey, true);
            }
        }

        private void canvas_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.KeyStatus.IsKeyReleased)
            {
                e.Handled = ViewModel.HandleKeyPress(e.OriginalKey, false);
            }
        }
    }
}
