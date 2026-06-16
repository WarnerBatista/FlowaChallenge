using OrderGenerator.Api.Dtos;

namespace OrderGenerator.Api.Services.Interfaces;

public interface IFixInitiatorService
{
    void Start();
    void Stop();
    void SendOrder(OrderDto order);
}
