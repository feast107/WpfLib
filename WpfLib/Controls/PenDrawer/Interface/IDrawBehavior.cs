using System;
using System.Collections.Generic;
using System.Windows;
using WpfLib.Controls.PenDrawer.Definition;
using WpfLib.Controls.PenDrawer.Model;

namespace WpfLib.Controls.PenDrawer.Interface
{
    /// <summary>
    /// 点阵笔绘制行为
    /// </summary>
    public interface IDrawBehavior
    {
        /// <summary>
        /// 显示控件
        /// </summary>
        FrameworkElement Canvas { get; }

        enum DrawStatus
        {
            Waiting,
            Drawing
        }

        enum PageDirection
        {
            Horizontal,
            Vertical,
        }

        PageDirection Direction { get; }
        DrawStatus Status { get; } 
        StrokeColor Color { get; set; } 
        StrokeThickness Thickness { get; set; }
        IList<StrokeModel> Strokes { get; }
        int StrokeCount { get; }
        #region 绘制事件
        void OnPenUp();
        void OnPenDown();
        void OnPenMove(Point point);
        void Erase(Rect rubber);
        void Erase(int from, int to);
        void Erase(int from);
        #endregion

        bool IsRendering { get; }
        /// <summary>
        /// 添加渲染队列
        /// </summary>
        /// <param name="action"></param>
        void QueueRender(Action action);
        /// <summary>
        /// 启动渲染(默认启动)
        /// </summary>
        void StartRender();
        /// <summary>
        /// 暂停渲染
        /// </summary>
        void PauseRender();
        /// <summary>
        /// 清空渲染队列，放弃之后的渲染
        /// </summary>
        void CleanRender();
    }
}
