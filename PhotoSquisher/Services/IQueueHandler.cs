using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.Services
{
    public interface IQueueHandler
    {

        /* potential methods
        Queue object. Ideally limit to one instance
        Construct: Add unprocessed photos to queue field
        Methods to "handle" queue:
        StartQueue 
        StopQueue
        GetQueue - List of objects in queue
        Methods to access info about state of the queue
         */
        public int QueueCount { get; }
    }
}
