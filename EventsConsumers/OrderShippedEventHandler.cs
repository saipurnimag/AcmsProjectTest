using System;
using System.Collections.Generic;
using System.Text;

namespace EventsConsumers
{
    public class OrderShippedEventHandler
    {
        private readonly string JsonString;

        public OrderShippedEventHandler(string message)
        {
            JsonString = message;
        }

        public void ProcessEvent(object stateInfo)
        {
            Console.WriteLine("Processing OrderShippedEvent: " + JsonString);
        }
    }
}
