using Confluent.Kafka;
using eAuction.Business.SellerBusiness;
using eAuction.Models.APIModels;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SellerService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SellerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ISellerBusinessManager _sellerBusinessManager;
        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "bidlist";

        public SellerController(ILoggerFactory logger, ISellerBusinessManager sellerBusinessManager)
        {
            _logger = logger.CreateLogger("Information");
            _sellerBusinessManager = sellerBusinessManager;
        }

        private string SendBidRequest(string topic, string message)
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
                    var result =  producer.ProduceAsync
                    (topic, new Message<Null, string>
                    {
                        Value = message
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }
            return "";
        }

        [HttpGet]
        [Route("/show-bids/{productId}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get([FromRoute]string productId,[FromQuery] BidDataParameters bidDataParameters)
        {
            try
            {
                _logger.LogInformation("Show-bids method called");
                bidDataParameters.productId = productId;
                var message = JsonSerializer.Serialize(bidDataParameters);
                SendBidRequest(topic, message);
                return Ok(_sellerBusinessManager.GetBidData(bidDataParameters));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return  Ok(ex.Message);
            }
        }


        [HttpPost]
        [Route("/add-product")]
        public IActionResult AddProduct(ProductData newProduct)
        {
            newProduct.ProductId = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
                _sellerBusinessManager.CreateProduct(newProduct);

            return Ok("Product created successfully!!");
        }

        [HttpDelete]
        [Route("/delete/{productId}")]
        public IActionResult Delete([FromRoute] string productId)
        {
            string msg = _sellerBusinessManager.DeleteProduct(productId);
            return Ok(msg);
        }

        [HttpGet]
        [Route("/show-products")]
        public IActionResult GetProducts()
        {
           return Ok(_sellerBusinessManager.GetProductList());
        }
    }
}
