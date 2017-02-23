using Snes9x.Common;
using Snes9x.Data;
using Snes9xCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Snes9x
{
    public sealed partial class MainPage : Page
    {
        internal ObservableCollection<IRomFile> RecentFiles { get; private set; }

        public static MainPage Current;

        public MainPage()
        {
            Current = this;

            this.InitializeComponent();
            RecentFiles = new ObservableCollection<IRomFile>();

            Loaded += MainPage_Loaded;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                if (RootFrame.CanGoBack)
                {
                    RootFrame.GoBack();
                    e.Handled = true;
                }
            };
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(typeof(RomExplorerPage));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var recentFiles = await RomProvider.Instance.GetRecentRomsAsync();
            foreach (var file in recentFiles)
            {
                RecentFiles.Add(file);
            }
        }

        private async Task<bool> LoadRom(IRomFile rom)
        {
            //Frame.Navigate(typeof(EmulatorPage), rom);
            //return await emulator.LoadRomAsync(rom);
            bool success =  await Emulator.Instance.LoadRomAsync(rom);
            if (success)
            {
                RootSplitView.IsPaneOpen = false;
            }
            return success;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            IRomFile rom = e.ClickedItem as IRomFile;
            if (rom != null)
            {
                await LoadRom(rom);
            }
        }

        private void SaveStateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".sfc");
            picker.FileTypeFilter.Add(".smc");
            picker.FileTypeFilter.Add(".zip");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.ViewMode = PickerViewMode.List;

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                if (await LoadRom(new StorageFileRom(file)))
                {
                    RomProvider.Instance.AddRecentRom(file);
                }
            }
        }

        private void ListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            RecentFilesFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
        }
    }
}
