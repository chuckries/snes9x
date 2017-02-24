﻿using Snes9x.Common;
using Snes9x.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Snes9x
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RomExplorerPage : Page
    {
        public RomExplorerPage()
        {
            this.InitializeComponent();
            Loaded += RomExplorerPage_Loaded;
        }

        private async void RomExplorerPage_Loaded(object sender, RoutedEventArgs e)
        {
            RecentRomsCollectionViewSource.Source = await RomProvider.Instance.GetGroupedRomsAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(EmulatorPage), e.ClickedItem, new DrillInNavigationTransitionInfo());
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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
                Frame.Navigate(typeof(EmulatorPage), new StorageFileRom(file), new DrillInNavigationTransitionInfo());
                RomProvider.Instance.AddRecentRom(file);
            }
        }
    }
}