using Prism.Services.Dialogs;
using System;

namespace ErogeDiary.Tests.TestDoubles;

internal sealed class StubDialogService : IDialogService
{
    public IDialogResult Result { get; init; } = new DialogResult(ButtonResult.Cancel);

    public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
    {
        callback(Result);
    }

    public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
    {
        callback(Result);
    }

    public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
    {
        callback(Result);
    }

    public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
    {
        callback(Result);
    }
}
