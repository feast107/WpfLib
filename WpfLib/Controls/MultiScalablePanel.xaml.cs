using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfLib.Controls
{
    /// <summary>
    /// 等间距等大小可缩放容器
    /// </summary>
    public partial class MultiScalablePanel
    {
        /// <summary>
        /// 初始化尺寸
        /// </summary>
        public MultiScalablePanel()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 内单元大小
        /// </summary>
        public Size Size { get; set; } = new Size(100,100);
        /// <summary>
        /// 缩放尺度
        /// </summary>
        public double Scale { get; set; } = 1.1;
        /// <summary>
        /// 内间距
        /// </summary>
        public double Spacing
        {
            get => SpacingPanel.Spacing;
            set
            {
                if ((int)SpacingPanel.Spacing != (int)value)
                {
                    SpacingPanel.Spacing = value;
                    if (_showed)
                    {
                        Resize();
                    }
                }
            }
        }

        /// <summary>
        /// 控制伴随滚轮的缩放键，默认为LeftCtrl
        /// </summary>
        public Key ZoomKey { get; set; } = Key.LeftCtrl;
        public void Add(FrameworkElement control)
        {
            control.RenderSize = Size;
            control.Width = Size.Width;
            control.Height = Size.Height;
            control.MouseDown += OnItemMouseDown;
            lock (_renderLock)
            {
                Children.Add(control);
                if (_showed && RealColumnCount < CalcColumnCount)
                {
                    Resize();
                }
            }
        }
        public bool Remove(FrameworkElement control)
        {

            if (Contains(control))
            { 
                control.MouseDown -= OnItemMouseDown;
                lock (_renderLock)
                {
                    Children.Remove(control);
                    if (Children.Count < RealColumnCount) { Resize(); }
                }
                return true;
            } 
            return false;
        }
        public bool Contains(FrameworkElement control)
        {
            return Children.Contains(control);
        }

        #region Privates Fields
        private readonly object _renderLock = new();
        private UIElementCollection Children => SpacingPanel.Children;
        /// <summary>
        /// 由尺寸计算出的列数量
        /// </summary>
        private int CalcColumnCount { get; set; }
        /// <summary>
        /// 相与数量后真实的列数量
        /// </summary>
        private int RealColumnCount { get; set; }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.NewSize.Width < Size.Width)
            {
                Size = new Size(sizeInfo.NewSize.Width - 2 * Spacing , sizeInfo.NewSize.Width/Size.Width*Size.Height);
            }
            if (sizeInfo.NewSize != sizeInfo.PreviousSize)
            {
                Resize();
            }
            _showed = true;
        }
        private void Init()
        {
            ScrollViewer.IsInertiaEnabled = true;
            SpacingPanel.Width = Size.Width;
            KeyDown += (o, e) =>
            {
                Console.WriteLine(e);
            };
            SpacingPanel.MouseWheel += (o, e) =>
            {
                if (!Keyboard.IsKeyDown(ZoomKey)) return;

                ScrollViewer.CanMouseWheel = false;
                var p = e.GetPosition(SpacingPanel);
                if (e.Delta > 0)
                {
                    if ( (Size.Width + Spacing * 2) * Scale < ActualWidth)
                    {
                        Size = new Size(Size.Width * Scale, Size.Height * Scale);
                        Resize();
                        SilentScroll(p.Y * Scale - e.GetPosition(ScrollViewer).Y);
                    }
                }
                else
                {
                    Size = new Size(Size.Width / Scale, Size.Height / Scale);
                    Resize();
                    SilentScroll(p.Y / Scale - e.GetPosition(ScrollViewer).Y);
                }
                ScrollViewer.CanMouseWheel = true;
            };
        }
       
        private bool _showed;
        private void Resize()
        {
            RealColumnCount 
                = CalcColumnCount 
                    = (int)(ActualWidth / (Size.Width + Spacing * 2));

            if(CalcColumnCount > Children.Count)
                RealColumnCount = Children.Count == 0 ? 1 : Children.Count;
            SpacingPanel.Width = Size.Width * RealColumnCount + (RealColumnCount - 1) * Spacing;


            foreach (FrameworkElement child in Children)
            {
                child.RenderSize = Size;
                child.Width = Size.Width;
                child.Height = Size.Height;
            }
        }
        private void ZoomMax()
        {
            double height = ActualHeight - 2 * Spacing;
            double width = ActualWidth - 2 * Spacing;
            double tmpW = height / Size.Height * Size.Width;
            if (tmpW > width)
            {
                height = width / Size.Width * Size.Height;
            }
            else
            {
                width = tmpW;
            }
            Size = new Size(width, height);
        }
        private void OnItemMouseDown(object o,MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left &&
                e.ClickCount == 2)
            {
                ZoomMax();
                int index = 0;
                while (index < Children.Count)
                {
                    if (Children[index].Equals(o))
                    {
                        break;
                    }
                    index++;
                }
                Resize();
                double offSet = index * (Size.Height + Spacing);
                SilentScroll(offSet);
            }
        }
        private void SilentScroll(double offset)
        {
            if (AutoInertiaEnabler != null)
            {
                AutoInertiaEnabler.Stop();
            }
            ScrollViewer.IsInertiaEnabled = false;
            ScrollViewer.ScrollToVerticalOffset(offset);
            AutoInertiaEnabler = new Timer() { Interval = 300,AutoReset = false};
            AutoInertiaEnabler.Elapsed += (o, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ScrollViewer.IsInertiaEnabled = true;
                });
            };
            AutoInertiaEnabler.Start();
        }

        private Timer AutoInertiaEnabler { get; set; }
        #endregion
    }
}
