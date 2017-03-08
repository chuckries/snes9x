using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snes9x.Data
{
    public class RomFile
    {
        public string Name { get; set; }
        public string FileName { get; protected set; }
        public string Path { get; protected set; }
        public string ScreenshotPath
        {
            get
            {
                return $"ms-appdata:///roaming/Screenshots/{Name}.bmp";
            }
        }

        public virtual Task<byte[]> GetBytesAsync()
        {
            return null;
        }
    }
}
