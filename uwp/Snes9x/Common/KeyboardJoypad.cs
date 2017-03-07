using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snes9xCore;
using Windows.UI.Core;
using Windows.System;

namespace Snes9x.Common
{
    class KeyboardJoypadConfig
    {
        public VirtualKey A { get; set; }
        public VirtualKey B { get; set; }
        public VirtualKey X { get; set; }
        public VirtualKey Y { get; set; }
        public VirtualKey L { get; set; }
        public VirtualKey R { get; set; }
        public VirtualKey Start { get; set; }
        public VirtualKey Select { get; set; }
        public VirtualKey Up { get; set; }
        public VirtualKey Down { get; set; }
        public VirtualKey Left { get; set; }
        public VirtualKey Right { get; set; }

        public static readonly KeyboardJoypadConfig Default = new KeyboardJoypadConfig
        {
            A = VirtualKey.V,
            B = VirtualKey.C,
            X = VirtualKey.D,
            Y = VirtualKey.X,
            L = VirtualKey.A,
            R = VirtualKey.S,
            Start = VirtualKey.Space,
            Select = VirtualKey.Enter,
            Up = VirtualKey.Up,
            Down = VirtualKey.Down,
            Left = VirtualKey.Left,
            Right = VirtualKey.Right
        };

        public static readonly KeyboardJoypadConfig Gamepad = new KeyboardJoypadConfig
        {
            A = VirtualKey.GamepadB,
            B = VirtualKey.GamepadA,
            X = VirtualKey.GamepadY,
            Y = VirtualKey.GamepadX,
            L = VirtualKey.GamepadLeftShoulder,
            R = VirtualKey.GamepadRightShoulder,
            Start = VirtualKey.GamepadMenu,
            Select = VirtualKey.GamepadView,
            Up = VirtualKey.GamepadDPadUp,
            Down = VirtualKey.GamepadDPadDown,
            Left = VirtualKey.GamepadDPadLeft,
            Right = VirtualKey.GamepadDPadRight
        };
    }

    class KeyboardJoypad : IJoypad
    {
        private CoreJoypad _coreJoypad;

        public KeyboardJoypadConfig Config { get; private set; }

        private bool A, B, X, Y, L, R, Start, Select, Up, Down, Left, Right = false;

        public KeyboardJoypad(uint joypadNumber)
            : this(joypadNumber, KeyboardJoypadConfig.Default)
        {
        }

        public KeyboardJoypad(uint joypadNumber, KeyboardJoypadConfig config)
        {
            _coreJoypad = new CoreJoypad(joypadNumber);
            Config = config;
            CoreWindow window = CoreWindow.GetForCurrentThread();
            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;
        }

        public uint JoypadNumber
        {
            get
            {
                return _coreJoypad.JoypadNumber;
            }
            set
            {
                _coreJoypad.JoypadNumber = value;
            }
        }

        public void ReportButtons()
        {
            _coreJoypad.ReportA(A);
            _coreJoypad.ReportB(B);
            _coreJoypad.ReportX(X);
            _coreJoypad.ReportY(Y);
            _coreJoypad.ReportL(L);
            _coreJoypad.ReportR(R);
            _coreJoypad.ReportStart(Start);
            _coreJoypad.ReportSelect(Select);
            _coreJoypad.ReportUp(Up);
            _coreJoypad.ReportDown(Down);
            _coreJoypad.ReportLeft(Left);
            _coreJoypad.ReportRight(Right);
        }

        private void Window_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            VirtualKey key = args.VirtualKey;

            if (key == Config.A)            A =         false;
            else if (key == Config.B)       B =         false;
            else if (key == Config.X)       X =         false;
            else if (key == Config.Y)       Y =         false;
            else if (key == Config.L)       L =         false;
            else if (key == Config.R)       R =         false;
            else if (key == Config.Start)   Start =     false;
            else if (key == Config.Select)  Select =    false;
            else if (key == Config.Up)      Up =        false;
            else if (key == Config.Down)    Down =      false;
            else if (key == Config.Left)    Left =      false;
            else if (key == Config.Right)   Right =     false;

            args.Handled = true;
        }

        private void Window_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.KeyStatus.RepeatCount == 1)
            {
                VirtualKey key = args.VirtualKey;

                if (key == Config.A)            A =         true;
                else if (key == Config.B)       B =         true;
                else if (key == Config.X)       X =         true;
                else if (key == Config.Y)       Y =         true;
                else if (key == Config.L)       L =         true;
                else if (key == Config.R)       R =         true;
                else if (key == Config.Start)   Start =     true;
                else if (key == Config.Select)  Select =    true;
                else if (key == Config.Up)      Up =        true;
                else if (key == Config.Down)    Down =      true;
                else if (key == Config.Left)    Left =      true;
                else if (key == Config.Right)   Right =     true;
            }

            args.Handled = true;
        }
    }
}
