using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KanBan
{
    public class KanBanControl : Control
    {
        KanBanViewModel _kanBanViewModel;

        public KanBanControl()
        {
            Columns = new Collection<KanBanColumn>();
            _kanBanViewModel = new KanBanViewModel(this);
            DataContext = _kanBanViewModel;
        }


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(KanBanControl), new PropertyMetadata(null, ItemsSource_Changed));

        private static void ItemsSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo.
            //do nothing for now, redraw when supported.
            KanBanControl kanBanControl = (KanBanControl)d;
            kanBanControl.HandleColumnsOrItemsSourceChanged();
        }


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(KanBanControl), new PropertyMetadata(null, ItemTemplate_Changed));

        private static void ItemTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //we force a Refresh:
            KanBanControl kanBanControl = (KanBanControl)d;
            var columns = kanBanControl.Columns;
            kanBanControl.Columns = null;
            kanBanControl.Columns = columns;
        }

        /// <summary>
        /// Gets or sets the Columns in the KanBanControl.
        /// </summary>
        public Collection<KanBanColumn> Columns
        {
            get { return (Collection<KanBanColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        /// <summary>
        /// Identifies the ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(Collection<KanBanColumn>), typeof(KanBanControl), new PropertyMetadata(null, OnColumns_Changed));
        private static void OnColumns_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
                KanBanControl kanBanControl = (KanBanControl)d;
            if (kanBanControl._kanBanViewModel != null) //I think we can remove this test since kanBanControl._kanBanViewModel is set in the constructor of KanBanControl
            {
                kanBanControl.HandleColumnsOrItemsSourceChanged();

                if(e.OldValue is ObservableCollection<KanBanColumn>)
                {
                    ((ObservableCollection<KanBanColumn>)e.OldValue).CollectionChanged -= kanBanControl.KanBanControl_ColumnsChanged;
                }
                if (kanBanControl.Columns is ObservableCollection<KanBanColumn>)
                {
                    ((ObservableCollection<KanBanColumn>)kanBanControl.Columns).CollectionChanged -= kanBanControl.KanBanControl_ColumnsChanged;
                    ((ObservableCollection<KanBanColumn>)kanBanControl.Columns).CollectionChanged += kanBanControl.KanBanControl_ColumnsChanged;
                }

                //IEnumerable<KanBanColumn> columns = (IEnumerable<KanBanColumn>)e.NewValue;
                //List<ColumnViewModel> columnViewModels = new List<ColumnViewModel>();
                //foreach (KanBanColumn column in columns)
                //{
                //    columnViewModels.Add(new ColumnViewModel(column, kanBanControl.ItemTemplate));
                //}
                ////todo: add a column for unclassified elements

                //kanBanControl._kanBanViewModel.Columns = columnViewModels;
                //kanBanControl.SetItemsInViewModelColumns(columnViewModels);
            }
        }


        public DataTemplate ColumnHeaderTemplate //should this be plural? (ColumnsHeaderTemplate)
        {
            get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
            set { SetValue(ColumnHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderTemplateProperty =
            DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(KanBanControl), new PropertyMetadata(null, ColumnHeaderTemplate_Changed));

        private static void ColumnHeaderTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: refresh
        }

        private void KanBanControl_ColumnsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HandleColumnsOrItemsSourceChanged();
        }

        void HandleColumnsOrItemsSourceChanged()
        {
            //todo: make a different one for when we activate the smooth mode.
            IEnumerable<KanBanColumn> columns = (IEnumerable<KanBanColumn>)Columns;
            if (columns != null)
            {
                ObservableCollection<ColumnViewModel> columnViewModels = new ObservableCollection<ColumnViewModel>();
                foreach (KanBanColumn column in columns)
                {
                    column._kanBanControl = this;
                    columnViewModels.Add(new ColumnViewModel(column, ItemTemplate));
                }
                //todo: add a column for unclassified elements

                _kanBanViewModel.Columns = null;
                _kanBanViewModel.Columns = columnViewModels;
                SetItemsInViewModelColumns(columnViewModels);
            } //todo: else put everything in an "UNCLASSIFIED" column
        }

        void SetItemsInViewModelColumns(ObservableCollection<ColumnViewModel> columns)
        {
            Dictionary<object, ColumnViewModel> quickColumnsFromId = new Dictionary<object, ColumnViewModel>();
            Dictionary<object, IEnumerable<ItemViewModel>> listFromId = new Dictionary<object, IEnumerable<ItemViewModel>>();
            ObservableCollection<ItemViewModel> remainingItems = new ObservableCollection<ItemViewModel>();
            foreach (ColumnViewModel column in columns)
            {
                column.Items.Clear();
                quickColumnsFromId.Add(column.ColumnDefinition.Id, column);
                listFromId.Add(column.ColumnDefinition.Id, new ObservableCollection<ItemViewModel>());
            }
            if (ItemsSource != null)
            {
                foreach (Object item in ItemsSource)
                {
                    object id;
                    //todo: follow the ColumnMemberPath on the item, then add the item to the corresponding columnViewModel
                    //...
                    ItemViewModel itemViewModel = new ItemViewModel(item);
                    Binding binding = new Binding(ColumnMemberPath); //for now, we consider that the ColumnMemberPath is directly accessible
                    binding.Source = item;
                    binding.Mode = BindingMode.TwoWay; //todo: see if we want this, it will allow us to easily change the column via the ItemViewModel.
                    itemViewModel.SetBinding(ItemViewModel.ItemColumnIdProperty, binding);
                    id = itemViewModel.ItemColumnId;
                    //todo: handle the case where the Binding is broken.

                    if (id != null && listFromId.ContainsKey(id))
                    {
                        ((ObservableCollection<ItemViewModel>)listFromId[id]).Add(itemViewModel);
                    }
                    else
                    {
                        remainingItems.Add(itemViewModel);
                    }
                }

                foreach (object id in listFromId.Keys)
                {
                    quickColumnsFromId[id].Items = (ObservableCollection<ItemViewModel>)listFromId[id];
                }
            }
        }

        public Style ColumnHeaderStyle { get; set; } //should this be plural ? ("ColumnsHeaderStyle")

        public KanBanViewModel ViewModel { get; set; }
        public string ColumnMemberPath { get; set; }

        public string OrderMemberPath { get; set; }

        //note: we might want the following properties to be DependencyProperties for Bindings.
        public bool CanUserReorderColumns { get; set; }
        public bool CanUserCreateModifyAndDeleteColumns { get; set; }
        public bool CanUserDragBetweenColumns { get; set; }
        public bool CanUserDragToUnclassifiedColumn { get; set; }
        public bool CanUserReorderItems { get; set; }

        /// <summary>
        /// The number of cards that will be displayed at once, and the number of cards that will be added when clicking on the "Show more..." button.
        /// </summary>
        public int PageSize { get; set; } //Number of cards, not an actual size.


        //props:


        //bool EnableSmoothRefresh
        //int PageSize //Number of cards, not an actual size.
        //string UniqueIdentifierMemberPath

        //events:
        //ItemMoved
        //ItemOrderChanged
        //ItemClicked
        //ColumnsEdited
        //ColumnHeaderClicked


        //Created:
        //props:
        //KanBanColumn Columns 
        //string ColumnMemberPath //where to find the property in the items to know in which column to put them.
        //string OrderMemberPath
        //bool CanUserRenameColumns
        //bool CanUserCreateModifyAndDeleteColumns
        //bool CanUserDragBetweenColumns
        //bool CanUserDragToUnclassifiedColumn
        //bool CanUserReorderItems
        //Style ColumnsHeaderStyle
        //DataTemplate ColumnsHeaderTemplate
    }
}
