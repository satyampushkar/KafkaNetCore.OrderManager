using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.EventProcesser.Interface
{
    public interface IEventProducer
    {
        Task<bool> WriteMessage(string message, string topic);
    }
}
