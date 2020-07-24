using DemoRestServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DemoRestServer.Controllers
{
    public class ContractSalesItemController : ApiController
    {
        private static List<ContractSalesItem> _DefaultContractSalesItemsList = new List<ContractSalesItem>()
        {
            new ContractSalesItem("MikeRoweSoft", "Mike Rowe", 1, "PROPOSAL", 5, "Would like to be allowed to use this name for their company but aren't"),
            new ContractSalesItem("Pidiboup", "This one", 12, "CLOSED_WON", 3, "Pretty cool company name"),
            new ContractSalesItem("AyoYayA", "A serious one", 2, "PROPOSAL", 1),
            new ContractSalesItem("Apple", "One that does more, costs less", 5,"LEAD", 2, "Considers costing more while doing less, thus becoming a luxury product"),
            new ContractSalesItem("Lorem Ipsum TM", "A plain ol' contact", 3, "PROPOSAL", 6, "Dolor sit amet etc."),
            new ContractSalesItem("CorpyCorp", "The best one", 1239, "LEAD", 9, "Do not forget to be \"corporate\""),
            new ContractSalesItem("LimitedLTD", "Confidential", 7, "PROPOSAL", 8, "You wish you knew"),
            new ContractSalesItem("SCP Foundation", "REDACTED", 8,"PROPOSAL", 7, "You do not want to know"),
            new ContractSalesItem("Charmander", "Spicy", 4, "PROPOSAL", 4, "Always on fire"),
            new ContractSalesItem("Koji Kondo", "Musical", 6, "LEAD", 10, "Gives an identity to games"),
            new ContractSalesItem("Some movie", "Some actor", 10, "CLOSED_WON", 11, "Some plot twist"),
            new ContractSalesItem("Anarchists", "None", 11, "LEAD", 12, "They don't know what they are doing so how would we?"),
            new ContractSalesItem("To be continued", "A future one", 13, "LEAD", 13, "The anticipation is killing me!!!"),
            new ContractSalesItem("I don't know", "Some guy", 133, "UNCLASSIFIED", 19, "This should only appear with the unclassified column")
        };

        private static Dictionary<Guid, ContractSalesItem> _ContractSalesItems = new Dictionary<Guid, ContractSalesItem>()
        {
            [_DefaultContractSalesItemsList[0].Id] = _DefaultContractSalesItemsList[0],
            [_DefaultContractSalesItemsList[1].Id] = _DefaultContractSalesItemsList[1],
            [_DefaultContractSalesItemsList[2].Id] = _DefaultContractSalesItemsList[2],
            [_DefaultContractSalesItemsList[3].Id] = _DefaultContractSalesItemsList[3],
            [_DefaultContractSalesItemsList[4].Id] = _DefaultContractSalesItemsList[4],
            [_DefaultContractSalesItemsList[5].Id] = _DefaultContractSalesItemsList[5],
            [_DefaultContractSalesItemsList[6].Id] = _DefaultContractSalesItemsList[6],
            [_DefaultContractSalesItemsList[7].Id] = _DefaultContractSalesItemsList[7],
            [_DefaultContractSalesItemsList[8].Id] = _DefaultContractSalesItemsList[8],
            [_DefaultContractSalesItemsList[9].Id] = _DefaultContractSalesItemsList[9],
            [_DefaultContractSalesItemsList[10].Id] = _DefaultContractSalesItemsList[10],
            [_DefaultContractSalesItemsList[11].Id] = _DefaultContractSalesItemsList[11],
            [_DefaultContractSalesItemsList[12].Id] = _DefaultContractSalesItemsList[12],
            [_DefaultContractSalesItemsList[13].Id] = _DefaultContractSalesItemsList[13]
        };
        // GET api/ContractSalesItem
        public IEnumerable<ContractSalesItem> GetContractSalesItem()
        {
            return _ContractSalesItems.Values.ToList();
        }

        public ContractSalesItem GetContractSalesItem(Guid id)
        {
            if (_ContractSalesItems.ContainsKey(id))
                return _ContractSalesItems[id];
            else
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("ID not found: " + id),
                    ReasonPhrase = "ID not found"
                });
        }

        // OPTIONS
        public HttpResponseMessage Options()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/ContractSalesItem/5
        public HttpResponseMessage PutContractSalesItem(Guid id, ContractSalesItem contractSalesItem)
        {
            if (id != contractSalesItem.Id)
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("The ID must be the same as the ID of the todo."),
                    ReasonPhrase = "The ID must be the same as the ID of the todo"
                };

            _ContractSalesItems[contractSalesItem.Id] = contractSalesItem;
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/ContractSalesItem
        public HttpResponseMessage PostContractSalesItem(ContractSalesItem contractSalesItem)
        {
            _ContractSalesItems[contractSalesItem.Id] = contractSalesItem;

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/ContractSalesItem/5
        public HttpResponseMessage DeleteContractSalesItem(Guid id)
        {
            if (_ContractSalesItems.ContainsKey(id))
            {
                var toDoItem = _ContractSalesItems[id];
                _ContractSalesItems.Remove(id);
                return Request.CreateResponse(HttpStatusCode.OK, toDoItem);
            }
            else
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("ID not found: " + id),
                    ReasonPhrase = "ID not found"
                });
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}