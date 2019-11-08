using System;
using System.Windows.Input;

namespace KanBan
{
    internal class MoveColumnLeftCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //todo.
        }
    }
}