using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using WpfLib.Annotations;

namespace WpfLib
{
    public class Command : ICommand
    {
        public Action<object> Action { get; }
        public Command(Action<object> action)
        {
            Action = action;
        }
        public void Execute(object parameter)
        {
            Action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (property == null ? value == null : property.Equals(value)) return;
            property = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
