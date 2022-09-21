using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfLib.Helpers
{
    public static class MouseDeviceHelper
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
        public Cursor InitialCursor => _initialCursor;
        #region Private Fields
        private static MouseDevice _device;
        private static Cursor _initialCursor;
        #endregion
        public CursorHelper(MouseDevice device)
        {
            if (_device == null)
            {
                _device = device;
                _initialCursor = device.OverrideCursor;
            }
        }

        public bool Recover()
        {
            return SetCursor(null);
        }

        public bool SetCursor(Cursor cursor)
        {
            return Device.SetCursor(cursor);
        }
    }
}
