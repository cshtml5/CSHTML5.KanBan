﻿using KanBan;
using KanBan.SampleElements;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CSHTML5.Extensions.SignalR.Client;
using CSHTML5.Extensions.SignalR.Client.EventArgs;

using System.Linq;
using System.Net;
using System.Text;
using System.Runtime.Serialization;

namespace KanBanSampleApplication
{
    public partial class MainPage : Page
    {
        const string LEAD = "LEAD";
        const string PROPOSAL = "PROPOSAL";
        const string NEGOCIATION = "NEGOCIATION";
        const string CLOSED_WON = "CLOSED_WON";
        const string UNCLASSIFIED = "UNCLASSIFIED";

        List<ContractSalesItem> _contractSalesitems = new List<ContractSalesItem>();

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += UserControl_Loaded;

            RefreshFromRestServer();
        }

        /* // Commented out because the data is now downloaded from the server.
        List<ContractSalesItem> GetContractSalesItems()
        {
            List<ContractSalesItem> contractSalesitems = new List<ContractSalesItem>();
            //                                              company name,   primary contact,             zdTicket,      status,       order    comment
            contractSalesitems.Add(new ContractSalesItem("MikeRoweSoft", "Mike Rowe", 1, PROPOSAL, 5, "Would like to be allowed to use this name for their company but aren't"));
            contractSalesitems.Add(new ContractSalesItem("Pidiboup", "This one", 12, CLOSED_WON, 3, "Pretty cool company name"));
            contractSalesitems.Add(new ContractSalesItem("AyoYayA", "A serious one", 2, PROPOSAL, 1));
            contractSalesitems.Add(new ContractSalesItem("Apple", "One that does more, costs less", 5, LEAD, 2, "Considers costing more while doing less, thus becoming a luxury product"));
            contractSalesitems.Add(new ContractSalesItem("Lorem Ipsum TM", "A plain ol' contact", 3, PROPOSAL, 6, "Dolor sit amet etc."));
            contractSalesitems.Add(new ContractSalesItem("CorpyCorp", "The best one", 1239, LEAD, 9, "Do not forget to be \"corporate\""));
            contractSalesitems.Add(new ContractSalesItem("LimitedLTD", "Confidential", 7, PROPOSAL, 8, "You wish you knew"));
            contractSalesitems.Add(new ContractSalesItem("SCP Foundation", "REDACTED", 8, PROPOSAL, 7, "You do not want to know"));
            contractSalesitems.Add(new ContractSalesItem("Charmander", "Spicy", 4, PROPOSAL, 4, "Always on fire"));
            contractSalesitems.Add(new ContractSalesItem("Koji Kondo", "Musical", 6, LEAD, 10, "Gives an identity to games"));
            contractSalesitems.Add(new ContractSalesItem("Some movie", "Some actor", 10, CLOSED_WON, 11, "Some plot twist"));
            contractSalesitems.Add(new ContractSalesItem("Anarchists", "None", 11, LEAD, 12, "They don't know what they are doing so how would we?"));
            contractSalesitems.Add(new ContractSalesItem("To be continued", "A future one", 13, LEAD, 13, "The anticipation is killing me!!!"));
            contractSalesitems.Add(new ContractSalesItem("I don't know", "Some guy", 133, UNCLASSIFIED, 19, "This should only appear with the unclassified column"));
            return contractSalesitems;
        }
        */

        private void ButtonCreateItem_Click(object sender, RoutedEventArgs e)
        {
            //todo: display the ChildWindow to create a new ContractSalesItem.
            ContractSalesItem newContractSalesItem = new ContractSalesItem();
            var childWindow = new CreateEditItemChildWindow();
            childWindow.DataContext = newContractSalesItem;
            childWindow.Closed += ChildWindowCreateItem_Closed;
            childWindow.Show();
        }

        private void ChildWindowCreateItem_Closed(object sender, EventArgs e)
        {
            ChildWindow childWindow = ((ChildWindow)sender);
            if (childWindow.DialogResult == true)
            {
                ContractSalesItem item = childWindow.DataContext as ContractSalesItem;
                if (item != null)
                {
                    _contractSalesitems.Add(item);
                    MyKanBanControl.ItemsSource = null;
                    MyKanBanControl.ItemsSource = _contractSalesitems;
                    AddToRestServerContractSalesItem(item);
                }
            }

        }

        private void KanBan_ItemClicked(object sender, ItemClickedEventArgs e)
        {
            var childWindow = new CreateEditItemChildWindow();
            var parameter = e.Parameter;

            if (parameter == "EDIT")
            {
                childWindow.DataContext = ((ContractSalesItem)e.Source).Clone();
                childWindow.Closed += ChildWindowEditItem_Closed;
                childWindow.Show();
            }
            else if (parameter == "DELETE")
            {
                DeleteFromRestServerContractSalesItem((ContractSalesItem)e.Source);
                _contractSalesitems.Remove(((ContractSalesItem)e.Source));
                MyKanBanControl.Refresh();
            }
        }

        private void KanBan_ItemMoved(object sender, ItemMovedEventArgs e)
        {
            var item = (ContractSalesItem)e.Source;
            UpdateRestServerContractSalesItem(item);
        }

        private void ChildWindowEditItem_Closed(object sender, EventArgs e)
        {
            ChildWindow childWindow = ((ChildWindow)sender);
            if (childWindow.DialogResult == true)
            {
                ContractSalesItem item = childWindow.DataContext as ContractSalesItem;
                if (item != null && item.CloneOf != null) //CloneOf should never be null but we never know.
                {
                    //we apply the changes that were applied to the clone to the original:
                    ContractSalesItem original = item.CloneOf;
                    original.CompanyName = item.CompanyName;
                    original.PrimaryContact = item.PrimaryContact;
                    original.ZenDeskTicketId = item.ZenDeskTicketId;
                    original.StatusInSalesCycle = item.StatusInSalesCycle;
                    original.Order = item.Order;
                    original.Comments = item.Comments;

                    UpdateRestServerContractSalesItem(original);

                    MyKanBanControl.Refresh();
                }
            }
        }

        HubConnection connection;
        IHubProxy hub;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SignalRConnectionStatusText.Text = "Connecting to SignalR...";


            // Specify the URL of the server:
            //string serverUri = new Uri(HtmlPage.Document.DocumentUri.ToString()).ToString();
            string serverUri = new Uri("http://localhost:44357/").ToString();

            // Create a new Connection:
            connection = new HubConnection(serverUri);

            // Create a new Hub:
            hub = await connection.CreateHubProxy("chathub");

            hub.On("UpdateKanBan",
                () =>
                {
                    RefreshFromRestServer();
                });

            connection.ClientConnecting += SignalRClientConnecting;
            connection.ClientConnected += SignalRClientConnected;
            connection.ClientReconnecting += SignalRClientReconnection;
            connection.ClientDisconnected += SignalRClientDisconnected;
            connection.Start();


        }

        private void SignalRClientConnecting(object sender, ConnectionEventArgs e)
        {
            SignalRConnectionStatusText.Text = "Connecting to SignalR...";
        }

        private void SignalRClientConnected(object sender, ConnectionEventArgs e)
        {
            SignalRConnectionStatusText.Text = "Connected to SignalR";
        }

        private void SignalRClientReconnection(object sender, ConnectionEventArgs e)
        {
            SignalRConnectionStatusText.Text = "Connection to SignalR lost, Attempting to reconnect";
        }

        private void SignalRClientDisconnected(object sender, ConnectionEventArgs e)
        {
            SignalRConnectionStatusText.Text = "Disconnected from SignalR";
        }

        private void MakeCallToUpdateOtherClients()
        {
            try
            {
                // Send the message: 
                hub.Invoke("MakeCallToUpdateOtherClients");
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to send message via SignalR. Exception message: " + e.Message);
            }
        }


        const string BaseUrl = "http://localhost:44314/";
        private async void RefreshFromRestServer()
        {
            // Download the items from the server via REST:
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers[HttpRequestHeader.Accept] = "application/xml";
            string response = null;
            try
            {
                response = await webClient.DownloadStringTaskAsync(BaseUrl + "/api/ContractSalesItem");
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to download items from REST. Exception message: " + e.Message);
            }

            // Update the KanBan:
            if (response != null)
            {
                var dataContractSerializer = new DataContractSerializer(typeof(List<ContractSalesItem>));
                List<ContractSalesItem> ContractSalesItems = (List<ContractSalesItem>)dataContractSerializer.DeserializeFromString(response);
                if (ContractSalesItems.Count > 0)
                {
                    _contractSalesitems = ContractSalesItems;
                    MyKanBanControl.ItemsSource = _contractSalesitems;
                }
            }
        }

        private async void AddToRestServerContractSalesItem(ContractSalesItem contractSalesItem)
        {
            string data = contractSalesItem.DataToString();

            // Upload the item to the server via REST:
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            try
            {
                await webClient.UploadStringTaskAsync(BaseUrl + "/api/ContractSalesItem/", "POST", data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to upload item via REST. Exception message: " + e.Message);

                return;
            }

            // Send message via SignalR to update the other clients:
            MakeCallToUpdateOtherClients();
        }

        private async void DeleteFromRestServerContractSalesItem(ContractSalesItem contractSalesItem)
        {
            // Update the server via REST:
            var webClient = new WebClient();
            try
            {
                string response = await webClient.UploadStringTaskAsync(BaseUrl + "/api/ContractSalesItem/" + contractSalesItem.Id.ToString(), "DELETE", "");
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to upload item via REST. Exception message: " + e.Message);

                return;
            }

            // Send message via SignalR to update the other clients:
            MakeCallToUpdateOtherClients();
        }

        private async void UpdateRestServerContractSalesItem(ContractSalesItem contractSalesItem)
        {
            // Upload the item to the server via REST:
            string data = contractSalesItem.DataToString();
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            string response = null;
            try
            {
                response = await webClient.UploadStringTaskAsync(BaseUrl + "/api/ContractSalesItem/" + contractSalesItem.Id.ToString(), "PUT", data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to upload item via REST. Exception message: " + e.Message);
            }

            // Send message via SignalR to update the other clients:
            if (response != null)
            {
                MakeCallToUpdateOtherClients();
            }
        }
    }

    public class ContractSalesItem
    {
        public ContractSalesItem()
        {
            Id = Guid.NewGuid();
        }

        public ContractSalesItem(string companyName, string primarycontact, int zenDeskTicketId, string statusInSalesCycle, int order, string comments = null)
        {
            CompanyName = companyName;
            PrimaryContact = primarycontact;
            ZenDeskTicketId = zenDeskTicketId;
            StatusInSalesCycle = statusInSalesCycle;
            Order = order;
            Comments = comments;
            Id = Guid.NewGuid();
        }

        public string CompanyName { get; set; }
        public string PrimaryContact { get; set; }
        public int ZenDeskTicketId { get; set; }
        public string Comments { get; set; }
        public string StatusInSalesCycle { get; set; }
        public int Order { get; set; }

        public Guid Id { get; set; }


        //Note: This property is only set when we called the method Clone. It will allow us to get the original back.
        //      We created it so we would have a way to apply the changes on the original item after showing the CreateEditItemChildWindow to edit an existing item.
        public ContractSalesItem CloneOf { get; private set; } = null;

        public ContractSalesItem Clone()
        {
            return new ContractSalesItem(CompanyName, PrimaryContact, ZenDeskTicketId, StatusInSalesCycle, Order, Comments) { CloneOf = this, Id = this.Id };
        }

        public string DataToString()
        {
            return string.Format(@"{{""CompanyName"": ""{0}"",""PrimaryContact"": ""{1}"", ""ZenDeskTicketId"": ""{2}"",""Comments"": ""{3}"",
                ""StatusInSalesCycle"": ""{4}"",""Order"": ""{5}"",""Id"": ""{6}""}}",
                CompanyName, PrimaryContact, ZenDeskTicketId, Comments, StatusInSalesCycle, Order, Id);
        }
    }
}
