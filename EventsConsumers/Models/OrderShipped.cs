using System;
using System.Text.Json;

public class OrderShipped : Event
{
    public DateTime ActualShipDate { get; set; }

    public string createJson(string orderID, string sellerID)
    {
        var obj = new OrderShipped();
        obj.OrderId = orderID;
        obj.SellerId = sellerID;
        return JsonSerializer.Serialize(obj);
    }
}