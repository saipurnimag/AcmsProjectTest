using System;
using System.Text.Json;

public class OrderCreated : Event
{
    public DateTime OrderDate { get; set; }
    public DateTime PromisedShipDate { get; set; }
    public DateTime PromisedDeliveryDate { get; set; }

    public string CreateJson(string orderId, string sellerId)
    {
        var date1 = new DateTime();
        var obj = new OrderCreated();
        obj.OrderId = orderId;
        obj.OrderDate = DateTime.Now;
        obj.PromisedShipDate = DateTime.Now;
        obj.PromisedDeliveryDate = DateTime.Now;
        obj.SellerId = sellerId;
        return JsonSerializer.Serialize(obj);
    }
}
