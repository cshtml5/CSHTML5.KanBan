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
            ItemDrop_Command = new ItemDropCommand(this);
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

        public ICommand ItemDrop_Command;
    }
}
