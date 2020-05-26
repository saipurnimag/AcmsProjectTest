using System;
using System.Text.Json;

public class OrderCreated : Event
{
    public String OrderDate { get; set; }
    public String PromisedShipDate { get; set; }
    public String PromisedDeliveryDate { get; set; }

    public string CreateJson(string orderId, string sellerId)
    {
        var obj = new OrderCreated();
        obj.OrderId = orderId;
        obj.SellerId = sellerId;
        return JsonSerializer.Serialize(obj);
    }
}
