using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace KanBan
{
    public class EditKanBanColumnChildWindow : ChildWindow
    {
        public EditKanBanColumnChildWindow()
        {
            OKCommand = new ButtonOKCommand(this, true);
            CancelCommand = new ButtonOKCommand(this, false);
        }

        public ButtonOKCommand OKCommand;
        public ButtonOKCommand CancelCommand;
    }
    public class ButtonOKCommand : ICommand
    {
        EditKanBanColumnChildWindow _childWindow = null;
        bool _isOKButton = true;
        internal ButtonOKCommand(EditKanBanColumnChildWindow childWindow, bool isOkButton)
        {
            _childWindow = childWindow;
            _isOKButton = isOkButton;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _childWindow.DialogResult = _isOKButton;
        }
    }
}
