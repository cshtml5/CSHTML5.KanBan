using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace KanBan
{
    public class ColumnViewModel : DependencyObject, INotifyPropertyChanged
    {
        public ColumnViewModel(KanBanColumn column, DataTemplate itemTemplate)
        {
            ColumnDefinition = column;
            ItemTemplate = itemTemplate;
            OpenContextMenuColumnModification_Command = new OpenContextMenuColumnModificationCommand(this);
            ItemDragCompleted_Command = new ItemDragCompletedCommand(this);
            ShowMore_Command = new ShowMoreCommand(this);
        }

        private bool _isUnclassifiedColumn = false;
        internal bool IsUnclassifiedColumn
        {
            get { return _isUnclassifiedColumn; }
            set
            {
                _isUnclassifiedColumn = value;
                ShowColumnEditionButton = false; //The Unclassified column cannot be altered.
            }
        }

        private bool _showColumnEditionButton = true;

        public bool ShowColumnEditionButton
        {
            get { return _showColumnEditionButton; }
            set
            {
                _showColumnEditionButton = value;
                OnPropertyChanged("ShowColumnEditionButton");
            }
        }


        private KanBanColumn _columnDefinition;
        public KanBanColumn ColumnDefinition
        {
            get { return _columnDefinition; }
            set
            {
                _columnDefinition = value;
                HeaderTemplate = value.HeaderTemplate;
                OnPropertyChanged("KanBanColumn");
            }
        }

        //private Style _headerStyle;
        //public Style HeaderStyle
        //{
        //    get { return _headerStyle; }
        //    set { _headerStyle = value; OnPropertyChanged("HeaderStyle"); }
        //}

        private DataTemplate _headerTemplate;
        public DataTemplate HeaderTemplate
        {
            get { return _headerTemplate; }
            set { _headerTemplate = value; OnPropertyChanged("HeaderTemplate"); }
        }



        //public DataTemplate HeaderTemplate
        //{
        //    get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        //    set { SetValue(HeaderTemplateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty HeaderTemplateProperty =
        //    DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ColumnViewModel), new PropertyMetadata(null, HeaderTemplate_Changed));

        //private static void HeaderTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //todo
        //}

        private DataTemplate _itemTemplate;
        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set { _itemTemplate = value; OnPropertyChanged("ItemTemplate"); }
        }

        private int _itemsToDisplay;
        internal int ItemsToDisplay
        {
            get { return _itemsToDisplay; }
            set
            {
                _itemsToDisplay = value;
                DisplayShowMoreButton = _itemsToDisplay < UnalteredItemsCollection.Count;
                SetItemsCollection();
            }
        }

        private bool _displayShowMoreButton;
        public bool DisplayShowMoreButton
        {
            get { return _displayShowMoreButton; }
            set { _displayShowMoreButton = value; OnPropertyChanged("DisplayShowMoreButton"); }
        }


        private Collection<ItemViewModel> _unalteredItemsCollection = new Collection<ItemViewModel>(); //todo: For Smooth refresh, we will want this to be an ObservableCollection.
        internal Collection<ItemViewModel> UnalteredItemsCollection
        {
            get { return _unalteredItemsCollection; }
            set
            {
                _unalteredItemsCollection = value;
                ItemsToDisplay = ColumnDefinition._kanBanControl.PageSize;
                SetOrderedItemsCollection();
            }
        }

        private Collection<ItemViewModel> _orderedItemsCollection;
        private Collection<ItemViewModel> OrderedItemsCollection
        {
            get { return _orderedItemsCollection; }
            set { _orderedItemsCollection = value; SetItemsCollection(); }
        }

        internal void SetOrderedItemsCollection()
        {
            OrderedItemsCollection = new Collection<ItemViewModel>(UnalteredItemsCollection.OrderByDescending(item => item.ItemOrder).ToList());
        }

        /// <summary>
        /// This method sets the Items collection with only an amount of items equal to ItemsToDisplay.
        /// </summary>
        void SetItemsCollection()
        {
            //Should we Items.Clear() here? It shouldn't be necessary since there won't be any reference to the former Collection anyway.
            //todo: Unregister from the CollectionChanged on the former Collection once it will be used for Smooth refresh.
            if (OrderedItemsCollection != null)
            {
                Items = new ObservableCollection<ItemViewModel>(OrderedItemsCollection.Take(ItemsToDisplay).ToArray<ItemViewModel>());
            }
        }

        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public ICommand CommandDeleteColumn { get; set; }

        public ICommand OpenContextMenuColumnModification_Command = null;

        public ICommand ShowMore_Command = null;




        ICommand MoveColumnLeft_Command = new MoveColumnLeftCommand();
        ICommand MoveColumnRight_Command;
        ICommand ModifyColumn_Command;
        ICommand DeleteColumn_Command;


        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        #region Columns edition
        internal void MenuItemEditColumn_Click(object sender, RoutedEventArgs e)
        {
            var childWindow = new EditKanBanColumnChildWindow();
            KanBanColumn dummyColumn = new KanBanColumn() { Header = ColumnDefinition.Header, Id = ColumnDefinition.Id };
            childWindow.DataContext = dummyColumn;
            childWindow.Closed += EditKanBanColumnChildWindow_Closed;
            childWindow.Show();

            ((ContextMenu)((MenuItem)sender).Tag).IsOpen = false;
        }

        internal void MenuItemDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            ((ContextMenu)((MenuItem)sender).Tag).IsOpen = false; //closes the ContextMenu - might not be needed here since we remove the corresponding button.

            var columns = ColumnDefinition._kanBanControl.Columns;
            ColumnDefinition._kanBanControl.Columns = null;
            columns.Remove(ColumnDefinition); //todo: if we force Columns to be an ObservableCollection, we can remove all the other lines of this method except the first
            ColumnDefinition._kanBanControl.Columns = columns;
        }

        private void EditKanBanColumnChildWindow_Closed(object sender, EventArgs e)
        {
            EditKanBanColumnChildWindow cw = (EditKanBanColumnChildWindow)sender;
            if (cw.DialogResult == true)
            {
                KanBanColumn dummyColumn = (KanBanColumn)cw.DataContext;
                ColumnDefinition.Header = dummyColumn.Header;
                if (ColumnDefinition.Id != dummyColumn.Id)
                {
                    ColumnDefinition.Id = dummyColumn.Id;
                    //force a refresh of the KanBanControl:
                    var columns = ColumnDefinition._kanBanControl.Columns;
                    ColumnDefinition._kanBanControl.Columns = null;
                    ColumnDefinition._kanBanControl.Columns = columns;
                }
            }
        }

        internal void MenuItemMoveColumnLeft_Click(object sender, RoutedEventArgs e)
        {
            MoveColumn(MoveDirection.Left);
            ((ContextMenu)((MenuItem)sender).Tag).IsOpen = false;
        }
        internal void MenuItemMoveColumnRight_Click(object sender, RoutedEventArgs e)
        {
            MoveColumn(MoveDirection.Right);
            ((ContextMenu)((MenuItem)sender).Tag).IsOpen = false;
        }

        /// <summary>
        /// Moves the column of this.ColumnDefinition in the direction given as parameter.
        /// </summary>
        /// <param name="moveDirection">The direction in which to move the column.</param>
        /// <returns>True if the columns was moved successfully, false otherwise (if the column was already all the way in the direction we tried to move it).</returns>
        bool MoveColumn(MoveDirection moveDirection)
        {
            KanBanControl kanBanControl = ColumnDefinition._kanBanControl;
            var columns = kanBanControl.Columns;
            //Get the current index of the column to move:
            int index = columns.IndexOf(this.ColumnDefinition);
            //Make sure we do not try to move the column outside of the possible range:
            if ((int)moveDirection == -1)
            {
                if (index == 0)
                {
                    return false;
                }
            }
            else
            {
                if (index == columns.Count - 1)
                {
                    return false;
                }
            }
            //Move the column:
            columns.RemoveAt(index);
            columns.Insert(index + (int)moveDirection, ColumnDefinition);
            //Force a refresh:
            ColumnDefinition._kanBanControl.Columns = null;
            ColumnDefinition._kanBanControl.Columns = columns;
            return true;
        }
        #endregion

        public ICommand ItemDragCompleted_Command;
    }

    internal class OpenContextMenuColumnModificationCommand : ICommand
    {
        ColumnViewModel _column = null;
        internal OpenContextMenuColumnModificationCommand(ColumnViewModel column)
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
            Button button = parameter as Button;
            if (button != null) //this should never be null but we never know...
            {
                //todo: check which actions are allowed and only show the ContextMenus accordingly.
                //Create the contextMenu for the button: (Note: it was not created directly in the xaml to avoid having a right click from the end-user display all the option when only some should be allowed.

                //todo: I kept it this way for now but it will eventually be better to have the ContextMenu defined in the Xaml with a Binding on the MenuItems' Visibility with the related properties ("CanUserRenameColumns", etc.)
                //      This way, it will be easier for the Developper to create a custom Style.

                ContextMenu contextMenu = new ContextMenu();
                object contextMenuStyle = null;
                if (Application.Current.Resources.TryGetValue("MaterialDesign_ContextMenu_Style", out contextMenuStyle))
                {
                    contextMenu.Style = (Style)contextMenuStyle;
                }
                object menuItemStyle = null;
                bool isMenuItemStyleFound = Application.Current.Resources.TryGetValue("MaterialDesign_ContextMenuItem_Style", out menuItemStyle);
                Style menuItemStyleAsStyle = menuItemStyle as Style;

                //create and add the different menuItems
                var kanBanControl = _column.ColumnDefinition._kanBanControl;
                if (kanBanControl.CanUserCreateModifyAndDeleteColumns)
                {
                    CreateAndAddMenuItem(contextMenu, menuItemStyleAsStyle, "Edit column", _column.MenuItemEditColumn_Click);
                    CreateAndAddMenuItem(contextMenu, menuItemStyleAsStyle, "Delete column", _column.MenuItemDeleteColumn_Click);
                }
                if (kanBanControl.CanUserReorderColumns)
                {
                    CreateAndAddMenuItem(contextMenu, menuItemStyleAsStyle, "Move column left", _column.MenuItemMoveColumnLeft_Click);
                    CreateAndAddMenuItem(contextMenu, menuItemStyleAsStyle, "Move column right", _column.MenuItemMoveColumnRight_Click);
                }

                //set the ContextMenu of the Button and open it.
                button.ContextMenu = contextMenu;
                button.ContextMenu.IsOpen = true;
            }
        }

        void CreateAndAddMenuItem(ContextMenu contextMenu, Style menuItemStyle, object content, RoutedEventHandler onClick)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Content = content; //todo: change this to "Edit column" and make the subsequent changes (for example renaming the methodcalled on click on this MenuItem).
            menuItem.Click += onClick;
            menuItem.Foreground = new SolidColorBrush(Colors.LimeGreen);
            menuItem.Tag = contextMenu; //so we can close it when we click on the menuItems.
            if (menuItemStyle != null)
            {
                menuItem.Style = (Style)menuItemStyle;
            }
            contextMenu.Items.Add(menuItem);
        }
    }

    internal enum MoveDirection : int
    {
        Left = -1,
        Right = 1
    }

    internal class ItemDragCompletedCommand : ICommand
    {
        ColumnViewModel _column = null;
        internal ItemDragCompletedCommand(ColumnViewModel column)
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
