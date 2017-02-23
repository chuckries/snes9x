using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9x.Common;
using Snes9x.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class EmulatorPage : Page, INotifyPropertyChanged
    {
        public bool _emulatorIsPaused;
        public bool EmulatorIsPaused
        {
            get { return _emulatorIsPaused; }
            set
            {
                _emulatorIsPaused = value;
                OnPropertyChanged();
            }
        }

        private Renderer _renderer = new Renderer();
        private DispatcherTimer _idleTimer = new DispatcherTimer();

        private bool _isMenuActive;

        public bool IsMenuActive
        {
            get { return _isMenuActive; }
            set
            {
                _isMenuActive = value;
                UpdateVisualState(true);
            }
        }

        public EmulatorPage()
        {
            InitializeComponent();
            IsMenuActive = true;
            EmulatorIsPaused = true;
            Emulator.Instance.RomLoaded += Emulator_RomLoaded;
            _idleTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
            _idleTimer.Tick += _idleTimer_Tick;
            PointerMoved += (s, e) =>
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                {
                    OnActivity();
                }
            };
            Tapped += (s, e) =>
            {
                if (IsMenuActive)
                {
                    IsMenuActive = false;
                }
                else
                {
                    OnActivity();
                }
            };

            MenuButton.Click += (s, e) => { MainPage.Current.RootSplitView.IsPaneOpen = true; };
            SaveButton.Click += (s, e) =>
            {
                var task = Canvas.RunOnGameLoopThreadAsync(() =>
                {
                    Emulator.Instance.SaveState();
                });
            };
            LoadButton.Click += (s, e) =>
            {
                var task = Canvas.RunOnGameLoopThreadAsync(() =>
                {
                    Emulator.Instance.LoadState();
                });
            };

            Unloaded += (s, e) =>
            {
                Canvas.RemoveFromVisualTree();
                Canvas = null;
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            OnActivity();

            Canvas.Paused = false;
            IRomFile romFile = e.Parameter as IRomFile;
            if (romFile != null)
            {
                bool success = await Emulator.Instance.LoadRomAsync(romFile);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _idleTimer.Stop();
            IsMenuActive = false;
            SetPointerVisibility(true);
            var task = Canvas.RunOnGameLoopThreadAsync(() =>
            {
                Emulator.Instance.SaveSRAM();
            });
        }

        private void Emulator_RomLoaded(object sender, IRomFile e)
        {
            EmulatorIsPaused = false;
            
        }

        private void Canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _renderer.CreateResources(sender, args.Reason);
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            Emulator.Instance.Update();
        }

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            _renderer.Draw(args.DrawingSession, sender.Size);
        }

        private void OnActivity()
        {
            IsMenuActive = true;
            _idleTimer.Start();
        }

        private void _idleTimer_Tick(object sender, object e)
        {
            _idleTimer.Stop();
            IsMenuActive = false;
        }

        private void UpdateVisualState(bool useTransitions)
        {
            if (IsMenuActive)
            {
                VisualStateManager.GoToState(this, "Active", useTransitions);
                SetPointerVisibility(true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Idle", useTransitions);
                if (!MainPage.Current.RootSplitView.IsPaneOpen)
                {
                    SetPointerVisibility(false);
                }
            }
        }

        private void SetPointerVisibility(bool isPointerVisible)
        {
            CoreCursor cursor = isPointerVisible ? new CoreCursor(CoreCursorType.Arrow, 0) : null;
            Window.Current.CoreWindow.PointerCursor = cursor;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
