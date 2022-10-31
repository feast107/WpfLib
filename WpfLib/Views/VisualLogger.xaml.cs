using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Data;
using WpfLib.Helpers;
using WpfLib.Views.ViewModel;
using ScrollViewer = HandyControl.Controls.ScrollViewer;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace WpfLib.Views.ViewModel
{
    public enum LogType : byte
    {
        Log,
        Info,
        Debug,
        Warn,
        Error,
        Fatal,
    }

    #region Definitions

    public class LogViewModel : ViewModelBase
    {
        public Brush HeaderFore
        {
            get => _headerFore;
            set => SetValue(ref _headerFore, value);
        }

        private Brush _headerFore;

        public Color ListBack
        {
            get => _listBack;
            set => SetValue(ref _listBack, value);
        }

        private Color _listBack;

        public Brush HeaderBack
        {
            get => _headerBack;
            set
            {
                SetValue(ref _headerBack, value);
                HeaderBackDefault ??= value;
            }
        }

        private Brush _headerBack;
        public Brush HeaderBackDefault { get; private set; }


        public bool Running { get; set; } = false;

        public string Text
        {
            get => _text;
            set => SetValue(ref _text, value);
        }

        private string _text;

        public ObservableCollection<StackViewModel> Frames { get; } = new();
    }

    public class StackViewModel : ViewModelBase
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
    #endregion
}

namespace WpfLib.Views
{
    /// <summary>
    /// VisualLogger.xaml 的交互逻辑
    /// </summary>
    public partial class VisualLogger : Window, ILogger
    {
        private List<LogViewModel> Debugs { get; } = new();

        class Roller
        {
            public List<LogViewModel> Debugs { get; set; }
            public UniformSpacingPanel DebugList { get; set; }
            public ScrollViewer Scroll { get; set; }
            public int RowMax { get; set; } = 0;
            private int Need { get; set; } = 0;
            private int Calc(int number)
            {
                Cancel.Cancel();
                Cancel = new();
                new Task(() =>
                {
                    Thread.Sleep(1000);
                    if (!Cancel.IsCancellationRequested)
                    {
                        Need = 0;
                    }
                },Cancel.Token).Start();
                if (number >= 10) return Need;
                if (Need == RowMax)
                {
                    Need = 0;
                }
                if (Need == 0)
                {
                    Need = number;
                }
                else
                {
                    Need = 10 * Need + number;
                    if (Need > RowMax) Need = RowMax;
                }
                return Need == 0 ? Need : Need - 1;
            }
            public void Roll(Key key)
            {
                var arg = 0;
                switch (key)
                {
                    case Key.D0:
                    case Key.NumPad0:
                        arg = 0;
                        break;
                    case Key.D1:
                    case Key.NumPad1:
                        arg = 1;
                        break;
                    case Key.D2:
                    case Key.NumPad2:
                        arg = 2;
                        break;
                    case Key.D3:
                    case Key.NumPad3:
                        arg = 3;
                        break;
                    case Key.D4:
                    case Key.NumPad4:
                        arg = 4;
                        break;
                    case Key.D5:
                    case Key.NumPad5:
                        arg = 5;
                        break;
                    case Key.D6:
                    case Key.NumPad6:
                        arg = 6;
                        break;
                    case Key.D7:
                    case Key.NumPad7:
                        arg = 7;
                        break;
                    case Key.D8:
                    case Key.NumPad8:
                        arg = 8;
                        break;
                    case Key.D9:
                    case Key.NumPad9:
                        arg = 9;
                        break;
                    default:
                        Need = 0;
                        return;
                }
                double height = 0;
                var i = 0;
                var count = Calc(arg);
                while (i < count)
                {
                    height += ((Expander)DebugList.Children[i]).ActualHeight;
                    i++;
                }
                Scroll.ScrollToVerticalOffsetWithAnimation(height);
                new Task(() =>
                {
                    Thread.Sleep(500);
                    Debugs[count].HeaderBack = Brushes.LightYellow;
                    Thread.Sleep(1500);
                    Debugs[count].HeaderBack = Debugs[count].HeaderBackDefault;
                }).Start();
            }
            private CancellationTokenSource Cancel { get; set; } = new();
        }

        private Roller Roll { get; set; }= new();

        public VisualLogger()
        {
            InitializeComponent();
            KeyUp += (o, e) =>
            {
                switch (e.Key)
                {
                    case Key.LeftCtrl:
                        {
                            foreach (Expander debugListChild in DebugList.Children)
                            {
                                debugListChild.IsExpanded = false;
                            }

                            break;
                        }
                    case Key.Escape:
                        Close();
                        break;
                    case Key.Down:
                    case Key.PageDown:
                    case Key.S:
                        ScrollViewer.ScrollToVerticalOffsetWithAnimation(ScrollViewer.ScrollableHeight);
                        break;
                    case Key.Up:
                    case Key.PageUp:
                    case Key.W:
                        ScrollViewer.ScrollToVerticalOffsetWithAnimation(0);
                        break;
                }
                Roll.Roll(e.Key);
            };
            Roll.Scroll = ScrollViewer;
            Roll.DebugList = DebugList;
            Roll.Debugs = Debugs;
        }

        private int Count
        {
            get;
            set;
        } = 1;

        #region 取堆栈顶下一层

        public void Log(object message)
        {
            Log(message, new StackTrace(true).GetFrames(1));
        }

        public void Debug(object message)
        {
            Debug(message, new StackTrace(true).GetFrames(1));
        }

        public void Info(object message)
        {
            Info(message, new StackTrace(true).GetFrames(1));
        }

        public void Warn(object message)
        {
            Warn(message, new StackTrace(true).GetFrames(1));
        }

        public void Error(object message)
        {
            Error(message, new StackTrace(true).GetFrames(1));
        }

        public void Fatal(object message)
        {
            Fatal(message, new StackTrace(true).GetFrames(1));
        }

        #endregion

        private void Log(object message, List<StackFrame> frames)
        {
            Debug(message, frames);
        }

        #region 传递

        private void Debug(object message, List<StackFrame> frames)
        {
            Dispatcher.Invoke(() =>
            {
                Get(message, frames, LogType.Debug);
            });
        }

        private void Info(object message, List<StackFrame> frames)
        {
            Dispatcher.Invoke(() =>
            {
                Get(message, frames, LogType.Info);
            });
        }

        private void Warn(object message, List<StackFrame> frames)
        {
            Dispatcher.Invoke(() =>
            {
                Get(message, frames, LogType.Warn);
            });
        }

        private void Error(object message, List<StackFrame> frames)
        {
            Dispatcher.Invoke(() =>
            {
                Get(message, frames, LogType.Error);
            });
        }

        private void Fatal(object message, List<StackFrame> frames)
        {
            Dispatcher.Invoke(() =>
            {
                Get(message, frames, LogType.Fatal);
            });
        }

        #endregion

        #region Privates

        private void Get(object message, List<StackFrame> frames, LogType type)
        {
            Dispatcher.Invoke(() =>
            {
                var vm = new LogViewModel
                {
                    Text = Count++ + ". " + message,
                    HeaderFore = FindHeaderFore(type),
                    ListBack = FindListBack(type),
                    HeaderBack = FindHeaderBack(type)
                };
                var ex = FindExpander();
                var i = 1;
                frames.ForEach(x =>
                {
                    vm.Frames.Add(new StackViewModel()
                    {
                        Method = i++ + ". " + x.GetMethod().ToString(),
                        FilePath = x.GetFileName(),
                        Row = x.GetFileLineNumber()
                    });
                });
                ex.DataContext = vm;
                FindListBox(ex).DataContext = vm;
                Roll.RowMax++;
                Debugs.Add(vm);
                DebugList.Children.Add(ex);
            });
        }
        #endregion

        #region EventHandlers
        private void DoubleClickTextBox(object sender, MouseButtonEventArgs e)
        {
                
            Growl.ClearGlobal();
            try
            {
                Clipboard.SetText(((TextBox)sender).Text);
                Growl.SuccessGlobal(new GrowlInfo() { ShowDateTime = false, Message = "复制成功" });
            }
            catch
            {
                Growl.ErrorGlobal(new GrowlInfo(){ShowDateTime = false,Message = "复制失败，剪切板不可访问"});
            }
        }
        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ((RunningBlock)sender).IsRunning = true;
        }
        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ((RunningBlock)sender).IsRunning = false;
        }
        #endregion

        #region Resources
        private Expander FindExpander()
        {
            return (Expander)this.FindResource("Content");
        }
        private static ListBox FindListBox(Expander expander)
        {
            return (ListBox)(expander.Content);
        }
        private Brush FindHeaderFore(LogType type)
        {
            return type switch
            {
                LogType.Debug => Brushes.Gray,
                LogType.Info => (Brush)FindResource("InfoBrush"),
                LogType.Warn => (Brush)FindResource("DarkWarningBrush"),
                LogType.Error => (Brush)FindResource("DangerBrush"),
                LogType.Fatal => Brushes.White,
                _ => null
            };
        }
        private Brush FindHeaderBack(LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                case LogType.Info:
                case LogType.Debug:
                case LogType.Warn:
                case LogType.Error:
                    return Brushes.GhostWhite;
                case LogType.Fatal:
                    return (Brush)FindResource("ReverseTextBrush");
            }
            return null;
        }
        private Color FindListBack(LogType type)
        {
            return type switch
            {
                LogType.Debug => Brushes.CornflowerBlue.Color,
                LogType.Info => Brushes.DarkTurquoise.Color,
                LogType.Warn => Brushes.BurlyWood.Color,
                LogType.Error => Brushes.IndianRed.Color,
                LogType.Fatal => Brushes.Gray.Color,
                _ => Brushes.Transparent.Color
            };
        }
        #endregion

        private void OnRunningBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            new Task(() =>
            {
                Thread.Sleep(100);
                Dispatcher.Invoke(() =>
                {
                    ((RunningBlock)sender).IsRunning = true;
                    ((RunningBlock)sender).IsRunning = false;
                });
            }).Start();
        }
    }
    public static class StackExtension
    {
        public static List<StackFrame> GetFrames(this StackTrace trace, int deep)
        {
            var st = trace.GetFrames();
            return deep < st?.Length ? st?.ToList().GetRange(deep, st.Length - deep) : new List<StackFrame>();
        }
    }
}
