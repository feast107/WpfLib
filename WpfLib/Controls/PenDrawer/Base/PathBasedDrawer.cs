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

namespace WpfLib.Controls.PenDrawer.Base
{
    /// <summary>
    /// 基于Path绘制的Drawer基类
    /// </summary>
    public abstract class PathBasedDrawer :DrawerBase
    {
        protected StrokeModel GetStroke()
        {
            StrokeModel model = new ()
            {
                Path = StoreCurrent.Path.Data.ToString(),
                Color = Color,
                Thickness = Thickness,
                Timestamp = DateTime.Now.ToBinary(),
                X1 = (int)StoreCurrent.FirstPoint?.X,
                Y1 = (int)StoreCurrent.FirstPoint?.Y
            };
            return model;
        }
        protected abstract class PathGenerator
        {
            public Point? FirstPoint { get;  private set; }
            public Path Path { get; }
            protected PathGenerator(Brush brush, double thickness)
            {
                Path = new Path()
                {
                    Stroke = brush,
                    StrokeThickness = thickness,
                };
            }

            public virtual void Draw(Point point)
            {
                FirstPoint ??= point;
            }
            public virtual void End(){}
        }
        
        protected PathBasedDrawer(Size size, IDrawBehavior.PageDirection direction) : base(size,direction)
        {
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

        protected readonly Canvas InternalCanvas;
        /// <summary>
        /// 当前绘制笔迹
        /// </summary>
        protected PathGenerator DrawCurrent { get; set; }
        /// <summary>
        /// 当前待存笔迹
        /// </summary>
        protected PathGenerator StoreCurrent { get; set; }
        
        /// <summary>
        /// 对笔记进行的存储
        /// </summary>
        protected readonly Dictionary<Path, StrokeModel> BackupDictionary = new();
        public override IList<StrokeModel> Strokes => BackupDictionary.Values.ToList();

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
                    BackupDictionary.Remove((Path)InternalCanvas.Children[i]);
                    InternalCanvas.Children.RemoveAt(i);
                    i--;
                }
            }
        }
        public override void Erase(int from, int to)
        {
            if (from > to || from < 0 || to > BackupDictionary.Count) return;
            OnPenUp();
            for (int i = to; i > from ;i--)
            {
                BackupDictionary.Remove((Path)InternalCanvas.Children[i]);
                InternalCanvas.Children.RemoveAt(i);
            }
        }
        public override void Erase(int from)
        {
            if (from < 0 || from >= BackupDictionary.Count) return;
            OnPenUp();
            while (BackupDictionary.Count > from)
            {
                int i = BackupDictionary.Count - 1;
                BackupDictionary.Remove((Path)InternalCanvas.Children[i]);
                InternalCanvas.Children.RemoveAt(i);
            }
        }
    }
}
