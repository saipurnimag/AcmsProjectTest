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

        public async void ProcessEvent()
        {
            Console.WriteLine("Processing OrderShippedEvent: " + JsonString);
        }
    }
}
