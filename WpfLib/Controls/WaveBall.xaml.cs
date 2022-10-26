using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl.Controls;

namespace WpfLib.Controls
{
    /// <summary>
    /// WaveBall.xaml 的交互逻辑
    /// </summary>
    public partial class WaveBall : UserControl
    {
        public WaveBall()
        {
            InitializeComponent();
        }
#nullable enable
        private Sprite? Sprite { get; set; }

        public bool IsShowed { get; private set; }
        public void Show(bool animation = false)
        {
            if (Sprite != null || Parent != null || IsShowed) return;
            Dispatcher.Invoke(() =>
            {
                if (animation)
                {
                    Outer.Children.Remove(Transition);
                    new Task(() =>
                    {
                        Thread.Sleep(100);
                        Dispatcher.Invoke(() =>
                        {
                            Outer.Children.Add(Transition);
                        });
                    }).Start();
                }
                Sprite = Sprite.Show(this);
            });
            IsShowed = true;
        }
        public void Close()
        {
            if (Sprite == null || !IsShowed) return;
            Dispatcher.Invoke(() =>
            {
                Sprite.Close();
            });
            Sprite = null;
            IsShowed = false;
        }

        public Color Color
        {
            get => ((SolidColorBrush)Waver.WaveStroke).Color;
            set => SetColor(value);
        }
        public int Percentage
        {
            get => (int)Waver.Value;
            set
            {
                Dispatcher.Invoke(() =>
                {
                    Waver.Value = value;
                    Label.Content = value + "%";
                });
            }
        }


        public void SetColor(Color color)
        {
            Dispatcher.Invoke(() =>
            {
                Waver.WaveStroke = new SolidColorBrush(Color.FromArgb(0xff, color.R, color.G, color.B));
                Grad1.Color = Color.FromArgb(0x66, color.R, color.G, color.B);
                Grad2.Color = Color.FromArgb(0xff, color.R, color.G, color.B);
            });
        }
    }
}
