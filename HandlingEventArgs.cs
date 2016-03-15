using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arith.WebDAV
{
    public abstract class HandlingEventArgs : EventArgs
    {
        public bool Handled { get; set; }

        public static void Invoke<T>(IEnumerable<EventHandler<T>> handlerCollection, object sender, T args)
            where T : HandlingEventArgs
        {
            foreach (var h in handlerCollection)
            {
                if (args.Handled) return;
                h(sender, args);
            }
        }
    }
}
