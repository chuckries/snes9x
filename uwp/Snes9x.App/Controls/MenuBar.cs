using Snes9x.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Snes9x
{
    public class MenuBar : CommandBar
    {
        public MainViewModel ViewModel { get => AppShell.Current.ViewModel; }

        public MenuBar()
        {
            Opening += MenuBar_Opening;
            Opened += MenuBar_Opened;
            Closing += MenuBar_Closing;
            Closed += MenuBar_Closed;
        }

        private void MenuBar_Opening(object sender, object e)
        {
            ((CommandBar)sender).OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Collapsed;
            ViewModel.SetPause(PauseFlags.Menu);
            _listenForKeys = true;
        }

        private void MenuBar_Opened(object sender, object e)
        {
        }

        private void MenuBar_Closing(object sender, object e)
        {
        }

        private void MenuBar_Closed(object sender, object e)
        {
            ((CommandBar)sender).OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Visible;
            ViewModel.ClearPause(PauseFlags.Menu);
            _listenForKeys = false;
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (_listenForKeys)
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (_listenForKeys)
            {
                base.OnKeyUp(e);
            }
        }

        private bool _listenForKeys = true;
    }
}
