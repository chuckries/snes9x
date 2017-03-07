using Microsoft.OneDrive.Sdk;
using Snes9x.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Snes9x.Data
{
    public class RomProvider
    {
        private static RomProvider s_Instance;
        public static RomProvider Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new RomProvider();
                }
                return s_Instance;
            }
        }

        protected RomProvider()
        {

        }

        public async Task<List<RomFile>> GetRecentRomsAsync()
        {
            var mruList = StorageApplicationPermissions.MostRecentlyUsedList;

            List<RomFile> recentRoms = new List<RomFile>(mruList.Entries.Count);

            foreach (var entry in mruList.Entries)
            {
                if (entry.Metadata == "romfile")
                {
                    // insert at the beginning of the list
                    recentRoms.Insert(0, new StorageFileRom(await mruList.GetFileAsync(entry.Token)));
                }
            }

            return recentRoms;
        }

        public async Task GetRecentRomsAsync(IList<RomFile> roms)
        {
            var mruList = StorageApplicationPermissions.MostRecentlyUsedList;

            foreach (var entry in mruList.Entries)
            {
                if (entry.Metadata == "romfile")
                {
                    // insert at the beginning of the list
                    StorageFile file = await mruList.GetFileAsync(entry.Token);
                    roms.Insert(0, new StorageFileRom(file));
                }
            }
        }

        public async Task GetOneDriveRomsAsync()
        {
            var msaAuthenticationProvider = new OnlineIdAuthenticationProvider(new[] { "onedrive.readonly", "onedrive.appfolder" });
            await msaAuthenticationProvider.AuthenticateUserAsync();
            var client = new OneDriveClient(msaAuthenticationProvider);
            var items = await client.Drive.Special.AppRoot.Children.Request().GetAsync();
        }

        public async Task<IEnumerable<IGrouping<string, RomFile>>> GetGroupedRomsAsync()
        {
            var recentRoms = await GetRecentRomsAsync();
            return new[] { new ListGrouping<string, RomFile>("Recent ROMs", recentRoms) };
        }

        public void AddRecentRom(StorageFile file)
        {
            var mruList = StorageApplicationPermissions.MostRecentlyUsedList;
            mruList.Add(file, "romfile");
        }

        public class ListGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
        {
            public ListGrouping(TKey key, IEnumerable<TElement> elements) : base(elements)
            {
                Key = key;
            }
            
            public TKey Key { get; protected set; }
        }
    }
}
