using System;
using System.Windows.Input;

namespace TravellingSalesman.Utils
{
    /// <summary>
    /// This is a delegate command, which was originally taken from:
    /// https://wpftutorial.net/DelegateCommand.html
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Readonly fields
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region ctor
        public DelegateCommand(Action<object> execute)
                       : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region Methods
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
