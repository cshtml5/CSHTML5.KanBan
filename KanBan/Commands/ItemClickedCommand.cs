using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanBan
{
    public class ItemClickedCommand : ICommand
    {
        ItemViewModel _item = null;
        internal ItemClickedCommand(ItemViewModel item)
        {
            _item = item;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            KanBanControl kanBanControl = _item._kanBanControl;
            kanBanControl.OnItemClicked(_item, parameter);
        }
    }
}
