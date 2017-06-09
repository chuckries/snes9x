using Snes9x.Common;
using Snes9x.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace Snes9x.ViewModels
{
    [Flags]
    public enum PauseFlags
    {
        None = 0,
        Empty = 1,
        LoadGame = 2,
        Menu = 4,
        Activate = 8,
    }

    public class KeyMap
    {
        public VirtualKey A { get; set; }
        public VirtualKey B { get; set; }
        public VirtualKey X { get; set; }
        public VirtualKey Y { get; set; }
        public VirtualKey L { get; set; }
        public VirtualKey R { get; set; }
        public VirtualKey Start { get; set; }
        public VirtualKey Select { get; set; }
        public VirtualKey Up { get; set; }
        public VirtualKey Down { get; set; }
        public VirtualKey Left { get; set; }
        public VirtualKey Right { get; set; }

        public static readonly KeyMap Default = new KeyMap
        {
            A = VirtualKey.V,
            B = VirtualKey.C,
            X = VirtualKey.D,
            Y = VirtualKey.X,
            L = VirtualKey.A,
            R = VirtualKey.S,
            Start = VirtualKey.Space,
            Select = VirtualKey.Enter,
            Up = VirtualKey.Up,
            Down = VirtualKey.Down,
            Left = VirtualKey.Left,
            Right = VirtualKey.Right
        };
    }

    public class PadMap
    {
        public GamepadButtons A { get; set; }
        public GamepadButtons B { get; set; }
        public GamepadButtons X { get; set; }
        public GamepadButtons Y { get; set; }
        public GamepadButtons L { get; set; }
        public GamepadButtons R { get; set; }
        public GamepadButtons Start { get; set; }
        public GamepadButtons Select { get; set; }
        public GamepadButtons Up { get; set; }
        public GamepadButtons Down { get; set; }
        public GamepadButtons Left { get; set; }
        public GamepadButtons Right { get; set; }

        public static readonly PadMap Default = new PadMap
        {
            A = GamepadButtons.B,
            B = GamepadButtons.A,
            X = GamepadButtons.Y,
            Y = GamepadButtons.X,
            L = GamepadButtons.LeftShoulder,
            R = GamepadButtons.RightShoulder,
            Start = GamepadButtons.Menu,
            Select = GamepadButtons.View,
            Up = GamepadButtons.DPadUp,
            Down = GamepadButtons.DPadDown,
            Left = GamepadButtons.DPadLeft,
            Right = GamepadButtons.DPadRight
        };
    }

    public class JoyState
    {
        public bool A { get; set; }
        public bool B { get; set; }
        public bool X { get; set; }
        public bool Y { get; set; }
        public bool L { get; set; }
        public bool R { get; set; }
        public bool Start { get; set; }
        public bool Select { get; set; }
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
    }

    public class MainViewModel : BindableBase
    {
        //public MenuViewModel MenuViewModel { get; }
        public Renderer Renderer { get; } = new Renderer();
        public bool IsPaused
        {
            get => !_pause.Equals(PauseFlags.None);
        }

        public bool Turbo { get; private set; }

        public Surface Surface { get; protected set; }

        public double ZoomFactor
        {
            get => Renderer.ZoomFactor;
            set
            {
                if (value != Renderer.ZoomFactor)
                {
                    Renderer.ZoomFactor = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            //MenuViewModel = new MenuViewModel(this);
            _joypad = new CoreJoypad(1);
            //MenuViewModel.IsStretched = true;
            //MenuViewModel.IsAspectPreserved = true;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            await CreateAppDirectories();
            Engine.Instance.Init(_savesFolder);

            _isInitialized = true;
        }

        public async Task LoadGame()
        {
            StorageFile pickedFile = await PickRomAsync();
            if (pickedFile != null)
            {
                StorageFile copiedFiled = await pickedFile.CopyAsync(_romFolder, pickedFile.Name, NameCollisionOption.ReplaceExisting);
                // if the file is a zip, unzip it to temporary storage
                if (copiedFiled.FileType == ".zip")
                {
                    using (Stream fs = await copiedFiled.OpenStreamForReadAsync())
                    using (ZipArchive archive = new ZipArchive(fs))
                    {
                        var romEntry = archive.Entries.FirstOrDefault();
                        if (romEntry != null)
                        {
                            using (var entryStream = romEntry.Open())
                            {
                                var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(romEntry.Name, CreationCollisionOption.ReplaceExisting);
                                using (Stream tempFileStream = await tempFile.OpenStreamForWriteAsync())
                                {
                                    await entryStream.CopyToAsync(tempFileStream);
                                    copiedFiled = tempFile;
                                }
                            }
                        }
                    }
                }
                await LoadRomAsync(copiedFiled);
            }
        }

        public async Task LoadRomAsync(StorageFile file)
        {
            SetPause(PauseFlags.LoadGame);
            await Engine.Instance.LoadRomAsync(file);
            ClearPause(PauseFlags.Empty);
            ClearPause(PauseFlags.LoadGame);
        }

        public void Update()
        {
            PollJoypad();
            ReportButtons();

            int times = Turbo ? 10 : 1;
            for (int i = 0; i < times; i++)
            {
                Surface = Engine.Instance.Update();
            }
        }

        public void SetPause(PauseFlags reason)
        {
            _pause |= reason;
            OnPropertyChanged(nameof(IsPaused));
        }

        public void ClearPause(PauseFlags reason)
        {
            _pause &= ~reason;
            OnPropertyChanged(nameof(IsPaused));
        }

        public bool HandleKeyPress(VirtualKey key, bool state)
        {
            if (key == _keyMap.A)
            {
                _joyState.A = state;
            }
            else if (key == _keyMap.B)
            {
                _joyState.B = state;
            }
            else if (key == _keyMap.X)
            {
                _joyState.X = state;
            }
            else if (key == _keyMap.Y)
            {
                _joyState.Y = state;
            }
            else if (key == _keyMap.L)
            {
                _joyState.L = state;
            }
            else if (key == _keyMap.R)
            {
                _joyState.R = state;
            }
            else if (key == _keyMap.Start)
            {
                _joyState.Start = state;
            }
            else if (key == _keyMap.Select)
            {
                _joyState.Select = state;
            }
            else if (key == _keyMap.Up)
            {
                _joyState.Up = state;
            }
            else if (key == _keyMap.Down)
            {
                _joyState.Down = state;
            }
            else if (key == _keyMap.Left)
            {
                _joyState.Left = state;
            }
            else if (key == _keyMap.Right)
            {
                _joyState.Right = state;
            }
            else if (key == VirtualKey.T)
            {
                Turbo = state;
            }
            else
            {
                return false;
            }

            return true;
        }

        private void PollJoypad()
        {
            if (Gamepad.Gamepads.Count > 0)
            {
                GamepadReading reading = Gamepad.Gamepads[0].GetCurrentReading();

                GamepadButtons previousButtons = _previousReading.Buttons;
                GamepadButtons buttons = reading.Buttons;
                GamepadButtons newButtons = buttons ^ previousButtons;

                if (newButtons.HasFlag(_padMap.A))
                {
                    _joyState.A = buttons.HasFlag(_padMap.A);
                }
                if (newButtons.HasFlag(_padMap.B))
                {
                    _joyState.B = buttons.HasFlag(_padMap.B);
                }
                if (newButtons.HasFlag(_padMap.X))
                {
                    _joyState.X = buttons.HasFlag(_padMap.X);
                }
                if (newButtons.HasFlag(_padMap.Y))
                {
                    _joyState.Y = buttons.HasFlag(_padMap.Y);
                }
                if (newButtons.HasFlag(_padMap.L))
                {
                    _joyState.L = buttons.HasFlag(_padMap.L);
                }
                if (newButtons.HasFlag(_padMap.R))
                {
                    _joyState.R = buttons.HasFlag(_padMap.R);
                }
                if (newButtons.HasFlag(_padMap.Start))
                {
                    _joyState.Start = buttons.HasFlag(_padMap.Start);
                }
                if (newButtons.HasFlag(_padMap.Select))
                {
                    _joyState.Select = buttons.HasFlag(_padMap.Select);
                }
                if (newButtons.HasFlag(_padMap.Up))
                {
                    _joyState.Up = buttons.HasFlag(_padMap.Up);
                }
                if (newButtons.HasFlag(_padMap.Down))
                {
                    _joyState.Down = buttons.HasFlag(_padMap.Down);
                }
                if (newButtons.HasFlag(_padMap.Left))
                {
                    _joyState.Left = buttons.HasFlag(_padMap.Left);
                }
                if (newButtons.HasFlag(_padMap.Right))
                {
                    _joyState.Right = buttons.HasFlag(_padMap.Right);
                }

                _previousReading = reading;
            }
        }

        private void ReportButtons()
        {
            _joypad.ReportA(_joyState.A);
            _joypad.ReportB(_joyState.B);
            _joypad.ReportX(_joyState.X);
            _joypad.ReportY(_joyState.Y);
            _joypad.ReportL(_joyState.L);
            _joypad.ReportR(_joyState.R);
            _joypad.ReportStart(_joyState.Start);
            _joypad.ReportSelect(_joyState.Select);
            _joypad.ReportUp(_joyState.Up);
            _joypad.ReportDown(_joyState.Down);
            _joypad.ReportLeft(_joyState.Left);
            _joypad.ReportRight(_joyState.Right);
        }

        private async Task CreateAppDirectories()
        {
            _romFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Roms", CreationCollisionOption.OpenIfExists);
            _savesFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);
        }

        private async Task<StorageFile> PickRomAsync()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".smc");
            picker.FileTypeFilter.Add(".sfc");
            picker.FileTypeFilter.Add(".zip");

            return await picker.PickSingleFileAsync();
        }

        private bool _isInitialized = false;

        // Directories
        private StorageFolder _romFolder;
        private StorageFolder _savesFolder;

        private PauseFlags _pause = PauseFlags.Empty;

        private JoyState _joyState = new JoyState();
        private KeyMap _keyMap = KeyMap.Default;
        private PadMap _padMap = PadMap.Default;
        private CoreJoypad _joypad;

        GamepadReading _previousReading;

        private string _currentRomName = null;
    }
}
