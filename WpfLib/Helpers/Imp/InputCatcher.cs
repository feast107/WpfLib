using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WpfLib
{
    
}
namespace WpfLib.Helpers.Imp
{
    interface IInterface
    {
        //[Obsolete]
        //static abstract void Target润time不支持StatAbsMemberInInterfaces([CallerMemberName]string n);
    }
    
    public class InputCatcher : IInput
    {
        private static readonly Dictionary<MouseButton, Input> ButtonMap = new ()
        {
            { MouseButton.Left ,   Input.LeftButton   },
            { MouseButton.Right ,  Input.RightButton  },
            { MouseButton.Middle , Input.MiddleButton },
        };
        private static readonly Dictionary<Key, Input> KeyMap = new ()
        {
            { Key.A ,         Input.A         },
            { Key.LeftCtrl ,  Input.LeftCtrl  },
            { Key.RightCtrl , Input.RightCtrl },
            { Key.Space ,     Input.Space     },
            { Key.Enter ,     Input.Enter     },
            { Key.CapsLock ,  Input.CapsLock  },
            { Key.LeftAlt ,   Input.LeftAlt   },
            { Key.ExSel ,     Input.ExSel     },
            { Key.LeftShift , Input.LeftShift }
        };

        private readonly Dictionary<Key, Input> _myKeyMap = new ();
        private readonly Dictionary<MouseButton, Input> _myButtonMap = new ();

        private IList<FrameworkElement> Targets { get; } = new List<FrameworkElement>();
        /// <summary>
        /// 当前输入
        /// </summary>
        public Input Inputs { get; private set; }

        /// <summary>
        /// 启用
        /// </summary>
        /// <returns></returns>
        public InputCatcher Enable(FrameworkElement target)
        {
            target.MouseDown += MouseDown;
            target.MouseUp += MouseUp;
            target.KeyDown += KeyDown;
            target.KeyUp += KeyUp;
            return this;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <returns></returns>
        public InputCatcher Disable(FrameworkElement target)
        {
            target.MouseDown -= MouseDown;
            target.MouseUp -= MouseUp;
            target.KeyDown -= KeyDown;
            target.KeyUp -= KeyUp;
            return this;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Inputs = Input.Nothing;
        }

        public InputCatcher()
        {
            Thread.Start();
        }

        #region Private Fields
        private Task Thread { get; set; } = new Task(() => { });
        private Input GetKey(Key key)
        {
            if (KeyMap.TryGetValue(key, out var ret))
            {
                return ret;
            } 
            if(_myKeyMap.TryGetValue(key, out ret))
            {
                return ret;
            }
            return Input.Nothing;
        }
        private Input GetMouse(MouseButton button)
        {
            if (ButtonMap.TryGetValue(button, out var ret))
            {
                return ret;
            }
            if (_myButtonMap.TryGetValue(button, out ret))
            {
                return ret;
            }
            return Input.Nothing;
        }

        private bool AddFlag(Input input)
        {
            if (!Inputs.Is(input))
            {
                Inputs += (ulong)input;
                return true;
            }
            return false;
        }
        private bool RemoveFlag(Input input)
        {
            if (Inputs.Is(input))
            {
                Inputs -= (ulong)input;
                return true;
            }
            return false;
        }

        private void KeyDown(object sender, KeyEventArgs events)
        {
            var input = GetKey(events.Key);
            if(AddFlag(input))
            {
                Happen(Inputs, input, true, events);
            }
        }
        private void KeyUp(object sender, KeyEventArgs events)
        {
            var input = GetKey(events.Key);
            if (RemoveFlag(input))
            {
                Happen(Inputs, input, true, events);
            }
        }
        private void MouseDown(object sender, MouseButtonEventArgs events)
        {
            var input = GetMouse(events.ChangedButton);
            if (AddFlag(input))
            {
                Happen(Inputs, input, true, events);
            }
        }
        private void MouseUp(object sender, MouseButtonEventArgs events)
        {
            var input = GetMouse(events.ChangedButton);
            if (RemoveFlag(input))
            {
                Happen(Inputs, input, false, events);
            }
        }

        private void Happen(Input all, Input change, bool active, EventArgs eventArgs)
        {
            Thread = Thread.ContinueWith((t) =>
            {
                InputChange(all, change, active, eventArgs);
            },TaskContinuationOptions.ExecuteSynchronously);
        }
        #endregion

        /// <summary>
        /// 输入更变事件
        /// </summary>
        public IInput.InputChangeEvent InputChange { get; set; }

        /// <summary>
        /// 主动添加字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public InputCatcher Optional(Key key, Input value)
        {
            _myKeyMap.Add(key, value);
            return this;
        }
        /// <summary>
        /// 主动添加字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public InputCatcher Optional(MouseButton key, Input value)
        {
            _myButtonMap.Add(key, value);
            return this;
        }

        public void DeOptional(Key key)
        {
            _myKeyMap.Remove(key);
        }
        public void DeOptional(MouseButton button)
        {
            _myButtonMap.Remove(button);
        }

        public IInput Attach(FrameworkElement target)
        {
            if (!Targets.Contains(target))
            {
                Targets.Add(target);
                Enable(target);
            }
            return this;
        }
        public IInput Detach(FrameworkElement target)
        {
            if(Targets.Remove(target))
            {
                Disable(target);
            }
            return this;
        }
        public bool Attached(FrameworkElement target)
        {
            return Targets.Contains(target);
        }
    }
}
