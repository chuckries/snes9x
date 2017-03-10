using Snes9x.Common;
using Snes9x.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snes9x.ViewModels
{
    public class RomExplorerViewModel : BindableBase
    {
        public ObservableCollection<Rom> Roms { get; } = new ObservableCollection<Rom>();

        public ObservableCollection<Rom> OneDriveRoms { get; } = new ObservableCollection<Rom>();

        public async Task PopulateRoms()
        {
            Roms.Clear();
            await RomProvider.Instance.GetRecentRomsAsync(Roms);
            var oneDriveRoms = await OneDriveRomDataSource.GetAsync();
            OneDriveRoms.Clear();
            foreach(var rom in oneDriveRoms)
            {
                OneDriveRoms.Add(rom);
            }
        }
    }
}
