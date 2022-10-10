using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDaily.Views.Converters;

public class InverseBooleanConverter : BooleanConverter<bool>
{
    public InverseBooleanConverter() : base(false, true, BooleanConverterDirection.FromBoolean) { }
}
