using System.Windows.Input;

namespace WpfLib.Helpers
{
    public static class MouseDeviceExtension
    {
        public static bool Recover(this MouseDevice device)
        {
            return device.SetCursor(null);
        }
    }
    public class CursorHelper
    {
        /// <summary>
        /// 鼠标设备
        /// </summary>
        public MouseDevice Device => _device;

        /// <summary>
        /// 初始光标
        /// </summary>
        public readonly Cursor InitialCursor;
        #region Private Fields
        private static MouseDevice _device;
        #endregion
        public CursorHelper(MouseDevice device)
        {
            _device ??= device;
            InitialCursor = device.OverrideCursor;
        }

        public void Recover()
        {
             SetCursor(InitialCursor);
        }

        public void SetCursor(Cursor cursor)
        {
            Device.OverrideCursor =cursor;
        }
    }
}
