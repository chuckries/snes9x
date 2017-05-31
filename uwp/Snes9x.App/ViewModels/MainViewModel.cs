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

namespace Snes9x.ViewModels
{
    [Flags]
    public enum PauseFlags
    {
        None = 0,
        Empty = 1,
        LoadGame = 2,
        Menu = 4,
    }
    public class MainViewModel : BindableBase
    {
        public bool IsPaused
        {
            get => !_pause.Equals(PauseFlags.None);
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            Engine.Instance.Init();

            await CreateAppDirectories();
            _isInitialized = true;
        }

        public async Task LoadGame()
        {
            SetPause(PauseFlags.LoadGame);
            StorageFile pickedFile = await PickRomAsync();
            try
            {
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
            finally
            {
                ClearPause(PauseFlags.LoadGame);
            }
        }

        public async Task LoadRomAsync(StorageFile file)
        {
            await Task.Run(() => Engine.Instance.LoadRom(file.Path));
            ClearPause(PauseFlags.Empty);
        }

        private void foo()
        {
            int i = 0;
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
    }
}
