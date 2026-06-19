using OrderAccumulator.Extensions;
using OrderAccumulator.Services.Interfaces;
using QuickFix.FIX44;

namespace OrderAccumulator.Services;

public class SellExposureProcessor : IExposureProcessor
{
    private readonly IExposureAccumulator _exposureAccumulator;

    public SellExposureProcessor(IExposureAccumulator exposureAccumulator)
    {
        _exposureAccumulator = exposureAccumulator ?? throw new ArgumentNullException(nameof(exposureAccumulator));
    }

    public ExecutionReport Process(NewOrderSingle order)
    {
        var exposure = _exposureAccumulator.GetExposure(order.Symbol.Value) - (order.OrderQty.Value * order.Price.Value);

        _exposureAccumulator.SetExposure(order.Symbol.Value, exposure);
        return order.GenerateNewExecutionReport();
    }
}
