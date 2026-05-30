using ErogeDiary.Models.Database.Entities;

namespace ErogeDiary.Views.Converters;
public class InstallationTypeToBooleanConverter() : BooleanConverter<InstallationType>(InstallationType.DmmGamePlayer,
    InstallationType.Default, BooleanConverterDirection.ToBoolean);
