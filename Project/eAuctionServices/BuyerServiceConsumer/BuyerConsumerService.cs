using Confluent.Kafka;
using eAuction.Business.BuyerBusiness;
using eAuction.Models.APIModels;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BuyerServiceConsumer
{
    public class BuyerConsumerService : IHostedService
    {
        private readonly string topic = "bids";
        private readonly string groupId = "bids_group";
        private readonly string bootstrapServers = "172.31.30.197:9092";
        private readonly IBuyerBusinesManager _buyerBusinesManager;

        public BuyerConsumerService(IBuyerBusinesManager buyerBusinesManager)
        {
            _buyerBusinesManager = buyerBusinesManager;
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
                            var newBid = JsonSerializer.Deserialize<BidDetails>(consumer.Message.Value);
                            message = _buyerBusinesManager.CreateBid(newBid);
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

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
