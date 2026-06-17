namespace OrderGenerator.Api.Dtos;

public record OrderResponse(string Message, Guid OrderId, string Status);
