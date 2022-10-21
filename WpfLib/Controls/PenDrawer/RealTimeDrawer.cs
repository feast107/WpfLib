using System.Windows;
using System.Windows.Media;
using WpfLib.Controls.PenDrawer.Base;
using WpfLib.Controls.PenDrawer.Interface;

namespace WpfLib.Controls.PenDrawer
{
    public partial class RealTimeDrawer : PathBasedDrawer
    {
        public RealTimeDrawer(int width, int height, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical) :base(width, height,direction) 
        { }
        #region Private fields
        private Point Last { get; set; }
        #endregion
        public override void OnPenDown()
        {
            base.OnPenDown();
            if (DrawCurrent != null)
            {
                OnPenUp();
            }
            InternalCanvas.Dispatcher.Invoke(() =>
            {
                DrawCurrent = new PathFigureGenerator(ColorAsBrush, ThicknessAsStroke);
                StoreCurrent = new PathFigureGenerator(ColorAsBrush, ThicknessAsStroke);
                InternalCanvas.Children.Add(DrawCurrent.Path);
            });
        }
        public override void OnPenUp()
        {  
            base.OnPenUp();
            InternalCanvas.Dispatcher.Invoke(() =>
            {
                BackupDictionary.Add(DrawCurrent.Path,GetStroke());
            });
            DrawCurrent = null;
            StoreCurrent = null;
        }
        public override void OnPenMove(Point point)
        {
            base.OnPenMove(point);
            var basePoint = point;
            point = new Point((int)(point.X * Scale), (int)(point.Y * Scale));
            if (Last == point)
            {
                return;
            }

            if (DrawCurrent == null)
            {
                OnPenDown();
            }
            InternalCanvas.Dispatcher.Invoke(() =>
            {
                DrawCurrent.Draw(point);
                StoreCurrent.Draw(basePoint);
            });
            Last = point;
        }
    }

    public partial class RealTimeDrawer
    {
        protected class PathFigureGenerator : PathGenerator
        {
            /// <summary>
            /// 当前绘制线条
            /// </summary>
            private PathFigure PathFigure { get; set; }
            /// <summary>
            /// 当前线条集合
            /// </summary>
            private PathFigureCollection PathFigures { get; }

            public PathFigureGenerator(Brush brush,double thickness):base(brush,thickness)
            {
                Path.Data = new PathGeometry(){ Figures = PathFigures = new()};
            }

            public override void Draw(Point point)
            {
                if (PathFigures.Count == 0)
                {
                    PathFigures.Add(PathFigure = new()
                    {
                        StartPoint = point,
                        IsClosed = false,
                        Segments = new()
                    });
                }
                else
                {
                    PathFigure?.Segments.Add(new LineSegment(point, true));
                }
            }
        }
    }
}
