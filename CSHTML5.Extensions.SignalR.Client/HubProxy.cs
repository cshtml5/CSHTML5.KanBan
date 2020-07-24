using System;

namespace CSHTML5.Extensions.SignalR.Client
{
    public class HubProxy : IHubProxy
    {
        object _jsHubProxy;
        string _hubName;

        public HubProxy(object jsHubProxy, string hubName)
        {
            _jsHubProxy = jsHubProxy;
            _hubName = hubName;
        }

        /// <summary>
        /// Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        public void Invoke(string method, params object[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Interop.ExecuteJavaScript("$0.invoke($1)", _jsHubProxy, method);
                    break;
                case 1:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2)", _jsHubProxy, method, args[0]);
                    break;
                case 2:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3)", _jsHubProxy, method, args[0], args[1]);
                    break;
                case 3:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3, $4)", _jsHubProxy, method, args[0], args[1], args[2]);
                    break;
                case 4:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3, $4, $5)", _jsHubProxy, method, args[0], args[1], args[2], args[3]);
                    break;
                case 5:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3, $4, $5, $6)", _jsHubProxy, method, args[0], args[1], args[2], args[3], args[4]);
                    break;
                case 6:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3, $4, $5, $6, $7)", _jsHubProxy, method, args[0], args[1], args[2], args[3], args[4], args[5]);
                    break;
                case 7:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3, $4, $5, $6, $7, $8)", _jsHubProxy, method, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    break;
                case 8:
                    Interop.ExecuteJavaScript("$0.invoke($1, $2, $3, $4, $5, $6, $7, $8, $9)", _jsHubProxy, method, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                    break;
                default:
                    throw new InvalidOperationException("The Invoke method cannot be called with so many arguments.");
            }
        }

        public object JsHubProxy
        {
            get
            {
                return _jsHubProxy;
            }
        }
    }
}
