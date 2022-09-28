using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfLib.Helpers
{
    public interface IHaveBackground : ICanChangeColor
    {
        Brush Background { get; set; }
    }
    public abstract class BackgroundBase : IHaveBackground
    {
        public abstract UIElement Subject { get; }
        public Brush Color { get => Background; set => Background = value;
        }
        public abstract Brush Background { get; set; }
    }
    public class PanelBackground : BackgroundBase
    {
        public readonly Panel Panel;
        public PanelBackground(Panel panel)
        {
            Panel = panel;
        }

        public override UIElement Subject => Panel;
        public override Brush Background { get => Panel.Background; set => Panel.Background = value; }
    }
    public class BorderBackground : BackgroundBase
    {
        public readonly Border Border;
        public BorderBackground(Border border)
        {
            Border = border;
        }

        public override UIElement Subject => Border;
        public override Brush Background { get => Border.Background; set => Border.Background = value; }
    }
    public class ControlBackground : BackgroundBase
    {
        public readonly Control Control;
        public ControlBackground(Control contentControl)
        {
            Control = contentControl;
        }

        public override UIElement Subject => Control;
        public override Brush Background { get => Control.Background; set => Control.Background = value; }
    }

    public class TextBlockBackground : BackgroundBase
    {
        public override UIElement Subject => TextBlock;

        public override Brush Background
        {
            get => TextBlock.Background;
            set => TextBlock.Background = value;
        }

        public readonly TextBlock TextBlock;

        public TextBlockBackground(TextBlock element)
        {
            TextBlock = element;
        }
    }
}
