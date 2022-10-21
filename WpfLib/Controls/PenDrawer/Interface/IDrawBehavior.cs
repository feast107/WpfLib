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

        #region 绘制事件
        void OnPenUp();
        void OnPenDown();
        void OnPenMove(Point point);

        void Erase(Rect rubber);
        #endregion

    }
}
