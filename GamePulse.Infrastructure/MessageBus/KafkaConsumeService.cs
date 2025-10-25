using Confluent.Kafka;
using GamePulse.Application.Events;
using GamePulse.Core.Interfaces;
using GamePulse.Core.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumer<string, string>? _consumer;
        private readonly ILogger<KafkaConsumeService> _logger;

        public KafkaConsumeService(
            IServiceProvider serviceProvider,
            ILogger<KafkaConsumeService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            try
            {
                var config = new ConsumerConfig()
                {
                    GroupId = configuration["Kafka:GroupId"] ?? "GamePulseGroup",
                    BootstrapServers = configuration["Kafka:Server"] ?? "localhost:9092",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                _consumer = new ConsumerBuilder<string, string>(config).Build();
                _logger.LogInformation("✅ Kafka Consumer создан успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка создания Kafka Consumer");
                _consumer = null;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);

            if (_consumer == null)
            {
                _logger.LogWarning("⚠️ Kafka Consumer не создан, сервис останавливается");
                return;
            }

            try
            {
                _consumer.Subscribe("game-searching");
                _logger.LogInformation("👂 Start consume for game-searching");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = _consumer.Consume(1000);

                        if (message != null)
                        {
                            _logger.LogInformation("📨 Получено сообщение: {Message}", message.Message.Value);

                            GameSearchEvent? searchEvent = JsonSerializer.Deserialize<GameSearchEvent>(message.Message.Value);

                            if (searchEvent != null)
                            {
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    IGameParser gameParser = scope.ServiceProvider.GetRequiredService<IGameParser>();
                                    IGameRepository gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

                                    var games = await gameParser.GetGamesFromApiAsync(searchEvent.NeededMonth);

                                    await gameRepository.AddGamesAsync(games);
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"message parsing error {typeof(GameSearchEvent)}");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "❌ Ошибка потребления сообщения");
                        await Task.Delay(5000, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Критическая ошибка в Kafka Consumer");
            }
            finally
            {
                _consumer?.Close();
                _consumer?.Dispose();
            }
        }
    }
}
