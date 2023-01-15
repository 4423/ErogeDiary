using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ErogeDiary.ViewModels.Dialogs;

public class VerifiableRoot : VerifiableBindableBase
{
    //public int RootId { get; set; }

    private string? name;
    [Required(ErrorMessage = "ルート名を入力してください。")]
    public string? Name
    {
        get => name;
        set
        {
            SetProperty(ref name, value);
            ValidateProperty(value);
        }
    }

    // Entities.Root では TimeSpan 型だが、ここでは string 型にしている
    // Converter を使って SelectedRoot.PlayTime の TimeSpan を直接ダイアログ上で編集することも可能だが、
    // 012:34:56 のような文字を入力したときに、012 が Convert → ConvertBack で一度 TimeSpan を経由することで
    // 12 として表示されるという挙動が心地よくないので、あえてここでは単に string で受けている
    private string? playTime;
    [Required(ErrorMessage = "プレイ時間を入力してください。")]
    [TimeSpanFormatRequired]
    public string? PlayTime
    {
        get { return playTime; }
        set
        {
            SetProperty(ref playTime, value);
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
            ValidateProperty(ClearedAt, nameof(ClearedAt));
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
