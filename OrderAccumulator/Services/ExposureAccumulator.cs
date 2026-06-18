using OrderAccumulator.Services.Interfaces;

namespace OrderAccumulator.Services;

public class ExposureAccumulator : IExposureAccumulator
{
    private const int MaxExposure = 100000000;
    private Dictionary<string, decimal> _currentExposure = new Dictionary<string, decimal>()
    {
        { "PETR4", 0 },
        { "VALE3", 0 },
        { "VIIA4", 0 }
    };

    public decimal GetExposure(string symbol) => _currentExposure[symbol];

    public bool IsLessThanMaxExposure(decimal exposure) => exposure <= MaxExposure;

    public void SetExposure(string symbol, decimal exposure) => _currentExposure[symbol] = exposure;
}
