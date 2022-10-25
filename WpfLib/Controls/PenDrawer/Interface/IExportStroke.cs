using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfLib.Controls.PenDrawer.Model;

namespace WpfLib.Controls.PenDrawer.Interface
{
    public interface IExportStroke
    {
        public IList<StrokeModel> Strokes { get; }
        int StrokeCount { get; }
    }
}
