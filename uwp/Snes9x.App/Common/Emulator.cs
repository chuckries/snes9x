using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snes9x.Core;
using Windows.Storage;
using System.IO;
using Windows.Foundation;
using Snes9x.Data;
using Windows.Storage.Streams;

namespace Snes9x.Common
{
    class Emulator
    {
        private static readonly Engine _engine = Engine.Instance;
        public static readonly Emulator Instance = new Emulator();

        private List<IJoypad> _joypads = new List<IJoypad>();

        public Rom Rom { get; private set; }
        public Surface Surface { get; private set; }

        private Emulator()
        {

        }

        public void Init()
        {
            _engine.Init();
            _joypads.Add(new KeyboardJoypad(1, KeyboardJoypadConfig.Gamepad));
        }

        public async Task LoadRomAsync(Rom rom)
        {
            if (Rom != null)
            {
                SaveSRAM();
            }

            if (rom.File == null)
            {
                if (rom.OneDriveItem == null)
                {
                    throw new Exception();
                }

                StorageFolder romsFolder = await Directories.GetRomsFolderAsync();
                rom.File = await OneDriveRomDataSource.DownloadRomAsync(rom.OneDriveItem, romsFolder);
                if (rom.File == null)
                {
                    throw new Exception();
                }
            }

            byte[] bytes = null;
            IBuffer buffer = await FileIO.ReadBufferAsync(rom.File);
            using (DataReader reader = DataReader.FromBuffer(buffer))
            {
                bytes = new byte[buffer.Length];
                reader.ReadBytes(bytes);
            }

            if (_engine.LoadRomMem(bytes))
            {
                Rom = rom;
                LoadSRAM();
                OnRomLoaded(Rom);
            }
        }

        public void Update()
        {
            _joypads.ForEach(j => j.ReportButtons());
            Surface = _engine.Update();
        }

        public bool SaveState()
        {
            return _engine.SaveState(GetSavePath(".sav"));
        }

        public bool LoadState()
        {
            return _engine.LoadState(GetSavePath(".sav"));
        }

        public bool SaveSRAM()
        {
            return _engine.SaveSRAM(GetSavePath(".srm"));
        }

        private bool LoadSRAM()
        {
            return _engine.LoadSRAM(GetSavePath(".srm"));
        }

        private string GetSavePath(string extension)
        {
            return Path.Combine(Directories.SavesPath, Rom.Name + extension);
        }
        
        public string GetScreenshotPath()
        {
            return Path.Combine(Directories.ScreenshotsPath, Rom.Name + ".bmp");
        }

        public event EventHandler<Rom> RomLoaded;

        protected void OnRomLoaded(Rom romFile)
        {
            RomLoaded?.Invoke(this, romFile);
        }
    }
}
