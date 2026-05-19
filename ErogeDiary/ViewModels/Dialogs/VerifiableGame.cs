using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Properties;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ErogeDiary.ViewModels.Dialogs;

public partial class VerifiableGame : ObservableValidator
{
    public VerifiableGame() { }
    public VerifiableGame(GameInfo gameInfo)
    {
        ErogameScapeGameId = gameInfo.Id;
        Title = gameInfo.Title;
        Brand = gameInfo.Brand;
        ReleaseDate = gameInfo.ReleaseDate;
        ImageUri = gameInfo.ImageUri;
    }


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_TitleRequired))]
    private string? title;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_BrandRequired))]
    private string? brand;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_ReleaseDateRequired))]
    private DateOnly? releaseDate;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_ImageUriRequired))]
    [ValidExtensionRequired(".jpg", ".jpeg", ".png", ".bmp")]
    private string? imageUri;

    [ObservableProperty]
    private string? erogameScapeGameId;

    [ObservableProperty]
    private InstallationType installationType;

    partial void OnInstallationTypeChanged(InstallationType value)
    {
        // InstallationType に依存する validation を再評価
        switch (value)
        {
            case InstallationType.Default:
                ValidateProperty(ExecutableFilePath, nameof(ExecutableFilePath));
                ClearErrors(nameof(WindowTitle));
                break;
            case InstallationType.DmmGamePlayer:
                ValidateProperty(WindowTitle, nameof(WindowTitle));
                ClearErrors(nameof(ExecutableFilePath));
                break;
        };
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(InstallationType), InstallationType.DmmGamePlayer, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_WindowTitleRequired))]
    private string? windowTitle;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(InstallationType), InstallationType.Default, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_ExecutableFilePathRequired))]
    [FileExistRequiredIf(nameof(InstallationType), InstallationType.Default)]
    private string? executableFilePath;

    [ObservableProperty]
    private bool isCleared;

    partial void OnIsClearedChanged(bool value)
    {
        // IsCleared に依存する validation を再評価
        ValidateProperty(ClearedAt, nameof(ClearedAt));
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(IsCleared), true, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = nameof(Strings.Validation_ClearedAtRequired))]
    private DateTime? clearedAt;

    public bool Valid()
    {
        var hasNullOrWhiteSpaceProperties =
            String.IsNullOrWhiteSpace(Title)
            || String.IsNullOrWhiteSpace(Brand)
            || String.IsNullOrWhiteSpace(ImageUri)
            || InstallationType switch
            {
                InstallationType.Default => String.IsNullOrWhiteSpace(ExecutableFilePath),
                InstallationType.DmmGamePlayer => String.IsNullOrWhiteSpace(WindowTitle),
            };

        return !hasNullOrWhiteSpaceProperties && !HasErrors;
    }

    public void Pretty()
    {
        // 整合性を取る
        switch (InstallationType)
        {
            case InstallationType.Default:
                WindowTitle = null;
                break;
            case InstallationType.DmmGamePlayer:
                ExecutableFilePath = null;
                break;
        }

        if (!IsCleared)
        {
            ClearedAt = null;
        }
    }

    // 各プロパティが non null の場合に呼び出すこと
    public void CopyTo(ref Game game)
    {
        game.Title = Title!;
        game.Brand = Brand!;
        game.ReleaseDate = ReleaseDate!.Value;
        game.ImageFileName = Path.GetFileName(ImageUri!);
        game.ErogameScapeGameId = ErogameScapeGameId;
        game.InstallationType = InstallationType;
        game.WindowTitle = WindowTitle;
        game.ExecutableFilePath = ExecutableFilePath;
        game.ClearedAt = ClearedAt;
    }
}
