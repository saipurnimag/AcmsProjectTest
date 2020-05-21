using System.Threading;

namespace EventsProducers
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var thread1 = new Thread(() => EventProducer.ProduceEvents());
            var thread2 = new Thread(() => EventProducer.ProduceEvents());
            var thread3 = new Thread(() => EventProducer.ProduceEvents());
            thread1.Start();
            thread2.Start();
            thread3.Start();
        }
    }
}