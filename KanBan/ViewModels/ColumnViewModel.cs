using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public ICommand CommandDeleteColumn { get; set; }



        public ICommand OpenContextMenuColumnModification_Command = null;






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
                    ColumnDefinition.Id = dummyColumn.Id; //todo: redraw everything if id is changed
                    //force a refresh of the KanBanControl:
                    var columns = ColumnDefinition._kanBanControl.Columns;
                    ColumnDefinition._kanBanControl.Columns = null;
                    ColumnDefinition._kanBanControl.Columns = columns;
                }
            }
        }
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
                CreateAndAddMenuItem(contextMenu, menuItemStyleAsStyle, "Edit column", _column.MenuItemEditColumn_Click);
                CreateAndAddMenuItem(contextMenu, menuItemStyleAsStyle, "Delete column", _column.MenuItemDeleteColumn_Click);

                //MenuItem menuItem = new MenuItem();
                //menuItem.Content = "Rename column"; //todo: change this to "Edit column" and make the subsequent changes (for example renaming the methodcalled on click on this MenuItem).
                //menuItem.Click += _column.MenuItemRenameColumn_Click;
                //menuItem.Tag = contextMenu; //so we can close it when we click on the menuItems.
                //if (isMenuItemStyleFound)
                //{
                //    menuItem.Style = (Style)menuItemStyle;
                //}
                //contextMenu.Items.Add(menuItem);

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
}
