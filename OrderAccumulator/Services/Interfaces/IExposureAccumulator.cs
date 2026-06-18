namespace OrderAccumulator.Services.Interfaces;

public interface IExposureAccumulator
{
    decimal GetExposure(string symbol);
    void SetExposure(string symbol, decimal exposure);
    bool IsLessThanMaxExposure(decimal exposure);
}
