using System;
using System.Collections.Generic;
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
                object contextMenuStyle = Application.Current.Resources["MaterialDesign_ContextMenu_Style"];

                if (contextMenuStyle != null)
                {
                    contextMenu.Style = (Style)contextMenuStyle;
                }
                object menuItemStyle = Application.Current.Resources["MaterialDesign_ContextMenuItem_Style"];
                bool isMenuItemStyleFound = menuItemStyle != null;
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
}
