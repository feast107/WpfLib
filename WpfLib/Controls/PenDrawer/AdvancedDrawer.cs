using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib.Controls.PenDrawer.Base;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace WpfLib.Controls.PenDrawer
{
    public class AdvancedDrawer : DrawerBase
    {
        public override FrameworkElement Canvas  => _canvas;
        public override IDrawBehavior.PageDirection Direction { get; set; }
        private readonly Canvas _canvas;
        private readonly List<Point> _points = new();
        public AdvancedDrawer(Size size, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical) :base(size,direction)
        {
            _canvas = new()
            {
                Width = ActualWidth,
                Height = ActualHeight,
            };
        }
        protected Path Current;
        protected StreamGeometry Stream { get; set; }
        private Point Last { get; set; }

        public override IList<StrokeModel> Strokes => throw new NotImplementedException();

        public override void OnPenDown()
        {
            if (Current != null)
            {
                OnPenUp();
            }
            _canvas.Dispatcher.Invoke(() =>
            {
                _canvas.Children.Add(Current = new()
                {
                    Stroke = Brushes.Black,
                    Data = Stream = new StreamGeometry()
                });
            });
        }
        public override void OnPenUp()
        {
            Current = null;
        }
        public override void OnPenMove(Point point)
        {
            point = new Point((int)(point.X * Scale), (int)(point.Y * Scale));
            if (point == Last)
            {
                return;
            }
            _points.Add(point);
            _canvas.Dispatcher.Invoke(() =>
            {
                using var s = Stream.Open();
                {
                    if (_points.Count % 3 == 0)
                    {
                        s.BeginFigure(_points[0], true, false);
                        s.LineTo(_points[1], true, true);
                        s.LineTo(_points[2], true, true);
                        _points.Clear();
                    }
                }
            });
            Last = point;
        }

        public override void Erase(Rect rubber)
        {
            throw new NotImplementedException();
        }

        public override void Erase(int from, int to)
        {
            throw new NotImplementedException();
        }

        public override void Erase(int from)
        {
            throw new NotImplementedException();
        }
    }
}
