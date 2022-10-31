using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;
using WpfLib.Helpers;

namespace WpfLib.Controls.PenDrawer.Base
{
    /// <summary>
    /// 基于Path绘制的Drawer基类
    /// </summary>
    public abstract class PathBasedDrawer :DrawerBase
    {
        protected abstract class PathGenerator
        {
            public List<PointModel> Points = new();
            public Path Path { get; }
            protected PathGenerator(Brush brush, double thickness)
            {
                Path = new()
                {
                    Stroke = brush,
                    StrokeThickness = thickness,
                };
            }

            public virtual void Draw(Point point)
            {
                Points.Add(new PointModel()
                {
                    X = (int)point.X,Y= (int)point.Y
                });
            }
            public virtual void End(){}
        }
        
        protected PathBasedDrawer(Size size, IDrawBehavior.PageDirection direction) : base(size,direction)
        {
            _directionInternal = direction;
            InternalCanvas = new Canvas()
            {
                Width = ActualWidth,
                Height = ActualHeight,
            };
            InternalCanvas.SizeChanged += (o, e) =>
            {
                Size target = Resize(e.NewSize, direction);
                InternalCanvas.Width = target.Width;
                InternalCanvas.Height = target.Height;
            };
        }
        public override FrameworkElement Canvas => InternalCanvas;

        public override IDrawBehavior.PageDirection Direction
        {
            get => _directionInternal;
            set
            {
                if (_directionInternal != value)
                {
                    _directionInternal = value;
                    int tmp = ActualHeight;
                    ActualHeight = ActualWidth;
                    ActualWidth = tmp;
                    InternalCanvas.Dispatcher.Invoke(() =>
                    {
                        InternalCanvas.Width = ActualWidth;
                        InternalCanvas.Height = ActualHeight;
                    });
                }
            }
        }

        private IDrawBehavior.PageDirection _directionInternal;

        protected readonly Canvas InternalCanvas;
        /// <summary>
        /// 当前绘制笔迹
        /// </summary>
        protected PathGenerator DrawCurrent { get; set; }
        

        public override void Erase(Rect rubber)
        {
            if (rubber.Left > Canvas.Width || rubber.Bottom > Canvas.Height)
            {
                return;
            }
            Geometry r = new RectangleGeometry(rubber);
            for (var i = 0; i < InternalCanvas.Children.Count; i++)
            {
                var detail = ((Path)InternalCanvas.Children[i]).Data.FillContainsWithDetail(r);
                if (detail != IntersectionDetail.Empty)
                {
                    Saver.Remove(i);
                    InternalCanvas.Children.RemoveAt(i);
                    i--;
                }
            }
        }
        public override void Erase(int from, int to)
        {
            if (from > to || from < 0 || to > StrokeCount) return;
            OnPenUp();
            for (int i = to; i > from ;i--)
            {
                Saver.Remove(i);
                InternalCanvas.Children.RemoveAt(i);
            }
        }
        public override void Erase(int from)
        {
            if (from < 0 || from >= StrokeCount) return;
            OnPenUp();
            while (StrokeCount > from)
            {
                int i = StrokeCount - 1;
                Saver.Remove(i);
                InternalCanvas.Children.RemoveAt(i);
            }
        }
    }
}
