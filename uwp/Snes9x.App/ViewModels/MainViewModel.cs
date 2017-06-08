using Snes9x.Common;
using Snes9x.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public bool IsPaused
        {
            get => !_pause.Equals(PauseFlags.None);
        }

        public Surface Surface { get; protected set; }

        public MainViewModel()
        {
            _joypad = new CoreJoypad(1);
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
            //await Task.Run(() =>
            //{
            //    if (_currentRomName != null)
            //    {
            //        Engine.Instance.SaveSRAM(Path.Combine(_savesFolder.Path, _currentRomName, ".srm"));
            //    }

            //    if (Engine.Instance.LoadRom(file.Path))
            //    {
            //        _currentRomName = file.DisplayName;
            //        string sramPath = Path.Combine(_savesFolder.Path, file.DisplayName, ".srm");
            //        if (File.Exists(sramPath))
            //        {
            //            Engine.Instance.LoadSRAM(sramPath);
            //        }
            //        ClearPause(PauseFlags.Empty);
            //    }
            //});
            ClearPause(PauseFlags.LoadGame);
        }

        public void Update()
        {
            ReportButtons();
            Surface = Engine.Instance.Update();
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
            else
            {
                return false;
            }

            return true;
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
        private CoreJoypad _joypad;

        private string _currentRomName = null;
    }
}
