using System.Windows;
using System.Windows.Media;
using WpfLib.Controls.PenDrawer.Base;
using WpfLib.Controls.PenDrawer.Interface;

namespace WpfLib.Controls.PenDrawer
{
    public partial class FragmentDrawer : PathBasedDrawer
    {
        public FragmentDrawer(Size size, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical) 
            : base(size,direction) { }

        public FragmentDrawer(int width,int height, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical)
            : base(new Size(width,height), direction) { }
        #region Private Fields

        private Point Last { get; set; }
        #endregion

        public override void OnPenDown()
        {
            if (DrawCurrent != null)
            {
               OnPenUp();
            }
            InternalCanvas.Dispatcher.Invoke(() =>
            {
                DrawCurrent = new PathStreamGenerator(ColorAsBrush, ThicknessAsStroke);
                StoreCurrent = new PathStreamGenerator(ColorAsBrush, ThicknessAsStroke);
            });
        }
        public override void OnPenUp()
        {
            InternalCanvas.Dispatcher.Invoke(() =>
            {
                DrawCurrent.End();
                StoreCurrent.End();
            });
            if (StoreCurrent != null && StoreCurrent.Points.Count > 0)
            {
                InternalCanvas.Dispatcher.Invoke(() => { BackupDictionary.Add(DrawCurrent.Path, GetStroke()); });
            }
            DrawCurrent = null;
            StoreCurrent = null;
        }
        public override void OnPenMove(Point point)
        {
            var basePoint = point;
            point = new Point(point.X * Scale, point.Y * Scale);
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

    public partial class FragmentDrawer
    {
        protected class PathStreamGenerator : PathGenerator
        {
            protected StreamGeometry Stream { get; set; }
            private StreamGeometryContext StreamContext { get; set; }

            public PathStreamGenerator(Brush brush, double thickness):base(brush,thickness)
            {
                Path.Data = Stream = new StreamGeometry();
                StreamContext = Stream.Open();
            }

            private bool IsGet { get; set; } = false;
            public override void Draw(Point point)
            {
                base.Draw(point);
                if (!IsGet)
                {
                    StreamContext.BeginFigure(point, true, false);
                    IsGet = true;
                }
                else
                {
                    StreamContext.LineTo(point, true, true);
                }
            }
            public override void End()
            {
                StreamContext.Close();
            }
        }
    }
}
