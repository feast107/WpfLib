using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace WpfLib.Controls
{
    /// <summary>
    /// Mask.xaml 的交互逻辑
    /// </summary>
    public partial class Mask
    {
        enum SourceType
        {
            Panel,
            ContentControl
        }
        private readonly FrameworkElement _src;
        private readonly SourceType _sourceType;
        private readonly List<UIElement> _elements = new();

        public Point? StartPosition { get; private set; }
        public Point? EndPosition { get; private set; }

        public Rect Region { get; private set; }

        private Point MoveLast { get; set; }
        private bool Drag { get; set; }
        public Mask(Panel container)
        {
            InitializeComponent();
            _src = container;
            _sourceType = SourceType.Panel;
            while (container.Children.Count > 0)
            {
                var child = container.Children[0];
                _elements.Add(child);
                child.IsEnabled = false;
                container.Children.RemoveAt(0);
                Children.Add(child);
            }
            Init(container);
        }
        public Mask(ContentControl container)
        {
            InitializeComponent();
            _src = container;
            _sourceType = SourceType.ContentControl;
            var o = (UIElement)container.Content;
            container.Content = this;
            Children.Insert(0,o);
            _elements.Add(o);
            o.IsEnabled = false;
            Init(container);
        }

        private bool Down { get; set; }
        private void Init(FrameworkElement container)
        {
            Height = container.ActualHeight;
            Width = container.ActualWidth;
            TopPanel.Height = BottomPanel.Height = Height / 2;
            LeftPanel.Width = RightPanel.Width = Width / 2;

            MouseDown += (o,e) =>
            {
                if (e.ChangedButton== MouseButton.Left)
                {
                    Region = new Rect(e.GetPosition(this),new Size(0,0));
                    var sp = e.GetPosition(this);
                    if (StartPosition == null && 
                        sp.Y >= 0 && 
                        sp.X >= 0 && 
                        sp.Y <= Height && 
                        sp.X <= Width)
                    {
                        StartPosition = sp;
                        {
                            TopPanel.Height = sp.Y;
                            BottomPanel.Height = Height - sp.Y;
                            LeftPanel.Width = sp.X;
                            BottomPanel.Width = Width - sp.Y;
                        }
                        Down = true;
                    }
                }
            };
            MouseUp += (o, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    var ep = e.GetPosition(this);
                    if (ep.X >= 0 && ep.Y >= 0 && ep.Y <= Height && ep.X <= Width)
                    {
                        EndPosition = ep;
                        Down = false;
                        ClipRect.Visibility = Visibility.Visible;
                    }
                }
            };
            MouseMove += (o, e) =>
            {
                if (Down)
                {
                    var ep = e.GetPosition(this);
                    if(ep.X>=0 &&ep.Y >=0 && ep.Y<=Height && ep.X<=Width)
                    {
                        EndPosition = ep;
                        var sp = StartPosition;
                        if (sp != null)
                        {
                            TopPanel.Height = (double)sp?.Y > ep.Y ? ep.Y : (double)sp?.Y;
                            BottomPanel.Height = Height - ((double)sp?.Y > ep.Y ? (double)sp?.Y : ep.Y);
                            LeftPanel.Width = (double)sp?.X > ep.X ? ep.X : (double)sp?.X;
                            RightPanel.Width = Width - ((double)sp?.X > ep.X ? (double)sp?.X : ep.X);
                        }
                    }
                }
            };

            ClipRect.MouseDown += (o, e) =>
            {
                e.MouseDevice.SetCursor(Cursors.Hand);
                MoveLast = e.GetPosition(this);
                Drag = true;
            };
            ClipRect.MouseUp += (o, e) =>
            {
                Drag = false;
                e.MouseDevice.SetCursor(Cursors.Arrow);
                MoveLast = new Point();
            };
            ClipRect.MouseMove += (o, e) =>
            {
                if (Drag)
                {
                    var now = e.GetPosition(this);
                    double horizonOffset = now.X - MoveLast.X;
                    double verticalOffset = now.Y - MoveLast.Y;
                    if (StartPosition?.X + horizonOffset > 0 && 
                        EndPosition?.X + horizonOffset < ActualWidth)
                    {
                        LeftMove(horizonOffset);
                        RightMove(horizonOffset);
                    }
                    if (StartPosition?.Y + verticalOffset > 0 && 
                        EndPosition?.Y + verticalOffset < ActualHeight)
                    {
                        TopMove(verticalOffset);
                        BottomMove(verticalOffset);
                    }
                    MoveLast = now;
                }
            };
        }

        public void Finish()
        {
            Dispatcher.Invoke(() =>
            {
                switch (_sourceType)
                {
                    case SourceType.Panel:
                        Children.Clear();
                        _elements.ForEach(x =>
                        {
                            ((Panel)_src).Children.Add(x);
                            x.IsEnabled = true;
                        });
                        break;
                    case SourceType.ContentControl:
                        Children.RemoveAt(0);
                        ((ContentControl)_src).Content = _elements[0];
                        _elements[0].IsEnabled = true;
                        break;
                }
            });
        }

        private void LeftMove(double offset)
        {
            var sp = StartPosition ?? new Point();
            var ep = EndPosition ?? new Point();
            if (sp.X + offset > 0)
            {
                if (sp.X + offset < ep.X)
                {
                    StartPosition = new Point(sp.X + offset, sp.Y);
                    LeftPanel.Width += offset;
                }
                else
                {
                    StartPosition = new Point(ep.X - 1, sp.Y);
                    LeftPanel.Width = ep.X - 1;
                }
            }
        }
        private void RightMove(double offset)
        {
            var sp = StartPosition ?? new Point();
            var ep = EndPosition ?? new Point();
            if (ep.X + offset < ActualWidth)
            {
                if (ep.X + offset > sp.X)
                {
                    EndPosition = new Point(ep.X + offset, ep.Y);
                    RightPanel.Width -= offset;
                }
                else
                {
                    EndPosition = new Point(sp.X + 1, ep.Y);
                    RightPanel.Width = ActualWidth - (sp.X - 1);
                }
            }
        }
        private void TopMove(double offset)
        {
            var sp = StartPosition ?? new Point();
            var ep = EndPosition ?? new Point();
            if (sp.Y + offset > 0)
            {
                if (sp.Y + offset < ep.Y)
                {
                    StartPosition = new Point(sp.X, sp.Y + offset);
                    TopPanel.Height += offset;
                }
                else
                {
                    StartPosition = new Point(sp.X, ep.Y - 1);
                    TopPanel.Height = ep.Y - 1;
                }
            }
        }
        private void BottomMove(double offset)
        {
            var sp = StartPosition ?? new Point();
            var ep = EndPosition ?? new Point();
            if (ep.Y + offset < ActualHeight)
            {
                if (ep.Y + offset > sp.Y)
                {
                    EndPosition = new Point(ep.X, ep.Y + offset);
                    BottomPanel.Height -= offset;
                }
                else
                {
                    EndPosition = new Point(ep.X, sp.Y + 1);
                    BottomPanel.Height = ActualHeight - (sp.Y - 1);
                }

            }
        }
        
        
        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var t = (Thumb)sender;
            var sp = StartPosition ?? new Point();
            var ep = EndPosition ?? new Point();
            switch (t.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    LeftMove(e.HorizontalChange);
                    break;
                case HorizontalAlignment.Right:
                    RightMove(e.HorizontalChange);
                    break;
            }
            switch (t.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    TopMove(e.VerticalChange);
                    break;
                case VerticalAlignment.Bottom:
                    BottomMove(e.VerticalChange);
                    break;
            }
        }


    }
}
