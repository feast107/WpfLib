using System;
using System.Windows;

namespace WpfLib.Helpers
{
    [Flags]
    public enum Inputs : ulong
    {
        Nothing      = 0x0,

        LeftButton   = 0x1,
        RightButton  = 0x2,
        MiddleButton = 0x4,
        MouseMoving  = 0x8,

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
        public static bool Is(this Inputs inputses, Inputs flag)
        {
            if (flag == Inputs.Nothing)
            {
                return inputses == Inputs.Nothing;
            }
            return inputses.HasFlag(flag);
        }

    }

    /// <summary>
    /// 对一系列FrameworkElement进行输入设备捕捉的接口
    /// </summary>
    public interface IInputCatcher
    {
        /// <summary>
        /// 现有的输入
        /// </summary>
        Inputs Inputs { get; }
        /// <summary>
        /// 确认该组件是否被捕捉
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool Attached(FrameworkElement target);
        /// <summary>
        /// 捕捉该组件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IInputCatcher Attach(FrameworkElement target);
        /// <summary>
        /// 解除该组件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IInputCatcher Detach(FrameworkElement target);
        /// <summary>
        /// 重置输入状态
        /// </summary>
        void Reset();
        /// <summary>
        /// 输入更变委托
        /// </summary>
        /// <param name="all">当前全部输入</param>
        /// <param name="change">更变的输入</param>
        /// <param name="active">增加or减少</param>
        /// <param name="eventArgs">事件参数</param>
        public delegate void InputChangeEvent(Inputs all, Inputs change, bool active, EventArgs eventArgs);
        /// <summary>
        /// 输入更变事件
        /// </summary>
        public InputChangeEvent InputChange { get; set; }
    }
}
