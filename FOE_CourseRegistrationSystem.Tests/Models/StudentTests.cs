using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FOE_CourseRegistrationSystem.Models;
using Xunit;

public class StudentTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var validationContext = new ValidationContext(model, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Student_WithValidData_ShouldPassValidation()
    {
        var student = new Student
        {
            StudentID = 1,
            Email = "student@university.com",
            Password = "password123",
            NIC = "123456789V",
            FullName = "John Doe",
            Nationality = "Sri Lankan",
            PhoneNo = "0771234567",
            AcademicYear = 3
        };

        var results = ValidateModel(student);
        Assert.Empty(results); // Test passes if there are no validation errors
    }

    [Fact]
    public void Student_MissingRequiredFields_ShouldFailValidation()
    {
        var student = new Student(); // No required fields set

        var results = ValidateModel(student);

        Assert.NotEmpty(results); // Should have validation errors
        Assert.Contains(results, v => v.ErrorMessage.Contains("The Email field is required."));
        Assert.Contains(results, v => v.ErrorMessage.Contains("The Password field is required."));
        Assert.Contains(results, v => v.ErrorMessage.Contains("The NIC field is required."));
    }

    [Fact]
    public void Student_InvalidEmail_ShouldFailValidation()
    {
        var student = new Student
        {
            Email = "invalid-email", // Invalid email format
            Password = "password123",
            NIC = "987654321V",
            FullName = "Jane Doe",
            Nationality = "Indian",
            PhoneNo = "0777654321",
            AcademicYear = 2
        };

        var results = ValidateModel(student);

        Assert.NotEmpty(results);
        Assert.Contains(results, v => v.ErrorMessage.Contains("The Email field is not a valid e-mail address."));
    }
}
