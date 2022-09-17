using System;
using System.Collections.Generic;
using System.Windows;

namespace WpfLib.Helpers
{
    [Flags]
    public enum Input : ulong
    {
        Nothing      = 0x0,

        LeftButton   = 0x1,
        RightButton  = 0x2,
        MiddleButton = 0x4,

        LeftCtrl     = 0x10,
        LeftAlt      = 0x20,
        CapsLock     = 0x40,
        LeftShift    = 0x80,

        Enter        = 0x100,
        ExSel        = 0x200,
        Space        = 0x400,
        RightCtrl    = 0x800,

        A            = 0x1000,

        BothClick    = 0x3,
        ScreenShot   = 0x1030,
    }

    public static class InputExtension
    {
        public static bool Is(this Input inputs, Input flag)
        {
            if (flag == Input.Nothing)
            {
                return inputs == Input.Nothing;
            }
            return inputs.HasFlag(flag);
        }

    }

    public interface IInput
    {
        Input Inputs { get; }
        bool Attached(FrameworkElement target);
        IInput Attach(FrameworkElement target);
        IInput Detach(FrameworkElement target);
        void Reset();

        public delegate void InputChangeEvent(Input all, Input change, bool active, EventArgs eventArgs);
        public InputChangeEvent InputChange { get; set; }
    }
}
