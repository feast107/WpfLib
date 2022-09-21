using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Castle.MicroKernel.Registration;
using HandyControl.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace WpfLib.Controls
{
    /// <summary>
    /// MultiScalableView.xaml 的交互逻辑
    /// </summary>
    public partial class MultiScalableView : UserControl
    {
        public MultiScalableView(Size initialSize)
        {
            Size = initialSize;
            InitializeComponent();
            Init();
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
                        if (Size.Width * Scale < ActualWidth)
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

        public Size Size { get; private set; }

        public double Scale { get; set; } = 1.1;

        private UIElementCollection Children => SpacingPanel.Children;

        public void Add(FrameworkElement control)
        {
            control.RenderSize = Size;
            control.Width = Size.Width;
            control.Height = Size.Height;
            Children.Add(control);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.NewSize.Width < Size.Width)
            {
                Size = new Size(sizeInfo.NewSize.Width, sizeInfo.NewSize.Width/Size.Width*Size.Height);
                Resize();
            }
        }

        private void Resize()
        {
            int count = (int)(ActualWidth / Size.Width);
            SpacingPanel.Width = Size.Width * count + (count-1) * SpacingPanel.Spacing;
            foreach (FrameworkElement child in Children)
            {
                child.RenderSize = Size;
                child.Width = Size.Width;
                child.Height = Size.Height;
            }
        }
    }
}
