using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using ErogeDiary.Models.Database.Entities;
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
    [Required(ErrorMessage = "タイトルを入力してください。")]
    private string? title;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "ブランドを入力してください。")]
    private string? brand;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "発売日を入力してください。")]
    private DateOnly? releaseDate;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "サムネイル画像の場所を入力してください。")]
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
    [RequiredIf(nameof(InstallationType), InstallationType.DmmGamePlayer, ErrorMessage = "ウィンドウタイトルを入力してください。")]
    private string? windowTitle;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(InstallationType), InstallationType.Default, ErrorMessage = "実行ファイルの場所を入力してください。")]
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
    [RequiredIf(nameof(IsCleared), true, ErrorMessage = "攻略日を入力してください。")]
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
