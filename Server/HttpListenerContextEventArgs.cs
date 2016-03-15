using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arith.WebDAV.Server
{
    public class HttpListenerContextEventArgs : EventArgs
    {
        public HttpListenerContext Context { get; private set; }
        public HttpListenerContextEventArgs(HttpListenerContext context)
        {
            Context = context;
        }
    }
}
