

namespace CSHTML5.Extensions.SignalR.Client
{
    /// <summary>
    /// A client side proxy for a server side hub.
    /// </summary>
    public interface IHubProxy
    {
        /// <summary>
        /// Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        void Invoke(string method, params object[] args);

        /// <summary>
        /// Gets the underlying JavaScript HubProxy from the JavaScript SignalR implementation.
        /// </summary>
        object JsHubProxy { get; }
    }
}
