using System;
using System.Text.Json;

public class OrderCancelled : Event
{
    public string CancellationOrigin, CancellationReason;

    public string createJson(string orderId, string sellerId, string cancellationOrigin, string cancellationReason)
    {
        var date1 = new DateTime();
        var obj = new OrderCancelled();
        obj.OrderId = orderId;
        obj.SellerId = sellerId;
        obj.CancellationOrigin = cancellationOrigin;
        obj.CancellationReason = cancellationReason;
        return JsonSerializer.Serialize(obj);
    }
}