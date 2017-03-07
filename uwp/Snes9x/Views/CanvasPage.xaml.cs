using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9x.Common;
using Snes9x.Data;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Snes9x
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmulatorPage : Page, INotifyPropertyChanged
    {
        bool _isFastForward = false;
        int _fastForwardFrames = 5;

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
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;
            };

            Loaded += (s, e) =>
            {
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            };

            ScreenshotButton.Click += (s, e) =>
            {
                var task = Canvas.RunOnGameLoopThreadAsync(() =>
                {
                    var screenshotTask = _renderer.SaveScreenshotAsync(Emulator.Instance.GetScreenshotPath());
                });
            };
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.T)
            {
                _isFastForward = false;
                args.Handled = true;
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.T)
            {
                _isFastForward = true;
                args.Handled = true;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            OnActivity();

            EmulatorIsPaused = Emulator.Instance.Rom == null;
            RomFile romFile = e.Parameter as RomFile;
            if (romFile != null && Emulator.Instance.Rom?.Name.CompareTo(romFile.Name) != 0)
            {
                bool success = await Emulator.Instance.LoadRomAsync(romFile);
            }

            base.OnNavigatedTo(e);
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
                var screenshotTask = _renderer.SaveScreenshotAsync(Emulator.Instance.GetScreenshotPath());
            });
        }

        private void Emulator_RomLoaded(object sender, RomFile e)
        {
            EmulatorIsPaused = false;
            
        }

        private void Canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _renderer.CreateResources(sender, args.Reason);
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            int i = 0;
            do
            {
                Emulator.Instance.Update();
                i++;
            } while (_isFastForward && i < _fastForwardFrames);
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
