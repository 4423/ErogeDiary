using ErogeDaily.Models;

namespace ErogeDaily.Views.Converters;
public class InstallationTypeToBooleanConverter : BooleanConverter<InstallationType>
{
    public InstallationTypeToBooleanConverter() :
        base(InstallationType.DmmGamePlayer, InstallationType.Default, BooleanConverterDirection.ToBoolean) { }
}
