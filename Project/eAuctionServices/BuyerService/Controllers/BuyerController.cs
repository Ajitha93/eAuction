using Confluent.Kafka;
using eAuction.Business.BuyerBusiness;
using eAuction.Models.APIModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BuyerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyerController : ControllerBase
    {
        private readonly IBuyerBusinesManager _buyerBusinesManager;
        private readonly string bootstrapServers = "172.31.30.197:9092";
        private readonly string topic = "bids";

        public BuyerController( IBuyerBusinesManager buyerBusinesManager)
        {
            _buyerBusinesManager = buyerBusinesManager;
        }


        private async Task<string> SendBuyerRequest(string topic, string message)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    var result = await producer.ProduceAsync
                    (topic, new Message<Null, string>
                    {
                        Value = message
                    });
                    return await Task.FromResult("Bid data is processing");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return await Task.FromResult("Error occured in Bid data processing");
        }

        [HttpPost]
        [Route("/place-bid")]
        public async Task<IActionResult> AddBuyer(BidDetails newBid)
        {
            string message = string.Empty;
            try
            {
               
                newBid.BuyerId = Guid.NewGuid().ToString();

                if (ModelState.IsValid)
                {
                    message = _buyerBusinesManager.CreateBidValidations(newBid);
                    if (message == "valid")
                    {
                        #region Kafka
                        message = JsonSerializer.Serialize(newBid);
                        return Ok(await SendBuyerRequest(topic, message));
                        #endregion
                        //message=_buyerBusinesManager.CreateBid(newBid);
                    }
                }

                
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            return Ok(message);
        }

        [HttpPost]
        [Route("/update-bid/{productId}/{buyerEmailld}/{newBidAmount}")]
        public IActionResult UpdateBuyer([FromRoute] string productId, [FromRoute] string buyerEmailld, [FromRoute] decimal newBidAmount)
        {
            string message = _buyerBusinesManager.UpdateBid(buyerEmailld, productId, newBidAmount);
            return Ok(message);          
        }
    }
}
