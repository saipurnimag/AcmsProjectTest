using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventsConsumers
{
    
    public class OrderCancelledEventHandler
    {
        private readonly string JsonString;

        public OrderCancelledEventHandler(string message)
        {
            JsonString = message;
        }

        public async void ProcessEvent()
        {
            Console.WriteLine("Processing OrderCancelledEvent: " + JsonString);
        }
    }
}
