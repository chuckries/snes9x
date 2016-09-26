using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snes9xCore;
using Windows.Storage;

namespace Snes9x.Common
{
    class Emulator
    {
        private static readonly CoreEmulator _coreEmulator = CoreEmulator.Instance;
        public static readonly Emulator Instance = new Emulator();

        public readonly EmulatorDirectories Directories = new EmulatorDirectories();

        public IRomFile Rom { get; private set; }
        public Surface Screen {
            get
            {
                return _coreEmulator.GetRenderedSurface();
            }
        }

        private Emulator()
        {

        }

        public async Task Init()
        {
            _coreEmulator.Init();
            await Directories.Init();
        }

        public async Task LoadRomAsync(IRomFile file)
        {
            byte[] bytes = await file.GetBytesAsync();
            if (_coreEmulator.LoadRomMem(bytes))
            {
                Rom = file;
            }
        }

        public void Update()
        {
            _coreEmulator.MainLoop();
        }

        public bool SaveState()
        {
            return _coreEmulator.SaveState(GetSavePath());
        }

        public bool LoadState()
        {
            return _coreEmulator.LoadState(GetSavePath());
        }

        private string GetSavePath()
        {
            return System.IO.Path.Combine(Directories.SavesFolder.Path, Rom.Name + ".sav");
        }
    }
}
