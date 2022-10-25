using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfLib.Controls.PenDrawer.Base;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;
using Point = System.Windows.Point;

namespace WpfLib.Controls.PenDrawer
{
    public class VisualDrawer : DrawerBase
    {
        public VisualDrawer(Size size, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical) : base(size, direction)
        {
            _canvas = new DrawingVisualElement()
            {
                Width = ActualWidth,
                Height = ActualHeight,
            };
           _canvas.DrawingVisual.Drawing?.Open().DrawImage(new BitmapImage(),new Rect());
        }
        private BitmapImage _source = new ();

        public override FrameworkElement Canvas => _canvas;

        public override IList<StrokeModel> Strokes => throw new NotImplementedException();

        private readonly DrawingVisualElement _canvas;
        public override void OnPenUp()
        {
            base.OnPenUp();
        }

        public override void OnPenDown()
        {
            base.OnPenDown();
        }

        public override void OnPenMove(Point point)
        {
            base.OnPenMove(point);
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
