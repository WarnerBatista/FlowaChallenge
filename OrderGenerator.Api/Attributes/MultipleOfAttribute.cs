using System.ComponentModel.DataAnnotations;

namespace OrderGenerator.Api.Attributes;

public class MultipleOfAttribute : ValidationAttribute
{
    private readonly decimal _factor;
    public MultipleOfAttribute(double factor) => _factor = (decimal)factor;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is decimal d && d % _factor == 0)
            return ValidationResult.Success;

        return new ValidationResult($"The value must be a multiple of {_factor}");
    }
}
