using ElectronicsStoreApiPractice.DTOs.Requests;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace ElectronicsStoreApiPractice.Tests;

public class CreateProductRequestTests
{
    [Fact]
    public void ValidRequest_ShouldPassValidation()
    {
        var request = new CreateProductRequest
        {
            Name = "Мышь",
            Price = 2500,
            Quantity = 5,
            CategoryId = 1,
            Description = "Игровая мышь"
        };

        var errors = Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void EmptyName_ShouldFailValidation()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Price = 2500,
            Quantity = 5
        };

        var errors = Validate(request);

        Assert.Contains(errors, error =>
            error.MemberNames.Contains(
                nameof(CreateProductRequest.Name)));
    }

    [Fact]
    public void ZeroPrice_ShouldFailValidation()
    {
        var request = new CreateProductRequest
        {
            Name = "Мышь",
            Price = 0,
            Quantity = 5
        };

        var errors = Validate(request);

        Assert.Contains(errors, error =>
            error.MemberNames.Contains(
                nameof(CreateProductRequest.Price)));
    }

    [Fact]
    public void NegativeQuantity_ShouldFailValidation()
    {
        var request = new CreateProductRequest
        {
            Name = "Мышь",
            Price = 2500,
            Quantity = -1
        };

        var errors = Validate(request);

        Assert.Contains(errors, error =>
            error.MemberNames.Contains(
                nameof(CreateProductRequest.Quantity)));
    }

    private static List<ValidationResult> Validate(object instance)
    {
        var context = new ValidationContext(instance);
        var errors = new List<ValidationResult>();

        Validator.TryValidateObject(
            instance,
            context,
            errors,
            validateAllProperties: true);

        return errors;
    }
}
