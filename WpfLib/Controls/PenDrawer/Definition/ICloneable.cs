using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfLib.Controls.PenDrawer.Definition
{
    public interface ICloneable<T>
    {
        T From(T from);
    }
}
