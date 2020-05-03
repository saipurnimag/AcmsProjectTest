using System;
using System.Threading;

namespace KafkaBananaStore
{
    class Program
    {
        public static void Main(string[] args)
        {
            EventProducer producer = new EventProducer();
            Thread thread1 = new Thread(new ThreadStart(()=>producer.ProduceEvents(1,1000)));
            Thread thread2 = new Thread(new ThreadStart(()=>producer.ProduceEvents(1001,2000)));
            thread1.Start();
            thread2.Start();
        }
    }
}