using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace ErogeDiary.ViewModels.Dialogs;

public partial class VerifiableRoot : ObservableValidator
{
    //public int RootId { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "ルート名を入力してください。")]
    private string? name;

    // Entities.Root では TimeSpan 型だが、ここでは string 型にしている
    // Converter を使って SelectedRoot.PlayTime の TimeSpan を直接ダイアログ上で編集することも可能だが、
    // 012:34:56 のような文字を入力したときに、012 が Convert → ConvertBack で一度 TimeSpan を経由することで
    // 12 として表示されるという挙動が心地よくないので、あえてここでは単に string で受けている
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "プレイ時間を入力してください。")]
    [TimeSpanFormatRequired]
    private string? playTime;

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

    [ObservableProperty]
    private AccentColor accentColor = default!;

    public void Pretty()
    {
        // 整合性を取る
        if (!IsCleared)
        {
            ClearedAt = null;
        }
    }

    public bool Valid() 
        => !String.IsNullOrWhiteSpace(Name) && !HasErrors;
}
