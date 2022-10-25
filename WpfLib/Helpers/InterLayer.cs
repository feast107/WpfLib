using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfLib.Helpers
{
    /// <summary>
    /// 用来表示能导出子元素的接口
    /// </summary>
    public interface IHaveElement
    {
        /// <summary>
        /// 挂在外层节点的钩子
        /// </summary>
        public FrameworkElement Hook { get; }
        IList<UIElement> MoveOut();
        void MoveIn(IList<UIElement> elements);
    }
    
    /// <summary>
    /// 多级容器基类
    /// </summary>
    public abstract class DetachableElement : IHaveElement
    {
        protected DetachableElement(FrameworkElement hook)
        {
            Hook = hook;
        }
        public FrameworkElement Hook { get; }
        public abstract IList<UIElement> MoveOut();
        public abstract void MoveIn(IList<UIElement> elements);
    }

    #region 针对Panel、Border、ContentControl封装的容器层级
    public sealed class PanelLayer<TPanel> : DetachableElement where TPanel : Panel
    {
        private readonly TPanel _panel;
        public PanelLayer(TPanel panel):base(panel)
        {
            _panel = panel;
        }
        public PanelLayer(TPanel panel,FrameworkElement hook) : base(hook)
        {
            _panel = panel;
        }
        public override IList<UIElement> MoveOut()
        {
            List<UIElement> ret = new();
            while (_panel.Children.Count > 0)
            {
                ret.Add(_panel.Children[0]);
                _panel.Children.RemoveAt(0);
            }
            return ret;
        }
        public override void MoveIn(IList<UIElement> elements)
        {
            elements.ToList().ForEach(x=>_panel.Children.Add(x));
        }
    }
    public sealed class BorderLayer<TBorder> : DetachableElement where TBorder : Border
    {
        private readonly TBorder _border;
        public BorderLayer(TBorder border):base(border)
        {
            _border = border;
        }
        public BorderLayer(TBorder border, FrameworkElement hook) : base(border)
        {
            _border = border;
        }
        public override IList<UIElement> MoveOut()
        {
            List<UIElement> ret = new(){_border.Child};
            _border.Child = null;
            return ret;
        }
        public override void MoveIn(IList<UIElement> elements)
        {
            _border.Child = elements[0];
        }
    }
    public sealed class ContentLayer<TContent> : DetachableElement where TContent : ContentControl
    {
        private readonly TContent _content;
        public ContentLayer(TContent content):base(content)
        {
            _content = content;
        }
        public ContentLayer(TContent content,FrameworkElement hook) : base(hook)
        {
            _content = content;
        }
        public override IList<UIElement> MoveOut()
        {
            List<UIElement> ret = new(){ (UIElement)_content.Content};
            _content.Content = null;
            return ret;
        }
        public override void MoveIn(IList<UIElement> elements)
        {
            _content.Content = elements[0];
        }
    }
    #endregion
    
    /// <summary>
    /// 用来表示穿插层的接口
    /// </summary>
    public interface IInterLayer
    {
        IHaveElement Outer { get; }
        IHaveElement Inner { get; }
        bool EnableChildren { get; set; }
        /// <summary>
        /// 复原
        /// </summary>
        void UnMount();
        /// <summary>
        /// 挂载，将穿插层插入
        /// </summary>
        void Mount();
    }
    
    /// <summary>
    /// 夹层实现类
    /// </summary>
    public sealed class InterLayer : IInterLayer
    {
        public IHaveElement Outer { get; }
        public IHaveElement Inner { get; }
        public bool EnableChildren { get; set; }

        #region Inits
        public InterLayer(Panel outer         , Panel inner         , FrameworkElement hook = null)
        {
            Outer = new PanelLayer<Panel>(outer);
            Inner = new PanelLayer<Panel>(inner,hook);
        }
        public InterLayer(Panel outer         , ContentControl inner, FrameworkElement hook = null)
        {
            Outer = new PanelLayer<Panel>(outer);
            Inner = new ContentLayer<ContentControl>(inner,hook);
        }
        public InterLayer(Panel outer         , Border inner        , FrameworkElement hook = null)
        {
            Outer = new PanelLayer<Panel>(outer);
            Inner = new BorderLayer<Border>(inner, hook);
        }

        public InterLayer(ContentControl outer, Panel inner         , FrameworkElement hook = null)
        {
            Outer = new ContentLayer<ContentControl>(outer);
            Inner = new PanelLayer<Panel>(inner, hook);
        }
        public InterLayer(ContentControl outer, ContentControl inner, FrameworkElement hook = null)
        {
            Outer = new ContentLayer<ContentControl>(outer);
            Inner = new ContentLayer<ContentControl>(inner, hook);
        }
        public InterLayer(ContentControl outer, Border inner        , FrameworkElement hook = null)
        {
            Outer = new ContentLayer<ContentControl>(outer);
            Inner = new BorderLayer<Border>(inner, hook);
        }

        public InterLayer(Border outer        , Panel inner         , FrameworkElement hook = null)
        {
            Outer = new BorderLayer<Border>(outer);
            Inner = new PanelLayer<Panel>(inner, hook);
        }
        public InterLayer(Border outer        , ContentControl inner, FrameworkElement hook = null)
        {
            Outer = new BorderLayer<Border>(outer);
            Inner = new ContentLayer<ContentControl>(inner, hook);
        }
        public InterLayer(Border outer        , Border inner        , FrameworkElement hook = null)
        {
            Outer = new BorderLayer<Border>(outer);
            Inner = new BorderLayer<Border>(inner, hook);
        }
        #endregion

        public void UnMount()
        {
            Outer.MoveOut();
            var els = Inner.MoveOut();
            foreach (UIElement element in els)
            {
                element.IsEnabled = false;
            }
            Outer.MoveIn(els);
        }
        public void Mount()
        {
            var els = Outer.MoveOut();
            foreach (UIElement element in els)
            {
                element.IsEnabled = EnableChildren;
            }
            Inner.MoveIn(els);
            Outer.MoveIn(new List<UIElement>(){Inner.Hook});
        }
    }
}
