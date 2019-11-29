using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace KanBan
{
    internal class ItemDropCommand : ICommand
    {
        ColumnViewModel _column = null;
        internal ItemDropCommand(ColumnViewModel column)
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
            //Version using Drop event:
            var param = parameter as DragEventArgs;
            if (param != null)
            {
                var castedParameter = param.Data.GetData("ItemDragEventArgs") as ItemDragEventArgs;

                int newItemOrder = -1;

                //We get the item on which the element was dropped:
                var elements = VisualTreeHelper.FindElementsInHostCoordinates(param.GetPosition(null).Position, _column.ColumnDefinition._kanBanControl);
                FrameworkElement elementDroppedOn = elements.ElementAt(0) as FrameworkElement;
                while (elementDroppedOn != null && //we go up the visual tree until we reached the root or...
                    (elementDroppedOn.DataContext == null || elementDroppedOn.DataContext.GetType() != typeof(ItemViewModel))) //...until we found an element that has the ItemViewModel as DataContext
                {
                    elementDroppedOn = elementDroppedOn.Parent as FrameworkElement;
                }
                if (elementDroppedOn != null)
                {
                    //todo: find a way to know in which half of the DropElement the dragged element was dropped.
                    //      We could do with comparing the position at which it was dropped with the position of the root of the item card
                    //          (but it need us to find said root - could be done by going up the visual tree until we find an element whose DataContext is not an ItemViewModel
                    //          -> the last one with the ItemViewModel should be the root element)
                    //      We could try to know it the cursor was on the Header or the body of the card.


                    //for now, we will consider the element dropped above:
                    newItemOrder = ((ItemViewModel)elementDroppedOn.DataContext).ItemOrder + 1; //Reminder: we have chosen to have the items with the bigger order number higher on the displayed list.
                }

                //Version using ItemDragCompleted event (this one doesn't seem good because the sender is the column from which the item comes):
                //var castedParameter = parameter as ItemDragEventArgs;
                //if(castedParameter != null)
                //{
                SelectionCollection selectionCollection = castedParameter.Data as SelectionCollection;
                if (selectionCollection.Count > 0)
                {
                    Selection selection = selectionCollection.ElementAt(0); //we can only drag one item so it is at the position 0.
                    FrameworkElement container = selection.Item as FrameworkElement;
                    ItemViewModel itemViewModel = container.DataContext as ItemViewModel;
                    if (itemViewModel != null)
                    {
                        itemViewModel.ItemOrder = newItemOrder;
                        UpdateItemOrders(newItemOrder);

                        itemViewModel.ItemColumnId = _column.ColumnDefinition.Id;

                        //force a refresh of the KanBanControl:
                        //todo: find a way to only refresh the Column itself.
                        var kanBanControl = _column.ColumnDefinition._kanBanControl;
                        var columns = kanBanControl.Columns;
                        kanBanControl.Columns = null;
                        kanBanControl.Columns = columns;

                        //keep the following to only refresh the column itself. It cannot be used properly currently as it causes a bug where the items are duplicated.
                        ////refresh the elements of the column:
                        //_column.Dispatcher.BeginInvoke( () =>
                        //{
                        //    _column.UnalteredItemsCollection.Add(itemViewModel); //we add the item manually because the whole collection was not refreshed in the process. This should change with Smooth refresh.
                        //    _column.SetOrderedItemsCollection();
                        //    });
                        //_column.SetOrderedItemsCollection();
                    }
                }
            }
        }

        void UpdateItemOrders(int addedItemItemOrder)
        {
            var itemsToUpdate = _column.Items.OrderBy(item => item.ItemOrder); //note: we use the currently displayed items only because the element could only be dragged on the Visible items. We also order them by ascending order, that way we can go through them and only increment those that need it (if there is a gap in the ItemOrders, we can stop incrementing the next ones)
            foreach (ItemViewModel item in itemsToUpdate)
            {
                if (item.ItemOrder == addedItemItemOrder)
                {
                    ++addedItemItemOrder;
                    ++item.ItemOrder;
                }
                else if (item.ItemOrder > addedItemItemOrder)
                {
                    break;
                }
            }
        }
    }
}
