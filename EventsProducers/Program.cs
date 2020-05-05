using System.Threading;

namespace EventsProducers
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var thread1 = new Thread(() => EventProducer.ProduceEvents(1, 1000));
            var thread2 = new Thread(() => EventProducer.ProduceEvents(1001, 2000));
            var thread3 = new Thread(() => EventProducer.ProduceEvents(2001, 3000));
            thread1.Start();
            thread2.Start();
            thread3.Start();
        }
    }
}