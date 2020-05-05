using System;
using System.Collections.Concurrent;
using System.Threading;

namespace EventsConsumers
{
    public class EventDispatcher
    {
        private ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q;
        public EventDispatcher(ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q)
        {
            this.q = q;
        }

        public void Start(CancellationToken ct)
        {
            Thread thread = new Thread(() => this.ConsumeEventsFromQueue(q));
            thread.Start(ct);
        }

        public void ConsumeEventsFromQueue(
            ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> queue)
        {
            while (true)
            {
                if (ThreadPool.ThreadCount != 0)
                {
                    if (queue.TryDequeue(out Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string> e))
                    {
                        string topic = e.Topic;
                        switch (topic)
                        {
                            case "order-created":
                                OrderCreatedEventHandler orderCreatedEventHandler = new OrderCreatedEventHandler(e.Message.Value);
                                ThreadPool.QueueUserWorkItem(orderCreatedEventHandler.ProcessEvent);
                                break;
                            case "order-shipped":
                                OrderShippedEventHandler orderShippedEventHandler = new OrderShippedEventHandler(e.Message.Value);
                                ThreadPool.QueueUserWorkItem(orderShippedEventHandler.ProcessEvent);
                                break;
                            case "order-delivered":
                                OrderDeliveredEventHandler orderDeliveredEventHandler = new OrderDeliveredEventHandler(e.Message.Value);
                                ThreadPool.QueueUserWorkItem(orderDeliveredEventHandler.ProcessEvent);
                                break;
                            case "order-cancelled":
                                OrderCancelledEventHandler orderCancelledEventHandler = new OrderCancelledEventHandler(e.Message.Value);
                                ThreadPool.QueueUserWorkItem(orderCancelledEventHandler.ProcessEvent);
                                break;
                            case "order-returned":
                                OrderReturnedEventHandler orderReturnedEventHandler = new OrderReturnedEventHandler(e.Message.Value);
                                ThreadPool.QueueUserWorkItem(orderReturnedEventHandler.ProcessEvent);
                                break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
}