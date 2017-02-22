using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Gaming.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            await Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                for (;;)
                {
                    var reading = e.GetCurrentReading();

                    var buttonLabels = new List<string>();
                    if (reading.Buttons.HasFlag(GamepadButtons.A)) buttonLabels.Add("nesB");
                    if (reading.Buttons.HasFlag(GamepadButtons.B)) buttonLabels.Add("nesA");
                    if (reading.Buttons.HasFlag(GamepadButtons.X)) buttonLabels.Add("nesY");
                    if (reading.Buttons.HasFlag(GamepadButtons.Y)) buttonLabels.Add("nesX");

                    buttonLabels.Sort(StringComparer.OrdinalIgnoreCase);

                    var text = "None";
                    if (buttonLabels.Count > 0)
                    {
                        text = string.Join(", ", buttonLabels);
                    }

                    tbButtonLabebl.Text = text;

                    await Task.Yield();
                }
            });
        }
    }
}
