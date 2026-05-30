using System;
using ErogeDiary.Properties;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ErogeDiary.Models.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidExtensionRequiredAttribute : ValidationAttribute
{
    private readonly string[] validExtensions;

    public ValidExtensionRequiredAttribute(params string[] validExtensions)
    {
        if (validExtensions == null || validExtensions.Length == 0)
        {
            throw new ArgumentException(nameof(validExtensions));
        }

        this.validExtensions = validExtensions;
    }

    public override string FormatErrorMessage(string name)
    {
        if (!String.IsNullOrEmpty(ErrorMessage))
        {
            return base.FormatErrorMessage(name);
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            Strings.Validation_ValidExtensionsFormat,
            String.Join(" ", validExtensions));
    }

    public override bool IsValid(object? value)
    {
        var fileName = value as string;
        if (fileName != null)
        {
            return validExtensions.Contains(Path.GetExtension(fileName));
        }
        return false;
    }
}
