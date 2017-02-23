﻿using Snes9x.Common;
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

        public async Task<List<IRomFile>> GetRecentRomsAsync()
        {
            var mruList = StorageApplicationPermissions.MostRecentlyUsedList;

            List<IRomFile> recentRoms = new List<IRomFile>(mruList.Entries.Count);

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

        public async Task<IEnumerable<IGrouping<string, IRomFile>>> GetGroupedRomsAsync()
        {
            var recentRoms = await GetRecentRomsAsync();
            return new[] { new ListGrouping<string, IRomFile>("Recent ROMs", recentRoms) };
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
