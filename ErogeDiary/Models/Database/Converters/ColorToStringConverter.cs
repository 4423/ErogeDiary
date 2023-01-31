using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Windows.Media;

namespace ErogeDiary.Models.Database.Converters;

public class ColorToStringConverter : ValueConverter<Color, string>
{
    public ColorToStringConverter()
        : base(color => color.ToString(), s => (Color)ColorConverter.ConvertFromString(s)) { }
}
