using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamePulse.Core.Entites;
using GamePulse.Application.DTOs.Game;

namespace GamePulse.Application.Queries.Game
{
    public class GetGamesDataQuery : IRequest<List<GameDto>>
    {
        public int Month { get; set; }

        public Guid? TagId { get; set; }

        public string? Platform { get; set; }
    }

}
