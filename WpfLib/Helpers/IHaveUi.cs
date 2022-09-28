using System.Windows;
using System.Windows.Media;

namespace WpfLib.Helpers
{
    public interface IHaveUi
    {
        UIElement Subject { get; }
    }

    public interface ICanChangeColor : IHaveUi
    {
        Brush Color { get; set; }
    }
}
