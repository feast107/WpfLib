using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;

namespace WpfLib.Controls.PenDrawer.Base
{
    public class StrokeSaver : IExportStroke
    {
        public IList<StrokeModel> Strokes => _strokes;
        public int StrokeCount =>  Strokes.Count;
        private readonly List<StrokeModel> _strokes = new ();
        private readonly List<PointModel> _points = new ();
        public void PenMove(Point point)
        {
           _points.Add(new PointModel()
           {
               X = (int) (point.X * 1.4),
               Y = (int) (point.Y * 1.242)
           });
        }
        public void PenUp(StrokeModel incomplete)
        {
            if (_points.Count > 0)
            {
                incomplete.Path = _points.PointsToStroke();
                incomplete.X1 = _points[0].X;
                incomplete.Y1 = _points[0].Y;
                _strokes.Add(incomplete);
                _points.Clear();
            }
        }
        public void Remove(int index)
        {
            _strokes.RemoveAt(index);
        }
    }
}
