﻿using Snes9x.Common;
using Snes9xCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Snes9x
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Windows.Storage.AccessCache.StorageItemMostRecentlyUsedList _mruList = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;

        internal ObservableCollection<IRomFile> RecentFiles { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            RecentFiles = new ObservableCollection<IRomFile>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            foreach (var entry in _mruList.Entries)
            {
                if (entry.Metadata == "romfile")
                {
                    RecentFiles.Insert(0, new StorageFileRom(await _mruList.GetFileAsync(entry.Token)));
                }
            }
        }

        private async void LoadRomButton_Click(object sender, RoutedEventArgs e)
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
                    _mruList.Add(file, "romfile");
                }
            }
        }

        private async Task<bool> LoadRom(IRomFile rom)
        {
            //Frame.Navigate(typeof(EmulatorPage), rom);
            return await emulator.LoadRomAsync(rom);
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

        private async void LoadStateButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            await emulator.Screenshot();
        }
    }
}
