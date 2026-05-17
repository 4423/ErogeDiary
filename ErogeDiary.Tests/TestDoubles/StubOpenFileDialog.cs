using ErogeDiary.Dialogs;

namespace ErogeDiary.Tests.TestDoubles;

internal sealed class StubOpenFileDialog : IOpenFileDialog
{
    public string? Show(string title, string filter) => null;
}
