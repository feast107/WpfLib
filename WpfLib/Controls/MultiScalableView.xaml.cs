using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfLib.Helpers;

namespace WpfLib.Controls
{
    /// <summary>
    /// MultiScalableView.xaml 的交互逻辑
    /// </summary>
    public partial class MultiScalableView
    {
        /// <summary>
        /// 初始化尺寸
        /// </summary>
        /// <param name="initialSize"></param>
        public MultiScalableView(Size initialSize)
        {
            if (Size.IsEmpty)
            {
                throw new ArgumentException("尺寸不可为空");
            }
            Size = initialSize;
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 内单元大小
        /// </summary>
        public Size Size { get; private set; }
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

        private readonly object _renderLock = new();

        private UIElementCollection Children => SpacingPanel.Children;

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
        public void Remove(FrameworkElement control)
        {
            lock (_renderLock)
            {
                int c = Children.Count;
                Children.Remove(control);
                control.MouseDown -= OnItemMouseDown;
                if (c > Children.Count && Children.Count < RealColumnCount)
                {
                    Resize();
                }
            }
        }

        #region Privates Fields

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
            SpacingPanel.Width = Size.Width;
            SpacingPanel.MouseWheel += (o, e) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (e.Delta > 0)
                    {
                        if ( (Size.Width + Spacing * 2) * Scale < ActualWidth)
                        {
                            Size = new Size(Size.Width * Scale, Size.Height * Scale);
                            Resize();
                        }
                    }
                    else
                    {
                        Size = new Size(Size.Width / Scale, Size.Height / Scale);
                        Resize();
                    }
                }
            };

        }
        private bool _showed;
        private void Resize()
        {
            RealColumnCount 
                = CalcColumnCount 
                    = (int)(ActualWidth / (Size.Width + Spacing * 2));

            if(CalcColumnCount > Children.Count)
                RealColumnCount = Children.Count;
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
                ScrollViewer.ScrollToVerticalOffset(offSet);
            }
        }
        #endregion
    }
}
