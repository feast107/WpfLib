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
        private Factory()
        {
            Init();
        }
        #endregion
        private void Init()
        {
         
        }
       
    }
}
