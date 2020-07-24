using System;

namespace CSHTML5.Extensions.SignalR.Client
{
    public static class HubProxyExtensions
    {
        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <param name="proxy">The SignalR IHubProxy.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        public static void On(this IHubProxy proxy, string eventName, Action onData)
        {
            Interop.ExecuteJavaScript("$0.on($1, function() { $2() })", proxy.JsHubProxy, eventName, onData);
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proxy">The SignalR IHubProxy.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        public static void On<T>(this IHubProxy proxy, string eventName, Action<T> onData)
        {
            Interop.ExecuteJavaScript("$0.on($1, function(value) { $2(value) })", proxy.JsHubProxy, eventName, onData);
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="proxy">The SignalR IHubProxy.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        public static void On<T1, T2>(this IHubProxy proxy, string eventName, Action<T1, T2> onData)
        {
            Interop.ExecuteJavaScript("$0.on($1, function(value1, value2) { $2(value1, value2) })", proxy.JsHubProxy, eventName, onData);
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="proxy">The SignalR IHubProxy.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        public static void On<T1, T2, T3>(this IHubProxy proxy, string eventName, Action<T1, T2, T3> onData)
        {
            Interop.ExecuteJavaScript("$0.on($1, function(value1, value2, value3) { $2(value1, value2, value3) })", proxy.JsHubProxy, eventName, onData);
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="proxy">The SignalR IHubProxy.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        public static void On<T1, T2, T3, T4>(this IHubProxy proxy, string eventName, Action<T1, T2, T3, T4> onData)
        {
            Interop.ExecuteJavaScript("$0.on($1, function(value1, value2, value3, value4) { $2(value1, value2, value3, value4) })", proxy.JsHubProxy, eventName, onData);
        }
    }
}
