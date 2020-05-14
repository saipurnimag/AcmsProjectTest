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

        public async void ProcessEvent()
        {
            Console.WriteLine("Processing OrderReturnedEvent: " + JsonString);
        }
    }
}
