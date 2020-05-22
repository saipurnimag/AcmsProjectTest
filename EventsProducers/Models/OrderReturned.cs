using System.Text.Json;

public class OrderReturned : Event
{
    public string createJson(string orderId, string sellerId)
    {
        var obj = new OrderReturned();
        obj.OrderId = orderId;
        obj.SellerId = sellerId;
        return JsonSerializer.Serialize(obj);
    }
}