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

namespace WpfLib.Controls
{
    /// <summary>
    /// InvisibleTab.xaml 的交互逻辑
    /// </summary>
    public partial class InvisibleTab : HandyControl.Controls.TabItem
    {
        public InvisibleTab()
        {
            InitializeComponent();
        }
        public UIElementCollection Children => Container.Children;
        private new object Content { get; set; }
    }
}
