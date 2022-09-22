using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WpfLib.Helpers;

namespace WpfLib.Controls
{
    /// <summary>
    /// ErasePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ErasePanel : UserControl
    {
        private CursorHelper Helper { get; set; }
        private Cursor CurrentCursor { get; set; }
        public ErasePanel()
        {
            InitializeComponent();
        }

        private UIElementCollection Children => Controls.Children;
        private void SetDevice(MouseDevice device)
        {
            if (Helper == null)
            {
                Helper = new(device);
            }
        }
        private void InitEvents()
        {
            Controls.MouseEnter += (o, e) =>
            {
                SetDevice(e.MouseDevice);
                if (CurrentCursor != null)
                {
                    Helper.SetCursor(CurrentCursor);
                }
            };
            Controls.MouseMove += (o, e) =>
            {
                SetDevice(e.MouseDevice);
            };
            Controls.MouseLeave += (o, e) =>
            {
                Helper.Recover();
            };

            MouseDown += (o, e) =>
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    Helper.Recover();
                }
            };

            VerySmall.Click += (o, e) =>
            {
                Helper.SetCursor(Cursors.None);
            };
        }
    }
}
