using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Snes9x.Data
{
    public static class OneDriveRomDataSource
    {
        static OneDriveClient s_Client;

        private static async Task<OneDriveClient> GetClientAsync()
        {
            if (s_Client == null)
            {
                var msaAuthenticationProvider = new OnlineIdAuthenticationProvider(new[] { "onedrive.readonly", "onedrive.appfolder" });
                await msaAuthenticationProvider.RestoreMostRecentFromCacheOrAuthenticateUserAsync();
                s_Client = new OneDriveClient(msaAuthenticationProvider);
            }

            return s_Client;
        }

        public static async Task<IEnumerable<RomFile>> GetAsync()
        {
            OneDriveClient client = await GetClientAsync();

            IItemChildrenCollectionPage items = await client.Drive.Special.AppRoot.Children.Request().GetAsync();
            List<RomFile> romFiles = new List<RomFile>(items.Count);
            foreach (Item item in items)
            {
                romFiles.Add(new RomFile { Name = item.Name });
            }

            while (items.NextPageRequest != null)
            {
                items = await items.NextPageRequest.GetAsync();
                foreach (Item item in items)
                {
                    romFiles.Add(new RomFile { Name = item.Name });
                }
            }

            return romFiles;
        }

        public static async Task<Stream> GetSreamForItemAsync(Item item)
        {
            OneDriveClient client = await GetClientAsync();

            Stream stream = await client.Drive.Items[item.Id].Content.Request().GetAsync();
            return stream;
        }

        public static async Task<StorageFile> DownloadRomAsync(Item item, StorageFolder folder)
        {
            using (Stream stream = await GetSreamForItemAsync(item))
            {
                StorageFile file = await folder.CreateFileAsync(item.Name, CreationCollisionOption.FailIfExists);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    await stream.CopyToAsync(fileStream);
                    return file;
                }
            }
        }
    }
}
