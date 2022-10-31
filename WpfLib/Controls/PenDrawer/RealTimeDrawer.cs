using SharpDX.Direct2D1.Effects;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WpfLib.Controls.PenDrawer.Base;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;

namespace WpfLib.Controls.PenDrawer
{
    public partial class RealTimeDrawer : PathBasedDrawer
    {
        public RealTimeDrawer(Size size, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical)
            :base(size,direction) { }

        public RealTimeDrawer(int width,int height, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical)
            : base(new Size(width,height), direction) { }

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
                DrawCurrent = new BezierFigureGenerator(ColorAsBrush, ThicknessAsStroke);
                InternalCanvas.Children.Add(DrawCurrent.Path);
            });
        }

        public override void OnPenUp()
        {
            base.OnPenUp();
            DrawCurrent = null;
        }

        public override void OnPenMove(Point point)
        {
            base.OnPenMove(point);
            point = new Point((point.X * Scale), (point.Y * Scale));
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
                base.Draw(point);
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

        protected class BezierFigureGenerator : PathGenerator
        {
            public BezierFigureGenerator(Brush brush, double thickness) : base(brush, thickness)
            {
                Path.Data = new PathGeometry() { Figures = PathFigures = new() };
            }

            /// <summary>
            /// 当前绘制线条
            /// </summary>
            private PathFigure PathFigure { get; set; }
            /// <summary>
            /// 当前线条集合
            /// </summary>
            private PathFigureCollection PathFigures { get; }

            private Point _lastPoint;
            public override void Draw(Point point)
            {
                base.Draw(point);
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
                    double offsetX = ((float)(point.X - _lastPoint.X));
                    double offsetY = ((float)(point.Y - _lastPoint.Y));

                    double fin = Math.Sqrt(offsetX * offsetX + offsetY * offsetY);
                    switch (fin)
                    {
                        case >= 2:
                        {
                            Point control = new Point(_lastPoint.X + offsetX / 3.0,
                                _lastPoint.Y + offsetY / 3.0);
                            Point end = new Point(_lastPoint.X + offsetX / 3.0 * 2.0,
                                _lastPoint.Y + offsetY / 3.0 * 2.0);
                            PathFigure?.Segments.Add(new BezierSegment(_lastPoint, control, end, true));
                            break;
                        }
                        case >= 1:
                        {
                            Point control = new Point(_lastPoint.X + offsetX / 2.0,
                                _lastPoint.Y + offsetY / 2.0);
                            PathFigure?.Segments.Add(
                                new PolyQuadraticBezierSegment(new[] { _lastPoint, control }, true));
                                break;
                        }
                        default:
                            PathFigure?.Segments.Add(new LineSegment(point, true));
                            break;
                    }
                }

                _lastPoint = point;
            }
        }
    }
}
