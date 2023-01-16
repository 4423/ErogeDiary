using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using ErogeDiary.Models.Database.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ErogeDiary.ViewModels.Dialogs;

public class VerifiableGame : VerifiableBindableBase
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


    private string? title;
    [Required(ErrorMessage = "タイトルを入力してください。")]
    public string? Title
    {
        get => title;
        set
        {
            SetProperty(ref title, value);
            ValidateProperty(value);
        }
    }

    private string? brand;
    [Required(ErrorMessage = "ブランドを入力してください。")]
    public string? Brand
    {
        get => brand;
        set
        {
            SetProperty(ref brand, value);
            ValidateProperty(value);
        }
    }

    private DateOnly? releaseDate;
    [Required(ErrorMessage = "発売日を入力してください。")]
    public DateOnly? ReleaseDate
    {
        get => releaseDate;
        set
        {
            SetProperty(ref releaseDate, value);
            ValidateProperty(value);
        }
    }

    private string? imageUri;
    [Required(ErrorMessage = "サムネイル画像の場所を入力してください。")]
    [ValidExtensionRequired(ValidExtensions = new string[] { ".jpg", ".jpeg", ".png", ".bmp" })]
    public string? ImageUri
    {
        get => imageUri;
        set
        {
            SetProperty(ref imageUri, value);
            ValidateProperty(value);
        }
    }

    private string? erogameScapeGameId;
    public string? ErogameScapeGameId
    {
        get => erogameScapeGameId;
        set { SetProperty(ref erogameScapeGameId, value); }
    }

    private InstallationType installationType;
    public InstallationType InstallationType
    {
        get => installationType;
        set
        {
            SetProperty(ref installationType, value);

            // InstallationType に依存する validation を再評価
            switch (installationType)
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
    }

    private string? windowTitle;
    [RequiredIf(nameof(InstallationType), InstallationType.DmmGamePlayer, ErrorMessage = "ウィンドウタイトルを入力してください。")]
    public string? WindowTitle
    {
        get => windowTitle;
        set
        {
            SetProperty(ref windowTitle, value);
            ValidateProperty(value);
        }
    }

    private string? executableFilePath;
    [RequiredIf(nameof(InstallationType), InstallationType.Default, ErrorMessage = "実行ファイルの場所を入力してください。")]
    [FileExistRequiredIf(nameof(InstallationType), InstallationType.Default)]
    public string? ExecutableFilePath
    {
        get => executableFilePath;
        set
        {
            SetProperty(ref executableFilePath, value);
            ValidateProperty(value);
        }
    }

    private bool isCleared;
    public bool IsCleared
    {
        get => isCleared;
        set
        {
            SetProperty(ref isCleared, value);
            // IsCleared に依存する validation を再評価
            ValidateProperty(clearedAt, nameof(ClearedAt));
        }
    }

    private DateTime? clearedAt;
    [RequiredIf(nameof(IsCleared), true, ErrorMessage = "攻略日を入力してください。")]
    public DateTime? ClearedAt
    {
        get => clearedAt;
        set
        {
            SetProperty(ref clearedAt, value);
            ValidateProperty(value);
        }
    }

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
