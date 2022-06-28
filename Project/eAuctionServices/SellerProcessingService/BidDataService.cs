using Confluent.Kafka;
using eAuction.Business.SellerBusiness;
using eAuction.Models.APIModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SellerProcessingService
{
    public class BidDataService : IHostedService
    {
        private readonly string topic = "bidlist";
        private readonly string groupId = "bidlist_group";
        private readonly string bootstrapServers = "172.31.30.197:9092";
        private readonly ISellerBusinessManager _sellerBusinessManager;
        private readonly string topic2 = "biddata";
        private readonly ILogger _logger;

        public BidDataService(ILoggerFactory logger,ISellerBusinessManager sellerBusinessManager)
        {
            _logger = logger.CreateLogger("Information");
            _sellerBusinessManager = sellerBusinessManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            string message = string.Empty;
            try
            {
                using (var consumerBuilder = new ConsumerBuilder
                <Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();
                    _logger.LogInformation("consumerBuilder Subscribe called");
                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume
                               (cancelToken.Token);
                            var bidDataParam = JsonSerializer.Deserialize<BidDataParameters>(consumer.Message.Value);
                            _logger.LogInformation("bidDataParam" +bidDataParam.productId);
                           var bidList=_sellerBusinessManager.GetBidData(bidDataParam);
                            message = JsonSerializer.Serialize(bidList);
                            _logger.LogInformation("before sending to topic2 message" + message);
                            await Task.Run(() => SendBidListData(topic2, message));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            //return Task.CompletedTask;
        }

        private async Task<bool> SendBidListData(string topic, string message)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
                using (var producer = new ProducerBuilder
                 <Null, string>(config).Build())
                {
                    var result = await producer.ProduceAsync
                    (topic, new Message<Null, string>
                    {
                        Value = message
                    });

                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return await Task.FromResult(false);
            //return  Task.FromResult("Error occured while getting Bid data list");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
