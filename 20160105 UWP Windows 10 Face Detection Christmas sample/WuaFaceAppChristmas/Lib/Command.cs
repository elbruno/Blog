using System;
using System.Windows.Input;

namespace WuaFaceAppChristmas.Lib
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public Command(Action execute, Func<bool> canexecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            _execute = execute;
            _canExecute = canexecute ?? (() => true);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object o)
        {
            return _canExecute();
        }

        public void Execute(object p)
        {
            if (!CanExecute(p))
            {
                return;
            }
            _execute();
        }
    }
}
