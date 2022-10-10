using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ErogeDaily.Models.DataAnnotations;
    
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredIfAttribute : ValidationAttribute
{
    public string ConditionProperty { get; private set; }
    public object ConditionPropertyValue { get; private set; }
    public override bool RequiresValidationContext { get { return true; } }


    public RequiredIfAttribute(string conditionProperty, object conditionPropertyValue)
    {
        ConditionProperty = conditionProperty;
        ConditionPropertyValue = conditionPropertyValue;
    }


    protected virtual bool IsValidIfShouldValidate(object? value)
    {
        return value != null && !String.IsNullOrEmpty(value as string);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var conditionProperty = validationContext.ObjectType.GetProperty(ConditionProperty);
        if (conditionProperty == null)
        {
            return new ValidationResult(
                string.Format(CultureInfo.CurrentCulture, "Could not find a property named '{0}'.", ConditionProperty));
        }

        var conditionValue = conditionProperty.GetValue(validationContext.ObjectInstance);
        var shouldValidate = object.Equals(conditionValue, ConditionPropertyValue);
        if (shouldValidate && !IsValidIfShouldValidate(value))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }
}