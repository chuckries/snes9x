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
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Snes9x.Common;
using System.Threading.Tasks;
using Snes9xCore;
using System.Threading;
using Windows.Storage;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Snes9x
{
    public sealed partial class EmulatorControl : UserControl
    {
        Emulator _emulator = Emulator.Instance;
        Renderer _renderer;

        public EmulatorControl()
        {
            this.InitializeComponent();
            _renderer = new Renderer();
        }

        #region Canvas Events
        private void canvas_GameLoopStarting(ICanvasAnimatedControl sender, object args)
        {
            // this is raised on the event thread, install our custom synchronization context
            SynchronizationContext.SetSynchronizationContext(new GameLoopSynchronizationContext(sender));
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(canvas_CreateResourcesAsync(sender, args).AsAsyncAction());
        }

        private async Task canvas_CreateResourcesAsync(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            //await (_renderer.CreateResources(sender, args.Reason));
            canvas.Paused = true;
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (_emulator.Rom != null)
            {
                _emulator.Update();
            }
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            _renderer.Draw(args.DrawingSession, sender.Size);
        }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        public async Task RunOnGameLoopThreadAsync(Func<Task> callback)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            Func<Task> operation = async () =>
            {
                try
                {
                    await callback();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            };

            if (canvas.HasGameLoopThreadAccess)
            {
                await operation();
            }
            else
            {
                await canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await operation();
                });
            }

            await tcs.Task;
        }

        public async Task RunOnGameLoopThreadAsync(Action callback)
        {
            if (canvas.HasGameLoopThreadAccess)
            {
                callback();
            }
            else
            {
                await canvas.RunOnGameLoopThreadAsync(() =>
                {
                    callback();
                });
            }
        }

        public async Task<bool> LoadRomAsync(RomFile romFile)
        {
            bool success = false;

            await RunOnGameLoopThreadAsync(async () =>
            {
                success =  await _emulator.LoadRomAsync(romFile);
            });

            if (success)
            {
                canvas.Paused = false;
            }

            return success;
        }

        public async Task Screenshot()
        {
            string fileName = $"{Emulator.Instance.Rom.Name}-{DateTime.Now.ToString("yyyy-MM-ddHHmmss")}.png";
            StorageFolder screenshotsFolder = await Directories.GetScreenshotsFolderAync();
            StorageFile file = await screenshotsFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await ScreenshotInternal(file);
        }

        public async Task<bool> SaveState()
        {
            bool success = false;

            await RunOnGameLoopThreadAsync(() =>
            {
                success = _emulator.SaveState();

            });

            return success;
        }

        public async Task<bool> LoadState()
        {
            bool success = false;

            await RunOnGameLoopThreadAsync(() =>
            {
                success = _emulator.LoadState();
            });

            return success;
        }

        private async Task ScreenshotInternal(StorageFile file)
        {
            await RunOnGameLoopThreadAsync(async () =>
            {
                //using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                //{
                //    await _bitmap.SaveAsync(stream, CanvasBitmapFileFormat.Png, 1.0f);
                //}
            });
        }
    }
}
