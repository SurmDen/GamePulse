using GamePulse.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamePulse.Application.Events
{
    public class GameSearchEventHandler : INotificationHandler<GameSearchEvent>
    {
        public GameSearchEventHandler(IMessageProduceService messageProduceService, ILogger<GameSearchEventHandler> logger)
        {
            _messageProduceService = messageProduceService;
            _logger = logger;
        }

        private readonly IMessageProduceService _messageProduceService;
        private readonly ILogger<GameSearchEventHandler> _logger;

        public async Task Handle(GameSearchEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                string message = JsonSerializer.Serialize(notification);

                await _messageProduceService.ProduceMessageAsync("game-searching", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying send event");

                throw;
            }
        }
    }
}
