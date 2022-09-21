using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfLib.Controls.Definition
{
    [Flags]
    public enum CompletableStatus
    {
        Created    = 0x0,
        Ready      = 0x1,
        Working    = 0x2,
        Completed  = 0x4,
        Successful = 0xC,
        Canceled   = 0x14,
    }
    public static class CompletableExtension
    {
        public static bool Is(this CompletableStatus current, CompletableStatus target)
        {
            return current.HasFlag(target);
        }
    }
    public interface ICompletable
    {
        CompletableStatus Status { get; }
        /// <summary>
        /// 强制中断
        /// </summary>
        /// <returns></returns>
        bool Finish();
        delegate void StatusChangeEvent(CompletableStatus status);
        StatusChangeEvent StatusChange { get; set; }
    }
}
