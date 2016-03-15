using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arith.WebDAV.Server
{
    public class WebDavServer : IDisposable
    {
        Dictionary<string, List<EventHandler<WebDavMethodEventArgs>>> handlers = new Dictionary<string, List<EventHandler<WebDavMethodEventArgs>>>();

        protected void AddMethodHandler(string method, EventHandler<WebDavMethodEventArgs> handler)
        {
            if (!handlers.ContainsKey(method))
                handlers[method] = new List<EventHandler<WebDavMethodEventArgs>>();
            handlers[method].Add(handler);
        }

        public HttpListener HttpListener { get; private set; }

        protected bool RemoveMethodHandler(string method, EventHandler<WebDavMethodEventArgs> handler)
        {
            var result = false;
            if (handlers.ContainsKey(method))
                result = handlers[method].Remove(handler);

            if (handlers[method].Count == 0)
                handlers.Remove(method);

            return result;
        }

        public WebDavServer()
        {
            HttpListener = new HttpListener();
        }

        public void Start()
        {
            HttpListener.Start();
            HttpListener.BeginGetContext(HttpListenerGetContextCallback, HttpListener);
        }

        List<EventHandler<ExceptionEventArgs>> exceptionEventHandlerList = new List<EventHandler<ExceptionEventArgs>>();
        public event EventHandler<ExceptionEventArgs> MethodHandlingExceptionOccurred
        {
            add { exceptionEventHandlerList.Add(value); }
            remove { exceptionEventHandlerList.Remove(value); }
        }

        void HttpListenerGetContextCallback(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            if (!listener.IsListening) return;
            var context = listener.EndGetContext(result);

            if (RequestProcessing != null)
                RequestProcessing(this, new HttpListenerContextEventArgs(context));

            var args = new WebDavMethodEventArgs(context);

            try
            {
                if (handlers.ContainsKey(context.Request.HttpMethod))
                    HandlingEventArgs.Invoke(handlers[context.Request.HttpMethod], this, args);
            }
            catch (Exception e)
            {
                var exArgs = new ExceptionEventArgs(e);
                ExceptionEventArgs.Invoke(exceptionEventHandlerList, this, exArgs);
                if (!exArgs.Handled && !SupressMethodHandlingException)
                    throw e;
                Debug.WriteLine(string.Format("An exception has been occurred: {0} - {1}", e.GetType().FullName, e.Message));
            }

            if (!args.Handled)
                args.SendSimpleResponse(HttpStatusCode.MethodNotAllowed);

            if (RequestProcessed != null)
                RequestProcessed(this, new HttpListenerContextEventArgs(context));


            HttpListener.BeginGetContext(HttpListenerGetContextCallback, HttpListener);
        }

        public event EventHandler<HttpListenerContextEventArgs> RequestProcessing;
        public event EventHandler<HttpListenerContextEventArgs> RequestProcessed;

        public bool SupressMethodHandlingException { get; set; }

        public void Stop()
        {
            HttpListener.Stop();
        }

        public void Close()
        {
            HttpListener.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpListener.Abort();
            }
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        ~WebDavServer() { Dispose(false); }

        #region HTTP/1.1 Methods
        public event EventHandler<WebDavMethodEventArgs> Get { add { AddMethodHandler("GET", value); } remove { RemoveMethodHandler("GET", value); } }
        public event EventHandler<WebDavMethodEventArgs> Post { add { AddMethodHandler("POST", value); } remove { RemoveMethodHandler("POST", value); } }
        public event EventHandler<WebDavMethodEventArgs> Put { add { AddMethodHandler("PUT", value); } remove { RemoveMethodHandler("PUT", value); } }
        public event EventHandler<WebDavMethodEventArgs> Head { add { AddMethodHandler("HEAD", value); } remove { RemoveMethodHandler("HEAD", value); } }
        public event EventHandler<WebDavMethodEventArgs> Delete { add { AddMethodHandler("DELETE", value); } remove { RemoveMethodHandler("DELETE", value); } }
        public event EventHandler<WebDavMethodEventArgs> Options { add { AddMethodHandler("OPTIONS", value); } remove { RemoveMethodHandler("OPTIONS", value); } }
        public event EventHandler<WebDavMethodEventArgs> Trace { add { AddMethodHandler("TRACE", value); } remove { RemoveMethodHandler("TRACE", value); } }
        public event EventHandler<WebDavMethodEventArgs> Connect { add { AddMethodHandler("CONNECT", value); } remove { RemoveMethodHandler("CONNECT", value); } }
        #endregion

        #region WebDAV Methods
        public event EventHandler<WebDavMethodEventArgs> Propfind { add { AddMethodHandler("PROPFIND", value); } remove { RemoveMethodHandler("PROPFIND", value); } }
        public event EventHandler<WebDavMethodEventArgs> Proppatch { add { AddMethodHandler("PROPPATCH", value); } remove { RemoveMethodHandler("PROPPATCH", value); } }
        public event EventHandler<WebDavMethodEventArgs> Mkcol { add { AddMethodHandler("MKCOL", value); } remove { RemoveMethodHandler("MKCOL", value); } }
        public event EventHandler<WebDavMethodEventArgs> Copy { add { AddMethodHandler("COPY", value); } remove { RemoveMethodHandler("COPY", value); } }
        public event EventHandler<WebDavMethodEventArgs> Move { add { AddMethodHandler("MOVE", value); } remove { RemoveMethodHandler("MOVE", value); } }
        public event EventHandler<WebDavMethodEventArgs> Lock { add { AddMethodHandler("LOCK", value); } remove { RemoveMethodHandler("LOCK", value); } }
        public event EventHandler<WebDavMethodEventArgs> Unlock { add { AddMethodHandler("UNLOCK", value); } remove { RemoveMethodHandler("UNLOCK", value); } }
        #endregion
    }
}
