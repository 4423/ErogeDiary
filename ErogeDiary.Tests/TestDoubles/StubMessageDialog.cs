using ErogeDiary.Dialogs;
using System.Threading.Tasks;

namespace ErogeDiary.Tests.TestDoubles;

internal sealed class StubMessageDialog : IMessageDialog
{
    public Task<MessageDialogResult> ShowAsync(MessageDialogParameters parameters)
        => Task.FromResult(MessageDialogResult.None);
}
