using System.Windows;
using System.Windows.Media;

namespace WpfLib.Controls.PenDrawer
{
    /// <summary>
    /// Usage: 
    /// <code>
    /// using (var dc = DrawingVisualElement.DrawingVisual.RenderOpen()) {
	/// 	dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ActualWidth, ActualHeight));  // Clear the background
    ///     dc.DrawLine(...);
    ///     ...
    /// }
    /// </code>
    /// </summary>
	public class DrawingVisualElement : FrameworkElement {
		public DrawingVisual DrawingVisual { get; }

		public DrawingVisualElement() {
			DrawingVisual = new DrawingVisual();
			AddVisualChild(DrawingVisual);
			AddLogicalChild(DrawingVisual);
		}

		protected override Visual GetVisualChild(int index) => DrawingVisual;

		protected override int VisualChildrenCount => 1;
	}
}
