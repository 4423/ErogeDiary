using System;
using ErogeDiary.Properties;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ErogeDiary.Models.DataAnnotations;

public class FileExistRequiredIfAttribute : RequiredIfAttribute
{
    public FileExistRequiredIfAttribute(
        string conditionProperty, 
        object conditionPropertyValue
    ) : base(conditionProperty, conditionPropertyValue)
    {
        ErrorMessageResourceType = typeof(Strings);
        ErrorMessageResourceName = nameof(Strings.Validation_FileNotFound);
    }


    protected override bool IsValidIfShouldValidate(object? value)
    {
        if (value is not string fileName)
        {
            return false;
        }
        return File.Exists(fileName);
    }
}
