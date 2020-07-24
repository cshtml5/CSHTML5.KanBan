using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoRestServer.Models
{
	public class ContractSalesItem
	{
        public string CompanyName { get; set; }
        public string PrimaryContact { get; set; }
        public int ZenDeskTicketId { get; set; }
        public string Comments { get; set; }
        public string StatusInSalesCycle { get; set; }
        public int Order { get; set; }

        public Guid Id { get; set; }

        public ContractSalesItem()
        {

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
    }
}