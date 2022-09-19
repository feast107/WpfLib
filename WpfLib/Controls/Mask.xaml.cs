using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
using WpfLib.Controls.Definition;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace WpfLib.Controls
{
    /// <summary>
    /// 用于空间区域截图的控件，双击完成截图
    /// </summary>
    public partial class Mask : Grid ,ICompletable
    {
        #region Private Fields
        enum SourceType
        {
            Panel,
            ContentControl,
            Border
        }
        private readonly FrameworkElement _src;
        private readonly SourceType _sourceType;
        private readonly List<UIElement> _elements = new();
        private Point MoveLast { get; set; }
        private bool Drag { get; set; }
        #endregion
        
        public Point? StartPosition { get; private set; }
        public Point? EndPosition { get; private set; }
        
        private new UIElementCollection Children => Container.Children;
        

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
            container.Children.Add(this);
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
        public Mask(Border container)
        {
            InitializeComponent();
            _src = container;
            _sourceType = SourceType.Border;
            var o = container.Child;
            container.Child = this;
            Children.Insert(0,o);
            _elements.Add(o);
            o.IsEnabled = false;
            Init(container);
        }

        private void Recover()
        {
            Dispatcher.Invoke(() =>
            {
                switch (_sourceType)
                {
                    case SourceType.Panel:
                        Children.Clear();
                        ((Panel)_src).Children.Remove(this);
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
                    case SourceType.Border:
                        Children.RemoveAt(0);
                        ((Border)_src).Child = _elements[0];
                        _elements[0].IsEnabled = true;
                        break;
                }
            });
        }
        public Rect Region
        {
            get
            {
                if (StartPosition == null || EndPosition == null)
                {
                    return Rect.Empty;
                }
                else
                {
                    return new Rect(StartPosition??new Point(), EndPosition??new Point());
                }
            }
        }

        public bool Finish()
        {
            if (!Status.HasFlag(CompletableStatus.Completed))
            {
                Recover();
                Status = CompletableStatus.Canceled;
            }
            return true;
        }

        public Brush BackGroundColor { get; set; } = new SolidColorBrush(Color.FromArgb(100,100,100,100));

        public delegate void CutFinishEvent(Rect region);

        public CutFinishEvent CutFinish { get; set; }

        #region Private Methods
        private bool Down { get; set; }
        private void Init(FrameworkElement container)
        {
            Height = container.ActualHeight;
            Width = container.ActualWidth;
            TopPanel.Height = BottomPanel.Height = Height / 2;
            LeftPanel.Width = RightPanel.Width = Width / 2;


            TopPanel.Background =
                LeftPanel.Background = 
                    RightPanel.Background = 
                        BottomPanel.Background = 
                            BackGroundColor;

            MouseDown += (o,e) =>
            {
                if (e.ChangedButton== MouseButton.Left)
                {
                    Status = CompletableStatus.Working;
                    if (StartPosition == null)
                    {
                        var sp = e.GetPosition(this);
                        if (sp.Y >= 0 &&
                            sp.X >= 0 &&
                            sp.Y <= Height &&
                            sp.X <= Width)
                        {
                            StartPosition = sp;
                            {
                                TopPanel.Height = sp.Y;
                                BottomPanel.Height = Height - sp.Y;
                                LeftPanel.Width = sp.X;
                                RightPanel.Width = Width - sp.Y;
                            }
                            Down = true;
                        }
                    }
                }
            };
            MouseUp += (o, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (Down)
                    {
                        var ep = e.GetPosition(this);
                        if (ep.X >= 0 && ep.Y >= 0 && ep.Y <= Height && ep.X <= Width)
                        {
                            EndPosition = ep;
                            Rect r = new Rect(StartPosition??new Point(), ep);
                            StartPosition = r.TopLeft;
                            EndPosition = r.BottomRight;
                            Down = false;
                            ClipRect.Visibility = Visibility.Visible;
                        }
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

            MouseDown += (o, e) =>
            {
                if (e.ClickCount >= 2 && Region != Rect.Empty)
                {
                    Recover();
                    CutFinish?.Invoke(Region);
                    Status = CompletableStatus.Successful;
                }
            };
            Status = CompletableStatus.Ready;
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

        #endregion

        public CompletableStatus Status { get; private set; } = CompletableStatus.Created;
    }
}
