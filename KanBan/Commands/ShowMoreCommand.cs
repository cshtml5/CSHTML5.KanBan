using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanBan
{
    internal class ShowMoreCommand : ICommand
    {
        ColumnViewModel _column = null;
        internal ShowMoreCommand(ColumnViewModel column)
        {
            _column = column;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            int newAmountOfItemsToDisplay = _column.ItemsToDisplay + _column.ColumnDefinition._kanBanControl.PageSize;
            int totalAmountOfItems = _column.UnalteredItemsCollection.Count;
            if (newAmountOfItemsToDisplay > totalAmountOfItems)
            {
                newAmountOfItemsToDisplay = totalAmountOfItems;
            }
            _column.ItemsToDisplay = newAmountOfItemsToDisplay;
        }
    }
}
