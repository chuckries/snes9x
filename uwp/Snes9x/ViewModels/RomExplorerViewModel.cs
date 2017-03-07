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
        public ObservableCollection<RomFile> Roms { get; } = new ObservableCollection<RomFile>();

        public async Task PopulateRoms()
        {
            Roms.Clear();
            await RomProvider.Instance.GetRecentRomsAsync(Roms);
        }
    }
}
