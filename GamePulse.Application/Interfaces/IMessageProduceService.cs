using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Interfaces
{
    public interface IMessageProduceService
    {
        public Task ProduceMessageAsync(string topicName, string message);
    }
}
