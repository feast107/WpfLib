using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfLib.Helpers;

namespace WpfLib.Effects
{
    public class Hover
    {
        public Hover(Panel element)
        {
            _subject = new PanelBackground(element);
            Init();
        }
        public Hover(Border element)
        {
            _subject = new BorderBackground(element);
            Init();
        }
        public Hover(Control element)
        {
            _subject = new ControlBackground(element);
            Init();
        }
        public Hover(TextBlock element,bool foreOrBack = true)
        {
            if (foreOrBack)
            {
                _subject = new TextBlockForeground(element);
            }
            else
            {
                _subject = new TextBlockBackground(element);
            }
            Init();
        }
        public Hover(ButtonBase element)
        {
            _subject = new ButtonBaseForeground(element);
            Init();
        }

        /// <summary>
        /// 需要变换的颜色
        /// </summary>
        public Color Transform { get; set; } = System.Windows.Media.Color.FromArgb(0x00, 0xff, 0xff, 0xff);

        /// <summary>
        /// 持续时间
        /// </summary>
        public int Duration { get; set; } = 300;
       
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                if (value != _isEnable)
                {
                    if (value)
                    {
                        _subject.Subject.MouseEnter += OnMouseEnter;
                        _subject.Subject.MouseLeave += OnMouseLeave;
                    }
                    else
                    {
                        _subject.Subject.MouseEnter -= OnMouseEnter;
                        _subject.Subject.MouseLeave -= OnMouseLeave;
                    }
                    _isEnable = value;
                }
            }
        }

        #region Private fields
        private Brush Color
        {
            get
            {
                if (_subject.Color == null)
                {
                    _subject.Color = new SolidColorBrush();
                }
                else if(_subject.Color.IsFrozen)
                {
                    _subject.Color = new SolidColorBrush(((SolidColorBrush)_subject.Color).Color);
                }
                return _subject.Color;
            }
            set => _subject.Color = value;
        }
        private Brush Origin { get; set; }
        private readonly ICanChangeColor _subject;
        private ColorAnimation Animation { get; set; }
        private bool _isEnable;
        private void Init()
        {
            IsEnable = true;
        }

        private void InitAnimation()
        {
            Animation = new()
            {
                To = Transform,
                Duration = new Duration(TimeSpan.FromMilliseconds(Duration)),
                AutoReverse = false,
                RepeatBehavior = new RepeatBehavior(1)
            };
            Storyboard.SetTarget(Animation, _subject.Color);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(SolidColorBrush.ColorProperty));
        }
        private Storyboard Story { get; set; } = new();
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Origin = _subject.Color?.CloneCurrentValue();
            if (Animation == null)
            {
               InitAnimation();
            }
            Animation.From = ((SolidColorBrush)Color).Color;
            Color.BeginAnimation(SolidColorBrush.ColorProperty, Animation);
            Story.Children.Add(Animation);
        }
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Story.Stop();
            Story = new();
            InitAnimation();
            Color = Origin?.CloneCurrentValue();
        }
        #endregion
    }
}
