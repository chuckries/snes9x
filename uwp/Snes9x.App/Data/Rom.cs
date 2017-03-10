using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Snes9x.Data
{
    public class Rom
    {
        //public string Name { get; set; }
        //public string FileName { get; protected set; }
        //public string Path { get; protected set; }


        //public virtual Task<byte[]> GetBytesAsync()
        //{
        //    return null;
        //}

        public StorageFile File { get; set; }

        public Microsoft.OneDrive.Sdk.Item OneDriveItem { get; set; }

        public string Name
        {
            get
            {
                return File?.Name ?? OneDriveItem?.Name ?? "???";
            }
        }

        public string ScreenshotPath
        {
            get
            {
                return $"ms-appdata:///roaming/Screenshots/{Name}.bmp";
            }
        }
    }
}
