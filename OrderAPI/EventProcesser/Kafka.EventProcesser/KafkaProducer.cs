﻿using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using OrderAPI.EventProcesser.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.EventProcesser.Kafka.EventProcesser
{
    public class KafkaProducer : IEventProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;
        private static readonly Random rand = new Random();
        public KafkaProducer(ILogger<KafkaProducer> logger)
        {
            _logger = logger;
            _producer = KafkaUtils.CreateProducer("localhost:9092");
        }

        public async Task<bool> WriteMessage(string message, string topic)
        {
            try
            {
                var dr = await this._producer.ProduceAsync(topic, new Message<string, string>()
                {
                    Key = rand.Next(50).ToString(),
                    Value = message
                });
                _logger.LogInformation($"KAFKA => Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            return true;
        }
    }
}
