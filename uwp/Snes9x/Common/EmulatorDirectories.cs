using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Snes9x.Common
{
    internal class EmulatorDirectories
    {
        public StorageFolder SavesFolder { get; private set; }
        public StorageFolder ScreenshotsFolder { get; private set; }

        public EmulatorDirectories()
        {
        }

        public async Task Init()
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;

            SavesFolder = await CreateAppDirectory(roamingFolder, "saves");
            ScreenshotsFolder = await CreateAppDirectory(roamingFolder, "screenshots");
        }

        private async Task<StorageFolder> CreateAppDirectory(StorageFolder parent, string name)
        {
            return await parent.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
        }
    }
}
