using QuickFix.FIX44;

namespace OrderAccumulator.Services.Interfaces;

public interface IExposureProcessor
{
    ExecutionReport Process(NewOrderSingle order);
}
