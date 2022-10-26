using System;
using System.Collections.Generic;
using System.Windows;
using HandyControl.Tools;
using SharpDX.DXGI;
using WpfLib.Controls.PenDrawer.Base;
using WpfLib.Controls.PenDrawer.Interface;
using WpfLib.Controls.PenDrawer.Model;
using D2D = SharpDX.Direct2D1;

namespace WpfLib.Controls.PenDrawer
{
    public class DirectXDrawer :DrawerBase
    {
        public DirectXDrawer(Size size, IDrawBehavior.PageDirection direction = IDrawBehavior.PageDirection.Vertical) : base(size,direction)
        { 
        }

        private static D2D.Factory Factory { get; } = new D2D.Factory();
        public override FrameworkElement Canvas  => _canvas; 

        public override IList<StrokeModel> Strokes => throw new NotImplementedException();

        private readonly FrameworkElement _canvas = null;
        private SwapChain _swapChain;

        private void InitializeDeviceResources()
        {
            var backBufferDesc =
                new ModeDescription(ActualWidth, ActualHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            
            //交换链
            var swapChainDesc = new SwapChainDescription
            {
                ModeDescription = backBufferDesc,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = _canvas.GetHandle(),
                IsWindowed = true
            };
     
        }

        public override void Erase(Rect rubber)
        {
            throw new NotImplementedException();
        }

        public override void Erase(int from, int to)
        {
            throw new NotImplementedException();
        }

        public override void Erase(int from)
        {
            throw new NotImplementedException();
        }
    }
}
