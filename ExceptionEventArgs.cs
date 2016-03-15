using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arith.WebDAV
{
    public class ExceptionEventArgs : HandlingEventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception exception) { Exception = exception; }
    }
}
