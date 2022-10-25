using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ErogeDiary.Models.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TimeSpanFormatRequiredAttribute : ValidationAttribute
    {
        public TimeSpanFormatRequiredAttribute()
        {
            ErrorMessage = "不正な時間の形式です。";
        }

        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                var s = value.ToString();
                return TimeSpanHelper.TryParseWithoutDays(s, out _);
            }
            return false;
        }
    }
}
