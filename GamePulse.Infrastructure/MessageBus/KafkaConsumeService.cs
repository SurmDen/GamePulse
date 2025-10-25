using Confluent.Kafka;
using GamePulse.Application.Events;
using GamePulse.Core.Interfaces;
using GamePulse.Core.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.MessageBus
{
    public class KafkaConsumeService : BackgroundService
    {
        public KafkaConsumeService(IGameParser gameParser, IGameRepository gameRepository, ILogger<KafkaConsumeService> logger, IConfiguration configuration)
        {
            try
            {
                _gameParser = gameParser;
                _gameRepository = gameRepository;
                _logger = logger;

                var config = new ConsumerConfig()
                {
                    GroupId = configuration["Kafka:GroupId"] ?? "GamePulseGroup",
                    BootstrapServers = configuration["Kafka:Server"] ?? throw new InvalidOperationException("invalid kafka server")
                };

                _consumer = new ConsumerBuilder<string, string>(config).Build();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while trying to connect Kafka");

                throw;
            }
        }

        private readonly IGameRepository _gameRepository;
        private readonly IGameParser _gameParser;
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<KafkaConsumeService> _logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topicName = "game-searching";

            _consumer.Subscribe(topicName);

            _logger.LogInformation($"Start consume for {topicName}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = _consumer.Consume(1000);

                    if (message != null)
                    {
                        GameSearchEvent? searchEvent = JsonSerializer.Deserialize<GameSearchEvent>(message.Message.Value);

                        if (searchEvent != null)
                        {
                            await _gameRepository.AddGamesAsync(await _gameParser.GetGamesFromApiAsync(searchEvent.NeededMonth));
                        }
                        else
                        {
                            _logger.LogWarning($"message parsing error {typeof(GameSearchEvent)}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Message from {topicName} was null");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cunsumer error ocured");

                    throw;
                }
            }
        }
    }
}
