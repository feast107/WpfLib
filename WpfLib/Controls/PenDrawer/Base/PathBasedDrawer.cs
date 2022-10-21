using System;
using System.Collections.Generic;
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
            StrokeModel model = new StrokeModel
            {
                Path = StoreCurrent.Path.Data.ToString(),
                Color = Color,
                Thickness = Thickness,
                Timestamp = DateTime.Now.ToBinary()
            };
            return model;
        }
        protected abstract class PathGenerator
        {
            public Path Path { get; }
            protected PathGenerator(Brush brush, double thickness)
            {
                Path = new Path()
                {
                    Stroke = brush,
                    StrokeThickness = thickness,
                };
            }
            public abstract void Draw(Point point);
            public virtual void End(){}
        }
        
        protected PathBasedDrawer(int width, int height, IDrawBehavior.PageDirection direction) : base(width, height,direction)
        {
            InternalCanvas = new Canvas()
            {
                Width = ActualWidth,
                Height = ActualHeight,
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
    }
}
