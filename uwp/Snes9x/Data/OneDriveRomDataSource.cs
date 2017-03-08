using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var items = await client.Drive.Special.AppRoot.Children.Request().GetAsync();
            List<RomFile> romFiles = new List<RomFile>(items.Count);
            foreach (var item in items)
            {
                romFiles.Add(new RomFile { Name = item.Name });
            }

            while (items.NextPageRequest != null)
            {
                items = await items.NextPageRequest.GetAsync();
                foreach (var item in items)
                {
                    romFiles.Add(new RomFile { Name = item.Name });
                }
            }

            return romFiles;
        }
    }
}
