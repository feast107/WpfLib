using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfLib.Views
{
    public class DebugViewModel : ViewModelBase
    {
        public string Text { get => _text; set => SetValue(ref _text, value); }
        private string _text;

        public ObservableCollection<FrameViewModel> Frames { get; set; } = new();
    }
    public class FrameViewModel : ViewModelBase
    {
        public string Method
        {
            get => _method;
            set => SetValue(ref _method, value);
        }
        private string _method;

        public string FilePath
        {
            get => _filePath;
            set => SetValue(ref _filePath, value);
        }
        private string _filePath;

        public int Row
        {
            get => _row;
            set => SetValue(ref _row, value);
        }
        private int _row;
    }
    /// <summary>
    /// DebugWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DebugWindow : Window
    {
        public ObservableCollection<DebugViewModel> Debugs = new();
        public DebugWindow()
        {
            InitializeComponent();
            DebugList.DataContext = Debugs;
        }

        public bool PrintStack { get; set; } = true;
        private Expander Get(object message, List<StackFrame> frames)
        {
            var vm = new DebugViewModel() { Text = message.ToString() };
            Expander ex = null;
            Dispatcher.Invoke(() =>
            {
                ex = FindExpander();
                frames.ForEach(x =>
                {
                    vm.Frames.Add(new FrameViewModel()
                    {
                        Method = x.GetMethod().Name,
                        FilePath = x.GetFileName(),
                        Row = x.GetFileLineNumber()
                    });
                });
                ex.DataContext = vm;
                FindListBox(ex).DataContext = vm;
            });
            return ex;
        }

        public void Log(object message, [CallerMemberName] string stack = null)
        {
            Info(message, stack);
        }

        public void Debug(object message, [CallerMemberName] string stack = null)
        {
            var st = new StackTrace(true).GetFrames();
            var l = st?.ToList().GetRange(2, st.Length - 2);
            Dispatcher.Invoke(() =>
            {
                var ex = Get(message, l);
                ((SolidColorBrush)FindListBox(ex).Resources["PrimaryBrush"]).Color = Brushes.CornflowerBlue.Color;
                DebugList.Children.Add(ex);
                ScrollViewer.ScrollToEnd();
            });
        }

        public void Info(object message, [CallerMemberName] string stack = null)
        {
            var st = new StackTrace(true).GetFrames();
            var l = st?.ToList().GetRange(2, st.Length - 2);
            Dispatcher.Invoke(() =>
            {
                var ex = Get(message, l);
                ex.Foreground = (Brush)FindResource("InfoBrush");
                ((SolidColorBrush)FindListBox(ex).Resources["PrimaryBrush"]).Color = Brushes.DarkTurquoise.Color;
                DebugList.Children.Add(ex);
                ScrollViewer.ScrollToEnd();
            });
        }

        public void Warn(object message, [CallerMemberName] string stack = null)
        {
            var st = new StackTrace(true).GetFrames();
            var l = st?.ToList().GetRange(2, st.Length - 2);
            Dispatcher.Invoke(() =>
            {
                var ex = Get(message, l);
                ex.Foreground = (Brush)FindResource("DarkWarningBrush");
                ((SolidColorBrush)FindListBox(ex).Resources["PrimaryBrush"]).Color = Brushes.BurlyWood.Color;
                DebugList.Children.Add(ex);
                ScrollViewer.ScrollToEnd();
            });
        }

        public void Error(object message, [CallerMemberName] string stack = null)
        {
            var st = new StackTrace(true).GetFrames();
            var l = st?.ToList().GetRange(2, st.Length - 2);
            Dispatcher.Invoke(() =>
            {
                var ex = Get(message, l);
                ex.Foreground = (Brush)FindResource("DangerBrush");
                ((SolidColorBrush)FindListBox(ex).Resources["PrimaryBrush"]).Color = Brushes.IndianRed.Color;
                DebugList.Children.Add(ex);
                ScrollViewer.ScrollToEnd();
            });
        }

        public void Fatal(object message, [CallerMemberName] string stack = null)
        {
            var st = new StackTrace(true).GetFrames();
            var l = st?.ToList().GetRange(2, st.Length - 2);
            Dispatcher.Invoke(() =>
            {
                var ex = Get(message, l);
                ex.Foreground = Brushes.White;
                ex.Background = (Brush)FindResource("ReverseTextBrush");
                ((SolidColorBrush)FindListBox(ex).Resources["PrimaryBrush"]).Color = Brushes.Black.Color;
                DebugList.Children.Add(ex);
                ScrollViewer.ScrollToEnd();
            });
        }


        private Expander FindExpander()
        {
            return (Expander)this.FindResource("Content");
        }

        private static ListBox FindListBox(Expander expander)
        {
            return (ListBox)(expander.Content);
        }

        private ContextMenu FindContextMenu()
        {
            return (ContextMenu)FindResource("ContextMenu");
        }
    }

}
