using OrderAccumulator.Services;

namespace OrderAccumulator.UnitTests.Services;

public class ExposureAccumulatorTests
{
    [Fact]
    public void GetExposure_ShouldReturnCurrentExposure_ForGivenSymbol()
    {
        // Arrange
        var sut = new ExposureAccumulator();
        string symbol = "PETR4";
        decimal expectedExposure = 5000m;
        sut.SetExposure(symbol, expectedExposure);

        // Act
        var actualExposure = sut.GetExposure(symbol);

        // Assert
        Assert.Equal(expectedExposure, actualExposure);
    }

    [Fact]
    public void IsLessThanMaxExposure_ShouldReturnTrue_WhenExposureIsBelowMax()
    {
        // Arrange
        var sut = new ExposureAccumulator();
        decimal exposure = 5000m;

        // Act
        var result = sut.IsLessThanMaxExposure(exposure);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsLessThanMaxExposure_ShouldReturnFalse_WhenExposureIsAboveMax()
    {
        // Arrange
        var sut = new ExposureAccumulator();
        decimal exposure = 150000000m;

        // Act
        var result = sut.IsLessThanMaxExposure(exposure);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SetExposure_ShouldUpdateExposure_ForGivenSymbol()
    {
        // Arrange
        var sut = new ExposureAccumulator();
        string symbol = "PETR4";
        decimal newExposure = 7500m;

        // Act
        sut.SetExposure(symbol, newExposure);
        var actualExposure = sut.GetExposure(symbol);

        // Assert
        Assert.Equal(newExposure, actualExposure);
    }
}
