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

        public async void ProcessEvent()
        {
            Console.WriteLine("Processing OrderDeliveredEvent: " + JsonString);
        }
    }
}
