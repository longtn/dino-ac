using System;
using System.Windows.Input;

namespace AC.Base
{
    public class ParamRelayCommand : ICommand
    {
        private Action<object> _action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public ParamRelayCommand(Action<object> action)
        {
            _action = action;
        }

        public ParamRelayCommand()
        {

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
