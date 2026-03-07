using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ErogeDiary.Models.DataAnnotations
{
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

            return $"有効な拡張子は {String.Join(" ", validExtensions)} のいずれかです。";
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
}
