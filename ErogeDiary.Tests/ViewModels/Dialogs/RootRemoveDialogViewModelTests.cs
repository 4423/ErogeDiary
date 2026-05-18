using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Tests.TestDoubles;
using ErogeDiary.ViewModels.Dialogs;
using System;
using System.Windows.Media;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Dialogs;

public class RootRemoveDialogViewModelTests
{
    [Fact]
    public void RemoveCommand_CanExecute_DependsOnSelectedRoot()
    {
        var viewModel = new RootRemoveDialogViewModel(new ErogeDiaryDbContext(), new StubMessageDialog());

        viewModel.SelectedRoot = null;
        Assert.False(viewModel.RemoveCommand.CanExecute(null));

        viewModel.SelectedRoot = CreateRoot();
        Assert.True(viewModel.RemoveCommand.CanExecute(null));
    }

    private static Root CreateRoot()
        => new()
        {
            Name = "Route A",
            PlayTime = TimeSpan.FromMinutes(10),
            Color = Colors.Red,
        };

}
