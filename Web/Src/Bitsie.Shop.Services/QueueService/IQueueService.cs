using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
namespace Bitsie.Shop.Services
{
    public interface IQueueService
    {
        Queue GetQueueByGuid(string guid);
        IList<Queue> GetFailed();
        Queue Enqueue(string guid, QueueAction action, string input, QueueStatus status, string url);
        void Process(Queue queue);
        void Save(Queue queue);
    }
}
