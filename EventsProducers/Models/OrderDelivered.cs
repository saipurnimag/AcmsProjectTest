using System;
using System.Text.Json;

public class OrderDelivered : Event
{
    public DateTime ActualDeliveryDate { get; set; }

    public string CreateJson(string orderID, string sellerId)
    {
        var date1 = new DateTime();
        var obj = new OrderDelivered();
        obj.OrderId = orderID;
        obj.SellerId = sellerId;
        obj.ActualDeliveryDate = DateTime.Now;
        return JsonSerializer.Serialize(obj);
    }
}