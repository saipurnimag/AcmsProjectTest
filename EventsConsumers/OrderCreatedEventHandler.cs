using System;

namespace EventsConsumers
{
    public class OrderCreatedEventHandler
    {
        private readonly string JsonString;

        public OrderCreatedEventHandler(string message)
        {
            JsonString = message;
        }

        public void ProcessEvent(object stateInfo)
        {
            Console.WriteLine("Processing OrderShippedEvent: " + JsonString);
        }
    }

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