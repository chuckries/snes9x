using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snes9xCore;
using Windows.Storage;
using System.IO;
using Windows.Foundation;

namespace Snes9x.Common
{
    class Emulator
    {
        private static readonly CoreEmulator _coreEmulator = CoreEmulator.Instance;
        public static readonly Emulator Instance = new Emulator();
        public readonly EmulatorDirectories Directories = new EmulatorDirectories();

        private List<IJoypad> _joypads = new List<IJoypad>();

        public IRomFile Rom { get; private set; }
        public Surface Surface { get; private set; }

        private Emulator()
        {

        }

        public async Task Init()
        {
            _coreEmulator.Init();
            _joypads.Add(new KeyboardJoypad(1));
            await Directories.Init();
        }

        public async Task<bool> LoadRomAsync(IRomFile file)
        {
            if (Rom != null)
            {
                SaveSRAM();
            }

            byte[] bytes = await file.GetBytesAsync();
            if (_coreEmulator.LoadRomMem(bytes))
            {
                Rom = file;
                LoadSRAM();
                OnRomLoaded(Rom);
                return true;
            }
            return false;
        }

        public void Update()
        {
            _joypads.ForEach(j => j.ReportButtons());
            Surface = _coreEmulator.Update();
        }

        public bool SaveState()
        {
            return _coreEmulator.SaveState(GetSavePath(".sav"));
        }

        public bool LoadState()
        {
            return _coreEmulator.LoadState(GetSavePath(".sav"));
        }

        public bool SaveSRAM()
        {
            return _coreEmulator.SaveSRAM(GetSavePath(".srm"));
        }

        private bool LoadSRAM()
        {
            return _coreEmulator.LoadSRAM(GetSavePath(".srm"));
        }

        private string GetSavePath(string extension)
        {
            return Path.Combine(Directories.SavesFolder.Path, Rom.Name + extension);
        }

        public event EventHandler<IRomFile> RomLoaded;

        protected void OnRomLoaded(IRomFile romFile)
        {
            RomLoaded?.Invoke(this, romFile);
        }
    }
}
