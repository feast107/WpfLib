using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfLib.Controls.Definition
{
    /// <summary>
    /// 脱手控件的当前状态
    /// </summary>
    [Flags]
    public enum CompletableStatus
    {
        /// <summary>
        /// 已创建
        /// </summary>
        Created    = 0x1,
        /// <summary>
        /// 就绪
        /// </summary>
        Ready      = 0x3,//0x1 + 0x2
        /// <summary>
        /// 工作中
        /// </summary>
        Working    = 0x5,//0x1 + 0x4
        /// <summary>
        /// 完成其生命周期
        /// </summary>
        Completed  = 0x8,
        /// <summary>
        /// 成功
        /// </summary>
        Successful = 0x18,//0x10 + 0x8
        /// <summary>
        /// 取消
        /// </summary>
        Canceled   = 0x28,//0x20 + 0x8
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
