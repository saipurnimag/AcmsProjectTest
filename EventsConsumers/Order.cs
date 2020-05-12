using System;
using System.Collections.Generic;
using System.Text;

namespace EventsConsumers
{
    class Order
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }
        public DateTime ActualShipDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public string CancellationOrigin { get; set; }
        public string CancellationReason { get; set; }
        public bool isReturned { get; set; }

    }
    class Seller
    {
        public string SellerId { get; set; }
    }
}
