using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KanBan
{
    public class ItemViewModel : DependencyObject, INotifyPropertyChanged
    {

        public ItemViewModel(object item)
        {
            Item = item;
        }

        private object _item;
        public object Item
        {
            get { return _item; }
            set { _item = value; OnPropertyChanged("Item"); }
        }

        public object ItemColumnId
        {
            get { return (object)GetValue(ItemColumnIdProperty); }
            set { SetLocalValue(ItemColumnIdProperty, value); } //note: calling SetLocalValue instead of SetValue because we never want to remove the Binding.
        }
        public static readonly DependencyProperty ItemColumnIdProperty =
            DependencyProperty.Register("ItemColumnId", typeof(object), typeof(ItemViewModel), new PropertyMetadata());

        public int ItemOrder
        {
            get { return (int)GetValue(ItemOrderProperty); }
            set { SetLocalValue(ItemOrderProperty, value); } //note: calling SetLocalValue instead of SetValue because we never want to remove the Binding.
        }
        public static readonly DependencyProperty ItemOrderProperty =
            DependencyProperty.Register("ItemOrder", typeof(int), typeof(ItemViewModel), new PropertyMetadata());

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
