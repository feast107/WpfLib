using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfLib.Controls.PenDrawer.Definition;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;
using WpfLib.Helpers;
using Point = System.Windows.Point;

namespace WpfLib.Controls.PenDrawer.Base
{
    public abstract partial class DrawerBase : IDrawBehavior
    {
        protected DrawerBase(Size size,IDrawBehavior.PageDirection direction)
        {
            RenderQueue = RenderLead;
            StartRender();
            var s = Resize(size, direction);
            ActualHeight = (int)s.Height;
            ActualWidth = (int)s.Width;
            Scale = (float)s.Width / 5600f;
        }
        public abstract FrameworkElement Canvas { get ;}

        #region 基础属性
        public Brush ColorAsBrush
        {
            get
            {
                return Color switch
                {
                    StrokeColor.Black => Brushes.Black,
                    StrokeColor.Blue => Brushes.Blue,
                    StrokeColor.Green => Brushes.Green,
                    StrokeColor.Red => Brushes.Red,
                    _ => Brushes.Black
                };
            }
        }
        public double ThicknessAsStroke
        {
            get
            {
                return Thickness switch
                {
                    StrokeThickness.VeryThin => 1,
                    StrokeThickness.Thin => 2,
                    StrokeThickness.Medium => 3,
                    StrokeThickness.Bold => 4,
                    StrokeThickness.VeryBold => 5,
                    _ => 1
                };
            }
        }
        public int ActualWidth { get; protected set; }
        public int ActualHeight { get; protected set; }
        public float Scale { get; set; }
        public abstract IDrawBehavior.PageDirection Direction { get; set; }
        public IDrawBehavior.DrawStatus Status { get; private set; }
        public StrokeColor Color { get; set; } = StrokeColor.Black;
        public StrokeThickness Thickness { get; set; } = StrokeThickness.VeryThin;
        #endregion

        #region Methods
        public virtual void OnPenUp()
        {
            Saver.PenUp(new StrokeModel()
            {
                Color = Color,
                Thickness = Thickness,
                Timestamp = DateTime.Now.TimeStamp()
            });
            Status = IDrawBehavior.DrawStatus.Waiting;
        }
        public virtual void OnPenDown()
        {
            Status = IDrawBehavior.DrawStatus.Waiting;
        }
        public virtual void OnPenMove(Point point)
        {
            Saver.PenMove(point);
            Status = IDrawBehavior.DrawStatus.Drawing;
        }


        public abstract void Erase(Rect rubber);
        public abstract void Erase(int from, int to);
        public abstract void Erase(int from);
        #endregion

        protected static Size Resize(Size from, IDrawBehavior.PageDirection direction)
        {
            double width = from.Width;
            double height = from.Height;
            switch (direction)
            {
                case IDrawBehavior.PageDirection.Vertical:
                    var ws = (int)(width / 210f * 297f);
                    if (ws > height)
                    {
                        width = (int)(height / (297f / 210f));
                    }
                    else
                    {
                        height = ws;
                    }
                    break;
                case IDrawBehavior.PageDirection.Horizontal:
                    var hs = (int)(height / 210f * 297f);
                    if (hs > width)
                    {
                        height = (int)(width / (297f / 210f));
                    }
                    else
                    {
                        width = hs;
                    }
                    break;
            }
            return new Size(width, height);
        }
    }

    /// <summary>
    /// 渲染相关的逻辑
    /// </summary>
    public abstract partial class DrawerBase
    {
        #region Renders
        private Task RenderLead { get; set; } = new(()=>{ });
        private Task RenderQueue { get; set; }
        public bool IsRendering { get; private set; } = false;
        public void QueueRender(Action action)
        {
            RenderQueue = RenderQueue.ContinueWith((t) =>
            {
                action();
            }, TaskContinuationOptions.AttachedToParent);
        }
        public void StartRender()
        {
            if (!IsRendering)
            {
                RenderLead.Start();
                IsRendering = true;
            }
        }
        public void PauseRender()
        {
            if (!IsRendering) return;
            RenderLead = new Task(() => { });
            RenderQueue = RenderLead;
            IsRendering = false;
        }
        public void CleanRender()
        {
            RenderLead = new Task(() => { });
            RenderQueue = RenderLead;
        }
        #endregion
    }

    /// <summary>
    /// 存储相关的逻辑
    /// </summary>
    public abstract partial class DrawerBase
    {
        #region 存储区域
        public IList<StrokeModel> Strokes => Saver.Strokes;
        public int StrokeCount => Saver.StrokeCount;
        protected StrokeSaver Saver { get; } = new();
        #endregion
    }
}
