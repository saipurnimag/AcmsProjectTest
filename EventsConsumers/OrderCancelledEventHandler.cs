using System;
using System.Collections.Generic;
using System.Text;

namespace EventsConsumers
{
    public class OrderCancelledEventHandler
    {
        private readonly string JsonString;

        public OrderCancelledEventHandler(string message)
        {
            JsonString = message;
        }

        public void ProcessEvent(object stateInfo)
        {
            Console.WriteLine("Processing OrderCancelledEvent: " + JsonString);
        }
    }
}
