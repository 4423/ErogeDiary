using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace ErogeDiary.ViewModels.Dialogs;

public class BindableDialogBase : BindableBase, IDialogAware
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

    protected virtual void RaiseRequestClose(IDialogResult dialogResult)
        => RequestClose?.Invoke(dialogResult);
}
