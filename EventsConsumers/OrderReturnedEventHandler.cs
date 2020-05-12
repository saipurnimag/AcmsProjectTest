using System;
using System.Collections.Generic;
using System.Text;

namespace EventsConsumers
{
    public class OrderReturnedEventHandler
    {
        private readonly string JsonString;

        public OrderReturnedEventHandler(string message)
        {
            JsonString = message;
        }

        public void ProcessEvent(object stateInfo)
        {
            Console.WriteLine("Processing OrderReturnedEvent: " + JsonString);
        }
    }
}
