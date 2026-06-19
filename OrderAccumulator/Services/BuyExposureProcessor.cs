using OrderAccumulator.Extensions;
using OrderAccumulator.Services.Interfaces;
using QuickFix.FIX44;

namespace OrderAccumulator.Services;

public class BuyExposureProcessor : IExposureProcessor
{
    private readonly IExposureAccumulator _exposureAccumulator;

    public BuyExposureProcessor(IExposureAccumulator exposureAccumulator)
    {
        _exposureAccumulator = exposureAccumulator ?? throw new ArgumentNullException(nameof(exposureAccumulator));
    }

    public ExecutionReport Process(NewOrderSingle order)
    {
        var exposure = order.OrderQty.Value * order.Price.Value + _exposureAccumulator.GetExposure(order.Symbol.Value);

        if (_exposureAccumulator.IsLessThanMaxExposure(exposure))
        {
            _exposureAccumulator.SetExposure(order.Symbol.Value, exposure);
            return order.GenerateNewExecutionReport();
        }
        else
            return order.GenerateRejectedExecutionReport();
    }
}
