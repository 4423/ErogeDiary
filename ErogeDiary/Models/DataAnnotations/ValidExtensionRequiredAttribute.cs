using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary.Models.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidExtensionRequiredAttribute : ValidationAttribute
    {
        private string[] validExtensions;
        public string[] ValidExtensions
        {
            get => validExtensions;
            set
            {
                validExtensions = value;
                if (value != null && String.IsNullOrEmpty(ErrorMessage))
                {
                    ErrorMessage = $"有効な拡張子は {String.Join(" ", ValidExtensions)} のいずれかです。";
                }
            }
        }

        public override bool IsValid(object value)
        {
            var fileName = value as string;
            if (fileName != null)
            {
                return ValidExtensions.Contains(Path.GetExtension(fileName));
            }
            return false;
        }
    }
}
