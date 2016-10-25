using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snes9x.Common
{
    public class GameLoopSynchronizationContext: SynchronizationContext
    {
        ICanvasAnimatedControl _control;

        public GameLoopSynchronizationContext(ICanvasAnimatedControl control)
        {
            _control = control;
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            var action = _control.RunOnGameLoopThreadAsync(() =>
            {
                SetSynchronizationContext(this);

                callback(state);
            });
        }
    }
}
