﻿using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Browser;
using CSHTML5.Extensions.SignalR.Client.EventArgs;
using Windows.UI.Xaml;

namespace CSHTML5.Extensions.SignalR.Client
{
    /// <summary>
    /// A SignalR Connection for interacting with Hubs.
    /// </summary>
    public class HubConnection
    {
        string _serverUri;
        object _hubConnection;
        static bool IsSignalRJavaScriptLibraryLoaded;
        private ConnectionState _connectionState = ConnectionState.Disconnected;

        public ConnectionState connectionState
        {
            get
            {
                return _connectionState;
            }
            set
            {
                _connectionState = value;

                switch (connectionState)
                {
                    case ConnectionState.Connecting:
                        if (ClientConnecting != null)
                            ClientConnecting(this, new ConnectionEventArgs());
                        break;

                    case ConnectionState.Connected:
                        if (ClientConnected != null)
                            ClientConnected(this, new ConnectionEventArgs());
                        break;

                    case ConnectionState.Reconnecting:
                        if (ClientReconnecting != null)
                            ClientReconnecting(this, new ConnectionEventArgs());
                        break;

                    case ConnectionState.Disconnected:
                        if (ClientDisconnected != null)
                            ClientDisconnected(this, new ConnectionEventArgs());
                        break;
                }
            }
        }

        #region ClientConnecting event

        /// <summary>
        /// An event raised when the client is connected to the server.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> ClientConnecting;
        #endregion

        #region ClientConnected event

        /// <summary>
        /// An event raised when the client is connected to the server.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> ClientConnected;
        #endregion

        #region ClientReconnecting event

        /// <summary>
        /// An event raised when the client failed to connect to the server.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> ClientReconnecting;
        #endregion

        #region ClientDisconnected event

        /// <summary>
        /// An event raised when the client is disconnected from the server.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> ClientDisconnected;
        #endregion

        /// <summary>
        /// Initializes a new instance of the SignalR.Client.HubConnection
        /// class.
        /// </summary>
        /// <param name="serverUri">The uri to connect to.</param>
        public HubConnection(string serverUri)
        {
            _serverUri = serverUri;

            //------------------------------------------------------------
            // On Microsoft Edge, SignalR can only run from HTTP or HTTPS:
            //------------------------------------------------------------
            bool isRunningoOnMicrosoftEdgeBrowser = Convert.ToBoolean(Interop.ExecuteJavaScript("window.navigator.userAgent.indexOf('Edge') > -1"));
            string documentUri = HtmlPage.Document.DocumentUri.ToString();
            if (isRunningoOnMicrosoftEdgeBrowser && !documentUri.ToLower().StartsWith("http"))
            {
                Windows.UI.Xaml.MessageBox.Show("On Microsoft Edge, you must run this app from HTTP or HTTPS, because SignalR only supports those schemas. To fix this issue, either deploy this application to a website, or test it on a different browser, or click the 'Run from Localhost' button in the Simulator (you will find it by clicking the small '...' button that is near the green 'Run in browser' button in the Simulator).");
            }
        }

        static async Task EnsureLibraryIsLoaded()
        {
            if (!IsSignalRJavaScriptLibraryLoaded)
            {
                // Load a JS file that was created based on https://github.com/DVLP/signalr-no-jquery
                await Interop.LoadJavaScriptFile("ms-appx:///CSHTML5.Extensions.SignalR.Client/bundle.js");
                IsSignalRJavaScriptLibraryLoaded = true;
            }
        }

        async Task EnsureConnectionIsCreated()
        {
            await EnsureLibraryIsLoaded();
            if (_hubConnection == null)
            {
                // We use the javascript library loaded in the 'EnsureLibraryIsLoaded' method:
                _hubConnection = Interop.ExecuteJavaScript("signalRNoJQueryBundle.signalRNoJquery.hubConnection($0)", _serverUri);

                Action<Object> OnStateChangedAction = OnStateChanged;

                Interop.ExecuteJavaScript("$0.stateChanged(function(data) {($1)(data);})", _hubConnection, OnStateChangedAction);
            }
        }

        private void OnStateChanged(Object data)
        {
            object newStateValueAsObject = Interop.ExecuteJavaScript(@"$0['newState']", data);

            int newStateValue = int.Parse(newStateValueAsObject.ToString());

           connectionState = (ConnectionState)newStateValue;

            Debug.WriteLine("SignalR state: " + connectionState.ToString());
        }


        /// <summary>
        /// Creates a SignalR IHubProxy for the hub with the specified name.
        /// </summary>
        /// <param name="hubName">The name of the hub.</param>
        /// <returns>A SignalR IHubProxy</returns>
        public async Task<IHubProxy> CreateHubProxy(string hubName)
        {
            // Note: the Silverlight version of this method is not 'async'

            //Check that the library was loaded and the connection created:
            await EnsureConnectionIsCreated();

            // Create the HubProxy:
            object _hubProxy = Interop.ExecuteJavaScript("$0.createHubProxy($1)", _hubConnection, hubName);
            IHubProxy hubProxy = new HubProxy(_hubProxy, hubName);
            return hubProxy;
        }

        /// <summary>
        /// Starts the SignalR Connection.
        /// </summary>
        public async void Start()
        {
            // Note: in Silvelight, this method is located in the Connection class.

            await EnsureConnectionIsCreated();
            await StartJS();
        }

        Task<object> StartJS()
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            Interop.ExecuteJavaScript("$0.start({ jsonp: true }).done(function() {($1)();})", _hubConnection, (Action)(() => { taskCompletionSource.SetResult(null); }));
            return taskCompletionSource.Task;
        }
    }
}
