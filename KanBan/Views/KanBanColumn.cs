using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KanBan
{
    public class KanBanColumn : Control
    {
        /// <summary>
        /// The KanBanControl that contains this KanBanColumn. It is mostly used by ColumnViewModel to apply changes on the elements of the Column to the KanbanControl.
        /// </summary>
        internal KanBanControl _kanBanControl = null;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //DragDropTarget<ItemsControl,UIElement> dragDropTarget = GetTemplateChild("PART_DragDropTarget") as DragDropTarget<ItemsControl, UIElement>;
            //if(dragDropTarget != null)
            //{
            //    //register to the events needed to handle drag and drop.
            //}
            //else
            //{
            //    //if the developper allowed drag and drop, show a message that says he needs a PanelDragDropTarget called "PART_ItemsControlDragDropTarget" in his DataTemplate applied on the Column.
            //}
        }

        /// <summary>
        /// The header of the column. It can be a string, an UIElement or an object
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(KanBanColumn), new PropertyMetadata(null, Header_Changed));
        private static void Header_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the template to apply on the Header object.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the HeaderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(KanBanColumn), new PropertyMetadata(null, HeaderTemplate_Changed));
        private static void HeaderTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the identifier of the column, which will be used to determine which items of the KanBanControl will be put in this Column.
        /// </summary>
        public object Id
        {
            get { return (object)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
        /// <summary>
        /// Identifies the Id dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(object), typeof(KanBanColumn), new PropertyMetadata(null, Id_Changed));
        private static void Id_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Tell the KanBanControl to move the items from the "Unclassified" Column into this one
            //throw new NotImplementedException();
        }

        private void PanelDragDropTarget_ItemDragStarting(object sender, ItemDragEventArgs e)
        {
            //PanelDragDropTarget p = (PanelDragDropTarget)sender;
            //StackPanel stackPanel = (StackPanel)p.Content;
            //TextBlock textBlock = (TextBlock)p.FindName(p.Name + "TextBlock");
            //DragDropListBox.Items.Insert(0, "Event ItemDragStarting triggered on target " + textBlock.Text);
        }

        private void PanelDragDropTarget_ItemDragCompleted(object sender, ItemDragEventArgs e)
        {
            //PanelDragDropTarget p = (PanelDragDropTarget)sender;
            //StackPanel stackPanel = (StackPanel)p.Content;
            //TextBlock textBlock = (TextBlock)p.FindName(p.Name + "TextBlock");
            //DragDropListBox.Items.Insert(0, "Event ItemDragCompleted triggered on target " + textBlock.Text);
        }
    }
}
