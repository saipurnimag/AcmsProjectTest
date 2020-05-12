using System;
using System.Collections.Generic;
using System.Text;

namespace EventsConsumers
{
    public class OrderDeliveredEventHandler
    {
        private readonly string JsonString;

        public OrderDeliveredEventHandler(string message)
        {
            JsonString = message;
        }

        public void ProcessEvent(object stateInfo)
        {
            Console.WriteLine("Processing OrderDeliveredEvent: " + JsonString);
        }
    }
}
