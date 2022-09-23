using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WpfLib.Controls.Definition;
using WpfLib.Helpers;

namespace WpfLib.Controls
{
    /// <summary>
    /// 用于控件区域截取的控件,左键拖拽，右键取消，双击完成截图
    /// </summary>
    public partial class Mask
    {
        #region Private Fields
        private IInterLayer Layer { get; }
        private Point MoveLast { get; set; } 
        private bool Down { get; set; }
        private bool Drag { get; set; } 
        #endregion
        
        public Point? StartPosition { get; private set; }
        public Point? EndPosition { get; private set; }
        
        public Mask(Panel container)
        {
            InitializeComponent();
            Layer = new InterLayer(container, Container, this);
            Layer.Mount();
            Init(container);
        }
        public Mask(ContentControl container)
        {
            InitializeComponent();
            Layer = new InterLayer(container, Container, this);
            Layer.Mount();
            Init(container);
        }
        public Mask(Border container)
        {
            InitializeComponent();
            Layer = new InterLayer(container, Container, this);
            Layer.Mount();
            Init(container);
        }

        private FrameworkElement Outer => Layer.Outer.Hook;
     
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
            if (!Status.Is(Status.Completed))
            {
                Recover();
                Status = Status.Canceled;
            }
            return true;
        }
        public Brush MaskColor { get; set; } = new SolidColorBrush(Color.FromArgb(100,100,100,100));

        public delegate void CutFinishEvent(Rect region);
        public CutFinishEvent CutFinish { get; set; }

        #region Private Methods
     
        private void Init(FrameworkElement container)
        {
            Height = container.ActualHeight;
            Width = container.ActualWidth;
            TopPanel.Height = BottomPanel.Height = Height / 2;
            LeftPanel.Width = RightPanel.Width = Width / 2;

            TopPanel.Background =
                LeftPanel.Background = 
                    RightPanel.Background = 
                        BottomPanel.Background = MaskColor;
       
            StatusChange += (s) =>
            {
                if (s.Is(Status.Completed))
                {
                    Outer.MouseMove -= Mousemove;
                }
            };

            void Mousemove(object o ,MouseEventArgs e)
            {
                if (Down)
                {
                    var ep = e.GetPosition(this);
                    if (ep.X >= 0 &&
                        ep.Y >= 0 &&
                        ep.Y <= Height &&
                        ep.X <= Width)
                    {
                        EndPosition = ep;
                        var sp = StartPosition;
                        if (sp != null)
                        {
                            TopPanel.Height = Region.Top;
                            BottomPanel.Height = Height - Region.Bottom;
                            LeftPanel.Width = Region.Left;
                            RightPanel.Width = Width - Region.Right;
                        }
                    }
                }
                else
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (StartPosition == null)
                        {
                            Status = Status.Working;
                            SetStart(e.GetPosition(this));
                        }
                    }
                }
            }
            void Mouseup(object o, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (Down)
                    {
                        SetEnd(e.GetPosition(this));
                    }
                }
            }
            MouseUp    += Mouseup;
            Outer.MouseMove += Mousemove;
            MouseLeave += (o, e) =>
            {
                if (Down)
                {
                    if (!new Rect(new Point(0, 0), this.RenderSize).Contains(e.GetPosition(this)))
                    {
                        Mouseup(o, new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
                    }
                }
            };
            MouseDoubleClick += (o, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (Region != Rect.Empty)
                    {
                        Recover();
                        CutFinish?.Invoke(Region);
                        Status = Status.Successful;
                    }
                    if (Status != Status.Working)
                    {
                        Status = Status.Working;
                        SetStart(new Point(0,0));
                        SetEnd(new Point(Width,Height));
                    }
                }
            };

            void DragUp(object o, MouseButtonEventArgs e)
            {
                Drag = false;
                e.MouseDevice.SetCursor(Cursors.Arrow);
                MoveLast = new Point();
            }
            ClipRect.MouseUp    += DragUp;
            ClipRect.MouseDown  += (o, e) =>
            {
                e.MouseDevice.SetCursor(Cursors.Hand);
                MoveLast = e.GetPosition(this);
                Drag = true;
            };
            ClipRect.MouseMove  += (o, e) =>
            {
                if (Drag)
                {
                    var now = e.GetPosition(this);
                    double horizonOffset = now.X - MoveLast.X;
                    double verticalOffset = now.Y - MoveLast.Y;
                    if (LeftPanel.Width + horizonOffset >= 0 && 
                        RightPanel.Width - horizonOffset >=0)
                    {
                        LeftMove(horizonOffset);
                        RightMove(horizonOffset);
                    }
                    if (TopPanel.Height + verticalOffset >= 0 && 
                        BottomPanel.Height - verticalOffset >= 0)
                    {
                        TopMove(verticalOffset);
                        BottomMove(verticalOffset);
                    }
                    MoveLast = now;
                }
            };
            ClipRect.MouseLeave += (o, e) =>
            {
                DragUp(o, new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
            };
            
            Status = Status.Ready;
        }
        private void SetStart(Point sp)
        {
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
                    RightPanel.Width = Width - sp.X;
                }
                Down = true;
            }
        }
        private void SetEnd(Point ep)
        {
            ep.X = ep.X > Width ? Width : ep.X < 0 ? 0 : ep.X;
            ep.Y = ep.Y > Height ? Height : ep.Y < 0 ? 0 : ep.Y;
            EndPosition = ep;
            var r = new Rect(StartPosition ?? new Point(), ep);
            var sp = StartPosition = r.TopLeft;
            EndPosition = ep = r.BottomRight;

            TopPanel.Height = (double)sp?.Y;
            BottomPanel.Height = Height - ep.Y;
            LeftPanel.Width = (double)sp?.X;
            RightPanel.Width = Width - ep.X;

            Down = false;
            ClipRect.Visibility = Visibility.Visible;
        }
        private void Recover()
        {
            Layer.EnableChildren = true;
            Dispatcher.Invoke(() =>
            {
                Layer.UnMount();
            });
        }
        private void LeftMove(double offset)
        {
            var sp = StartPosition ?? new Point();
            var ep = EndPosition ?? new Point();
            if (sp.X + offset >= 0)
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
            if (ep.X + offset <= ActualWidth)
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
            if (sp.Y + offset >= 0)
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
            if (ep.Y + offset <= ActualHeight)
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

    }

    public partial class Mask : IStatusMachine
    {
        private Status _status = Status.Created;
        public Status Status
        {
            get => _status;
            private set
            {
                if (value != _status)
                {
                    _status = value;
                    StatusChange?.Invoke(_status);
                }
            }
        }
        public IStatusMachine.StatusChangeEvent StatusChange { get; set; }
    }
}
