using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using FOE_CourseRegistrationSystem.Models;
using Xunit;

public class StaffTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var validationContext = new ValidationContext(model, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Staff_WithValidData_ShouldPassValidation()
    {
        var staff = new Staff
        {
            StaffID = 1,
            Email = "test@university.com",
            Password = "password123",
            NIC = "123456789V",
            FullName = "John Doe",
            PhoneNo = "0771234567",
            Role = "Admin"
        };

        var results = ValidateModel(staff);

        Assert.Empty(results); // Test passes if there are no validation errors
    }

    [Fact]
    public void Staff_MissingRequiredFields_ShouldFailValidation()
    {
        var staff = new Staff(); //  No fields set

        var results = ValidateModel(staff);

        Assert.NotEmpty(results); // Should have validation errors
        Assert.Contains(results, v => v.ErrorMessage.Contains("The Email field is required."));
        Assert.Contains(results, v => v.ErrorMessage.Contains("The Password field is required."));
        Assert.Contains(results, v => v.ErrorMessage.Contains("The NIC field is required."));
    }

    [Fact]
    public void Staff_InvalidEmail_ShouldFailValidation()
    {
        var staff = new Staff
        {
            StaffID = 2,
            Email = "invalid-email", // Invalid Email Format
            Password = "password123",
            NIC = "987654321V",
            FullName = "Jane Doe",
            PhoneNo = "0777654321",
            Role = "Staff"
        };

        var results = ValidateModel(staff);

        Assert.NotEmpty(results);
        Assert.Contains(results, v => v.ErrorMessage.Contains("The Email field is not a valid e-mail address."));
    }
}
