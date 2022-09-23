using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfLib.Helpers.Implement
{
    interface IInterface
    {
        //[Obsolete]
        //static abstract void Target润time不支持StatAbsMemberInInterfaces([CallerMemberName]string n);
    }
    internal class InputCatcher : IInputCatcher
    {
        private static readonly Dictionary<MouseButton, Inputs> ButtonMap = new ()
        {
            { MouseButton.Left ,   Helpers.Inputs.LeftButton   },
            { MouseButton.Right ,  Helpers.Inputs.RightButton  },
            { MouseButton.Middle , Helpers.Inputs.MiddleButton },
        };
        private static readonly Dictionary<Key, Inputs> KeyMap = new ()
        {
            { Key.A ,         Helpers.Inputs.A         },
            { Key.LeftCtrl ,  Helpers.Inputs.LeftCtrl  },
            { Key.RightCtrl , Helpers.Inputs.RightCtrl },
            { Key.Space ,     Helpers.Inputs.Space     },
            { Key.Enter ,     Helpers.Inputs.Enter     },
            { Key.CapsLock ,  Helpers.Inputs.CapsLock  },
            { Key.LeftAlt ,   Helpers.Inputs.LeftAlt   },
            { Key.ExSel ,     Helpers.Inputs.ExSel     },
            { Key.LeftShift , Helpers.Inputs.LeftShift }
        };

        private readonly Dictionary<Key, Inputs> _myKeyMap = new ();
        private readonly Dictionary<MouseButton, Inputs> _myButtonMap = new ();

        private IList<FrameworkElement> Targets { get; } = new List<FrameworkElement>();
        /// <summary>
        /// 当前输入
        /// </summary>
        public Inputs Inputs { get; private set; }

        /// <summary>
        /// 启用
        /// </summary>
        /// <returns></returns>
        private void Enable(FrameworkElement target)
        {
            target.MouseDown += MouseDown;
            target.MouseUp += MouseUp;
            target.KeyDown += KeyDown;
            target.KeyUp += KeyUp;
        }
        /// <summary>
        /// 禁用
        /// </summary>
        /// <returns></returns>
        private void Disable(FrameworkElement target)
        {
            target.MouseDown -= MouseDown;
            target.MouseUp -= MouseUp;
            target.KeyDown -= KeyDown;
            target.KeyUp -= KeyUp;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Inputs = Inputs.Nothing;
        }

        public InputCatcher()
        {
            Thread.Start();
        }

        #region Private Fields
        private Task Thread { get; set; } = new Task(() => { });
        private Inputs GetKey(Key key)
        {
            if (KeyMap.TryGetValue(key, out var ret))
            {
                return ret;
            } 
            if(_myKeyMap.TryGetValue(key, out ret))
            {
                return ret;
            }
            return Inputs.Nothing;
        }
        private Inputs GetMouse(MouseButton button)
        {
            if (ButtonMap.TryGetValue(button, out var ret))
            {
                return ret;
            }
            if (_myButtonMap.TryGetValue(button, out ret))
            {
                return ret;
            }
            return Inputs.Nothing;
        }

        private bool AddFlag(Inputs inputs)
        {
            if (!Inputs.Is(inputs))
            {
                Inputs += (ulong)inputs;
                return true;
            }
            return false;
        }
        private bool RemoveFlag(Inputs inputs)
        {
            if (Inputs.Is(inputs))
            {
                Inputs -= (ulong)inputs;
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


        private void Happen(Inputs all, Inputs change, bool active, EventArgs eventArgs)
        {
            Thread = Thread.ContinueWith((t) =>
            {
                InputChange?.Invoke(all, change, active, eventArgs);
            },TaskContinuationOptions.ExecuteSynchronously);
        }
        #endregion

        /// <summary>
        /// 输入更变事件
        /// </summary>
        public IInputCatcher.InputChangeEvent InputChange { get; set; }

        /// <summary>
        /// 主动添加字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public InputCatcher Optional(Key key, Inputs value)
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
        public InputCatcher Optional(MouseButton key, Inputs value)
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

        public IInputCatcher Attach(FrameworkElement target)
        {
            if (!Targets.Contains(target))
            {
                Targets.Add(target);
                Enable(target);
            }
            return this;
        }
        public IInputCatcher Detach(FrameworkElement target)
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
