using OrderGenerator.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OrderGenerator.Api.Dtos;

public record OrderDto(
    [Required]
    [RegularExpression("^(PETR4|VALE3|VIIA4)$",
    ErrorMessage = "The Symbol field must be one of the following values: PETR4, VALE3 ou VIIA4.")]
    string Symbol,

    [Required]
    [RegularExpression("(?i)^(Buy|Sell)$",
    ErrorMessage = "The Side field must be either 'Buy' or 'Sell' (case insensitive).")]
    string Side,

    [Range(1, 99999)]
    int Quantity,

    [Range(0.01, 999.99)]
    [MultipleOf(0.01)]
    decimal Price
);

