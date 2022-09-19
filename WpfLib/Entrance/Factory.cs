using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using WpfLib.Helpers;
using WpfLib.Helpers.Implement;

namespace WpfLib.Entrance
{
    public class Factory
    {
        #region Private Fields
        private static Factory Instance
        {
            get
            {
                if (_instance == null) { lock (Lock) {  _instance ??= new Factory();  } } return _instance;
            }
        }
        private static readonly object Lock = new();
        private static  Factory _instance;
        private WindsorContainer Container { get; } = new();
        private Factory()
        {
            Init();
        }
        #endregion

        private void Init()
        {
            Container.Register(Component
                .For<IInputCatcher>()
                .ImplementedBy<InputCatcher>()
                .LifestyleTransient());

            
        }
        public static T Resolve<T>()
        {
            return Instance.Container.Resolve<T>();
        }
    }
}
