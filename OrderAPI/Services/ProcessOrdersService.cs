using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderAPI.EventProcesser.Interface;
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderAPI.Services
{
    public class ProcessOrdersService : BackgroundService
    {
        private readonly IEventProducer _producer;
        private readonly IEventConsumer _consumer;
        private readonly ILogger<ProcessOrdersService> _logger;

        public ProcessOrdersService(IEventProducer producer, IEventConsumer consumer, ILogger<ProcessOrdersService> logger)
        {
            _logger = logger;
            _producer = producer;
            _consumer = consumer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderProcessing Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                string orderRequest = _consumer.ReadMessage();
                if (!string.IsNullOrWhiteSpace(orderRequest))
                {
                    //Deserilaize 
                    OrderRequest order = JsonConvert.DeserializeObject<OrderRequest>(orderRequest);

                    //TODO:: Process Order
                    _logger.LogInformation($"Info: OrderHandler => Processing the order for {order.productname}");
                    order.status = OrderStatus.COMPLETED;

                    //Write to ReadyToShip Queue

                    await _producer.WriteMessage(JsonConvert.SerializeObject(order), "readytoship");
                    
                }
                else
                {
                    await Task.Delay(2000);
                }
            }
        }
    }
}
