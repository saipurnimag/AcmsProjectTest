using System;
using System.Threading;

namespace EventsProducers
{
    class Program
    {
        public static void Main(string[] args)
        {
            Thread thread1 = new Thread(new ThreadStart(()=>EventProducer.ProduceEvents(1,1000)));
            Thread thread2 = new Thread(new ThreadStart(()=>EventProducer.ProduceEvents(1001,2000)));
            Thread thread3 = new Thread(new ThreadStart((() => EventProducer.ProduceEvents(2001,3000))));
            thread1.Start();
            thread2.Start();
            thread3.Start();
        }
    }
}