using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfLib.Helpers
{
    public interface IHaveForeground : ICanChangeColor
    {
        Brush Foreground { get; set; }
    }

    public abstract class ForegroundBase : IHaveForeground
    {
        public abstract UIElement Subject { get; }
        public Brush Color { get => Foreground; set => Foreground = value; }
        public abstract Brush Foreground { get; set; }
    }
    
    public class TextBlockForeground : ForegroundBase
    {
        public override UIElement Subject => _subject;
        public override Brush Foreground { get => _subject.Foreground; set=>_subject.Foreground = value; }
        
        private readonly TextBlock _subject;
        public TextBlockForeground(TextBlock element)
        {
            _subject = element;
        }

    }

    public class ButtonBaseForeground : ForegroundBase
    {
        public override UIElement Subject => _subject;
        public override Brush Foreground { get => _subject.Background; set => _subject.Background = value; }
        private readonly ButtonBase _subject;
        public ButtonBaseForeground(ButtonBase element)
        {
            _subject = element;
        }

    }
}
