using System;

namespace EventsConsumers
{
    public class OrderCreatedEventHandler
    {
        private string JsonString;
        public OrderCreatedEventHandler(string message)
        {
            JsonString = message;
        }
        public void ProcessEvent(Object stateInfo)
        {
            Console.WriteLine("Processing OrderShippedEvent: "+JsonString);   
        }
    }

    public class OrderShippedEventHandler
    {
        private string JsonString;
        public OrderShippedEventHandler(string message)
        {
            JsonString = message;
        }
        public void ProcessEvent(Object stateInfo)
        {
            Console.WriteLine("Processing OrderShippedEvent: "+JsonString);
        }
    }

    public class OrderDeliveredEventHandler
    {
        private string JsonString;
        public OrderDeliveredEventHandler(string message)
        {
            JsonString = message;
        }
        public void ProcessEvent(Object stateInfo)
        {
            Console.WriteLine("Processing OrderDeliveredEvent: "+JsonString);
        }
    }

    public class OrderCancelledEventHandler
    {
        private string JsonString;
        public OrderCancelledEventHandler(string message)
        {
            JsonString = message;
        }
        public void ProcessEvent(Object stateInfo)
        {
            Console.WriteLine("Processing OrderCancelledEvent: "+JsonString);
        }
    }

    public class OrderReturnedEventHandler
    {
        private string JsonString;
        public OrderReturnedEventHandler(string message)
        {
            JsonString = message;
        }
        public void ProcessEvent(Object stateInfo)
        {
            Console.WriteLine("Processing OrderReturnedEvent: "+JsonString);
        }
    }
}