using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ErogeDaily.Models.DataAnnotations
{
    public class FileExistRequiredIfAttribute : RequiredIfAttribute
    {
        public FileExistRequiredIfAttribute(
            string conditionProperty, 
            object conditionPropertyValue
        ) : base(conditionProperty, conditionPropertyValue)
        {
            ErrorMessage = "指定された場所にファイルが存在しません。";
        }


        protected override bool IsValidIfShouldValidate(object? value)
        {
            var fileName = value as string;
            if (fileName == null)
            {
                return false;
            }
            return File.Exists(fileName);
        }
    }
}
