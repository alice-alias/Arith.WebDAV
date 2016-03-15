using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arith.WebDAV.Server
{
    public enum WebDavStatusCode
    {
        MultiStatus = 207,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        InsufficientStorage = 507,
    }
}
