using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Arith.WebDAV.Server
{
    public class WebDavMethodEventArgs : HandlingEventArgs
    {
        public HttpListenerContext Context { get; private set; }
        public WebDavMethodEventArgs(HttpListenerContext context)
        {
            Context = context;
        }

        public void SendSimpleResponse(HttpStatusCode statusCode = HttpStatusCode.OK) { SendSimpleResponse((int)statusCode); }
        public void SendSimpleResponse(WebDavStatusCode statusCode) { SendSimpleResponse((int)statusCode); }

        public void SendSimpleResponse(int statusCode)
        {
            SetStatusCode(statusCode);
            Context.Response.Close();
        }


        public void SetStatusCode(HttpStatusCode statusCode) { SetStatusCode((int)statusCode); }
        public void SetStatusCode(WebDavStatusCode statusCode) { SetStatusCode((int)statusCode); }

        public void SetStatusCode(int statusCode)
        {
            Context.Response.StatusCode = statusCode;
            Context.Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(statusCode);
        }

        public int GetDepthHeader()
        {

            int value;
            if (int.TryParse(Context.Request.Headers["Depth"], out value) && (value == 0 || value == 1))
                return value;
            else
                return -1;
        }

        public string GetTimeoutHeader()
        {
            string timeout = Context.Request.Headers["Timeout"];

            if (!String.IsNullOrEmpty(timeout) && !timeout.Equals("infinity") &&
                !timeout.Equals("Infinite, Second-4100000000"))
                return timeout;
            return "Second-345600";
        }

    }
}
