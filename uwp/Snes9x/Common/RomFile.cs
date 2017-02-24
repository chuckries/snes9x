using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snes9x.Common
{
    public abstract class RomFile
    {
        public string Name { get; protected set; }
        public string FileName { get; protected set; }
        public string Path { get; protected set; }
        public string ScreenshotPath { get; protected set; } = "ms-appdata:///roaming/screenshots/zelda.bmp";

        public abstract Task<byte[]> GetBytesAsync();
    }
}
