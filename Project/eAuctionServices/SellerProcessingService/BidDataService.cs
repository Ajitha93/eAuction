using Confluent.Kafka;
using eAuction.Business.SellerBusiness;
using eAuction.Models.APIModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
        private readonly string bootstrapServers = "localhost:9092";
        private readonly ISellerBusinessManager _sellerBusinessManager;
        private readonly string topic2 = "biddata";

        public BidDataService(ISellerBusinessManager sellerBusinessManager)
        {
            _sellerBusinessManager = sellerBusinessManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
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

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume
                               (cancelToken.Token);
                            var bidDataParam = JsonSerializer.Deserialize<BidDataParameters>(consumer.Message.Value);
                            var bidList=_sellerBusinessManager.GetBidData(bidDataParam);
                            var message1 = JsonSerializer.Serialize(bidList);
                            SendBidListData(topic2, message1);
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

            return  Task.CompletedTask;
        }

        private Task<string> SendBidListData(string topic, string message)
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
                    (topic2, new Message<Null, string>
                    {
                        Value = message
                    });
                    return  Task.FromResult("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return  Task.FromResult("Error occured while getting Bid data list");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
