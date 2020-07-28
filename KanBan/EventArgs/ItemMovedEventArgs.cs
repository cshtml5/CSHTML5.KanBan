using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanBan
{
    public class ItemMovedEventArgs : EventArgs
    {
        internal ItemMovedEventArgs(object source)
        {
            Source = source;
        }
        public object Source { get; private set; }
    }
}
