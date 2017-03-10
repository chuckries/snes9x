using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Snes9x.Common
{
    public static class Directories
    {
        private enum FolderType
        {
            Saves,
            Screenshots,
            Roms
        }

        public static string SavesPath
        {
            get
            {
                return Path.Combine(
                    ApplicationData.Current.RoamingFolder.Path,
                    FolderType.Saves.ToString()
                    );
            }
        }

        public static string ScreenshotsPath
        {
            get
            {
                return Path.Combine(
                    ApplicationData.Current.RoamingFolder.Path,
                    FolderType.Screenshots.ToString()
                    );
            }
        }

        public static string RomsPath
        {
            get
            {
                return Path.Combine(
                    ApplicationData.Current.RoamingFolder.Path,
                    FolderType.Roms.ToString()
                    );
            }
        }

        public static async Task<StorageFolder> GetSavesFolderAsync()
        {
            return await GetAppDirectory(
                ApplicationData.Current.RoamingFolder,
                FolderType.Saves.ToString()
                );
        }

        public static async Task<StorageFolder> GetScreenshotsFolderAync()
        {
            return await GetAppDirectory(
                ApplicationData.Current.RoamingFolder,
                FolderType.Screenshots.ToString()
                );
        }

        public static async Task<StorageFolder> GetRomsFolderAsync()
        {
            return await GetAppDirectory(
                ApplicationData.Current.LocalFolder,
                FolderType.Roms.ToString()
                );
        }

        private static async Task<StorageFolder> GetAppDirectory(StorageFolder parent, string name)
        {
            return await parent.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
        }
    }
}
