using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ErogeDaily.Models.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FileExistRequiredAttribute : ValidationAttribute
    {
        public FileExistRequiredAttribute()
        {
            ErrorMessage = "指定された場所にファイルが存在しません。";
        }

        public override bool IsValid(object value)
        {
            var fileName = value as string;
            if (fileName != null)
            {
                return File.Exists(fileName);
            }
            return false;
        }
    }
}
