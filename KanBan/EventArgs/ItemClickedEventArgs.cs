using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanBan
{
    public class ItemClickedEventArgs : EventArgs
    {
        internal ItemClickedEventArgs(object source, object parameter = null)
        {
            Source = source;
            Parameter = parameter;
        }
        public object Source { get; private set; }
        public object Parameter { get; private set; }
    }
}
