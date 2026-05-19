using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using System;

namespace ErogeDiary.ViewModels.Dialogs;

public partial class BindableDialogBase : ObservableObject, IDialogAware
{
    public string Title => "";

    public event Action<IDialogResult>? RequestClose;

    public virtual bool CanCloseDialog() => true;

    public virtual void OnDialogClosed() { }

    public virtual void OnDialogOpened(IDialogParameters parameters) { }


    protected void CloseDialogOK()
        => RaiseRequestClose(new DialogResult(ButtonResult.OK));

    protected void CloseDialogCancel()
        => RaiseRequestClose(new DialogResult(ButtonResult.Cancel));

    [RelayCommand]
    private void Cancel()
        => CloseDialogCancel();

    protected virtual void RaiseRequestClose(IDialogResult dialogResult)
        => RequestClose?.Invoke(dialogResult);
}
