﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using WpfLib.Controls.PenDrawer.Definition;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;
using Point = System.Windows.Point;

namespace WpfLib.Controls.PenDrawer.Base
{
    public abstract class DrawerBase : IDrawBehavior
    {
        public Brush ColorAsBrush
        {
            get
            {
                switch (Color)
                {
                    case StrokeColor.Black:
                        return Brushes.Black;
                    case StrokeColor.Blue:
                        return Brushes.Blue;
                    case StrokeColor.Green:
                        return Brushes.Green;
                    case StrokeColor.Red:
                        return Brushes.Red;
                }
                return Brushes.Black;
            }
        }
        public double ThicknessAsStroke
        {
            get
            {
                switch (Thickness)
                {
                    case StrokeThickness.VeryThin:
                        return 1;
                    case StrokeThickness.Thin:
                        return 2;
                    case StrokeThickness.Medium:
                        return 3;
                    case StrokeThickness.Bold:
                        return 4;
                    case StrokeThickness.VeryBold:
                        return 5;
                }
                return 1;
            }
        }

        public abstract FrameworkElement Canvas { get ;}

        protected int ActualWidth { get; }
        protected int ActualHeight { get; }
        public float Scale { get; set; }
        protected DrawerBase(int width, int height,IDrawBehavior.PageDirection direction)
        {
            switch (direction)
            {
                case IDrawBehavior.PageDirection.Vertical:
                    var ws = (int)(width / 210f * 297f);
                    if (ws > height)
                    {
                        width = (int)(height / (297f / 210f));
                    }
                    else
                    {
                        height = ws;
                    }
                    break;
                case IDrawBehavior.PageDirection.Horizontal:
                    var hs = (int)(height / 210f * 297f);
                    if (hs > width)
                    {
                        height = (int)(width / (297f / 210f));
                    }
                    else
                    {
                        width = hs;
                    }
                    break;
            }
            ActualHeight = height;
            ActualWidth = width;
            Scale = width / 5600f;
        }

        public IDrawBehavior.PageDirection Direction { get; }
        public IDrawBehavior.DrawStatus Status { get; private set; }
        public StrokeColor Color { get; set; } = StrokeColor.Black;
        public StrokeThickness Thickness { get; set; } = StrokeThickness.VeryThin;
        public abstract IList<StrokeModel> Strokes { get; }

        public virtual void OnPenUp()
        {
            Status = IDrawBehavior.DrawStatus.Waiting;
        }
        public virtual void OnPenDown()
        {
            Status = IDrawBehavior.DrawStatus.Waiting;
        }
        public virtual void OnPenMove(Point point)
        {
            Status = IDrawBehavior.DrawStatus.Drawing;
        }
        public abstract void Erase(Rect rubber);

    }
}