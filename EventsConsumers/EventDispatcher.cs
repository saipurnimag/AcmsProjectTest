using System.Collections.Concurrent;
using System.Threading;
using Confluent.Kafka;
using MongoDB.Driver;

namespace EventsConsumers
{
    public class EventDispatcher
    {
        private readonly ConcurrentQueue<ConsumeResult<Ignore, string>> q;
        
        public static MongoClient mongo = new MongoClient();
        public EventDispatcher(ConcurrentQueue<ConsumeResult<Ignore, string>> q)
        {
            this.q = q;
        }

        public void Start()
        {
            var thread = new Thread(() => ConsumeEventsFromQueue(q));
            thread.Start();
        }

        public void ConsumeEventsFromQueue(
            ConcurrentQueue<ConsumeResult<Ignore, string>> queue)
        {
            //while (!queue.IsEmpty && ct.IsCancellationRequested)
            while (true)
            {
                //if (ThreadPool.ThreadCount != 0)
                //{
                    if (queue.TryDequeue(out var e))
                    {
                        var topic = e.Topic;
                        switch (topic)
                        {
                            case "order-created":
                                var orderCreatedEventHandler = new OrderCreatedEventHandler(mongo,e.Message.Value);
                                orderCreatedEventHandler.ProcessEvent();
                                break;
                            case "order-shipped":
                                var orderShippedEventHandler = new OrderShippedEventHandler(mongo,e.Message.Value);
                                orderShippedEventHandler.ProcessEvent();
                                break;
                            case "order-delivered":
                                var orderDeliveredEventHandler = new OrderDeliveredEventHandler(mongo,e.Message.Value);
                                orderDeliveredEventHandler.ProcessEvent();
                                break;
                            case "order-cancelled":
                                var orderCancelledEventHandler = new OrderCancelledEventHandler(mongo,e.Message.Value);
                                orderCancelledEventHandler.ProcessEvent();
                                break;
                            case "order-returned":
                                var orderReturnedEventHandler = new OrderReturnedEventHandler(mongo,e.Message.Value);
                                orderReturnedEventHandler.ProcessEvent();
                                break;
                        }
                    //}
                }
                else
                {
                }
            }
                
        }
    }
}