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
    public enum Status
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
    public static class StatusExtension
    {
        public static bool Is(this Status current, Status target)
        {
            return current.HasFlag(target);
        }
    }
    
    /// <summary>
    /// 脱手控件状态机
    /// </summary>
    public interface IStatusMachine
    {
        Status Status { get; }
        /// <summary>
        /// 强制中断
        /// </summary>
        /// <returns></returns>
        bool Finish();
        delegate void StatusChangeEvent(Status status);
        StatusChangeEvent StatusChange { get; set; }
    }
}
