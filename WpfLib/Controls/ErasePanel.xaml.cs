using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfLib.Controls.Definition;
using WpfLib.Helpers;

namespace WpfLib.Controls
{
    /// <summary>
    /// ErasePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ErasePanel 
    {
        #region Private fields
        private IInterLayer InterLayer { get; }
        private CursorHelper Helper { get; set; }
        private Cursor CurrentCursor { get; set; }
        #endregion

        public FrameworkElement Source => InterLayer.Outer.Hook;

        public ErasePanel(Panel container)
        {
            InitializeComponent();
            InterLayer = new InterLayer(container, Container, this);
            Init();
        }
        public ErasePanel(ContentControl container)
        {
            InitializeComponent();
            InterLayer = new InterLayer(container, Container, this);
            Init();
        }
        public ErasePanel(Border container)
        {
            InitializeComponent();
            InterLayer = new InterLayer(container, Container, this);
            Init();
        }

        #region Private methods
        private void Init()
        {
            InterLayer.Mount();
            InitEvents();
            Status = Status.Ready;
        }
        private void SetDevice(MouseDevice device)
        {
            Helper ??= new(device);
        }
        private void InitEvents()
        {
            void MouseEnter(object o, MouseEventArgs e)
            {
                SetDevice(e.MouseDevice);
                if (CurrentCursor != null)
                {
                    Helper.SetCursor(CurrentCursor);
                }
            }
            void MouseMove (object o, MouseEventArgs e)
            {
                SetDevice(e.MouseDevice);
            }
            void MouseLeave(object o, MouseEventArgs e)
            {
                Helper?.Recover();
            }
            void MouseDown (object o, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    if (Status.Is(Status.Working))
                    {
                        Status = Status.Ready;
                        Source.MouseMove -= EraserMouseMove;
                        Eraser.Visibility = Visibility.Collapsed;
                        Helper.SetCursor(CurrentCursor = null);
                    }else
                    {
                        Helper?.Recover();
                        Finish();
                        Status = Status.Completed;
                    }
                    
                }
            }

            Source.MouseEnter += MouseEnter;
            Source.MouseMove  += MouseMove;
            Source.MouseLeave += MouseLeave;
            Source.MouseDown  += MouseDown;
            StatusChange += (s) =>
            {
                if (s.Is(Status.Completed))
                {
                    Source.MouseEnter -= MouseEnter;
                    Source.MouseMove -= MouseMove;
                    Source.MouseLeave -= MouseLeave;
                    Source.MouseDown -= MouseDown;
                    Helper?.Recover();
                }
            };
           
            void Working()
            {
                Status = Status.Working;
                Helper?.SetCursor(CurrentCursor ??= Cursors.None);
            }
            void Move(Point point)
            {
                Eraser.Margin = new Thickness(
                    point.X - Eraser.Width / 2,
                    point.Y - Eraser.Height / 2,
                    Outer.ActualWidth - point.X - Eraser.Width / 2,
                    Outer.ActualHeight - point.Y - Eraser.Height / 2);
            }
            void EraserMouseMove(object o, MouseEventArgs e)
            {
                var p = Mouse.GetPosition(Outer);
                Move(p);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    OnErase?.Invoke(
                        new Rect(
                            new Point(Eraser.Margin.Left, Eraser.Margin.Top),
                            new Size(Eraser.Width, Eraser.Height)
                        ), e.MouseDevice);
                }
            }
            void ClickAndWidth(object o, RoutedEventArgs e, double width)
            {
                if (Status.Is(Status.Ready))
                {
                    Working();
                    Eraser.Width = Eraser.Height = width;
                    Move(Mouse.GetPosition(Outer));
                    Eraser.Visibility = Visibility.Visible;
                    Source.MouseMove += EraserMouseMove;
                }
                else if(Status.Is(Status.Working))
                {
                    Eraser.Width = Eraser.Height = width;
                }
            }
            
            VerySmall.Click += (o, e) =>
            {
                ClickAndWidth(o, e, 8);
            };
            Small.Click     += (o, e) =>
            {
                ClickAndWidth(o, e, 12);
            };
            Medium.Click    += (o, e) =>
            {
                ClickAndWidth(o, e, 16);
            };
            Large.Click     += (o, e) =>
            {
                ClickAndWidth(o, e, 20);
            };
            Status = Status.Ready;
        }
        #endregion

        public delegate void EraseEvent(Rect region,MouseDevice device);

        public EraseEvent OnErase { get; set; }
    }
    public partial class ErasePanel : IStatusMachine
    {
        private Status _status;
        public Status Status
        {
            get => _status;
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChange?.Invoke(_status);
                }
            }
        }
        public bool Finish()
        {
            InterLayer.UnMount();
            return true;
        }
        public IStatusMachine.StatusChangeEvent StatusChange { get; set; }
    }
}
