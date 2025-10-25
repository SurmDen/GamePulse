using GamePulse.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GamePulse.Infrastructure.MessageBus
{
    public class KafkaProduceService : IMessageProduceService, IDisposable
    {
        public KafkaProduceService(ILogger<KafkaProduceService> logger, IConfiguration configuration)
        {
            _logger = logger;

            try
            {
                var config = new ProducerConfig()
                {
                    BootstrapServers = configuration["Kafka:server"] ?? throw new InvalidOperationException("invalid kafka server"),

                    Acks = Acks.All,
                    MessageSendMaxRetries = 3,
                    RetryBackoffMs = 1000
                };

                _producer = new ProducerBuilder<string, string>(config).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured, while trying to connect Kafka");

                throw;
            }
        }

        private readonly ILogger<KafkaProduceService> _logger;
        private readonly IProducer<string, string> _producer;

        public async Task ProduceMessageAsync(string topicName, string message)
        {
            try
            {
                _logger.LogInformation($"Start sending message to topic: {topicName}");

                var kafkaMessage = new Message<string, string>()
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = message
                };

                var result = await _producer.ProduceAsync(topicName, kafkaMessage);

                _logger.LogInformation($"Successfully sent message to topic: {topicName}, partition: {result.Partition}, offset: {result.Offset}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occured while sending message to topic: {topicName}");

                throw;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
