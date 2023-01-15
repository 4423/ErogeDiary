using ErogeDiary.Models.Database.Entities;

namespace ErogeDiary.Views.Converters;
public class InstallationTypeToBooleanConverter : BooleanConverter<InstallationType>
{
    public InstallationTypeToBooleanConverter() :
        base(InstallationType.DmmGamePlayer, InstallationType.Default, BooleanConverterDirection.ToBoolean) { }
}
