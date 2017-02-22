using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            hamburgerMenu.ItemsSource = MenuItem.GetMenuItems();
            hamburgerMenu.OptionsItemsSource = MenuItem.GetOptionItems();
        }

        private void hamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            MenuItem item = e.ClickedItem as MenuItem;

            myTextBlock.Text = item.Text;
            hamburgerMenu.IsPaneOpen = false;
        }
    }

    class MenuItem
    {
        public string Text { get; set; }

        public static List<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>()
            {
                new MenuItem { Text = "Menu Item 1" },
                new MenuItem { Text = "Menu Item 2" }
            };
        }

        public static List<MenuItem> GetOptionItems()
        {
            return new List<MenuItem>()
            {
                new MenuItem { Text = "Option Item 1" },
                new MenuItem { Text = "Option Item 2" }
            };
        }
    }
}
