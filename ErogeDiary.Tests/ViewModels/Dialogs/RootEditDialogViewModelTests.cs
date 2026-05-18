using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Tests.TestDoubles;
using ErogeDiary.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using System;
using System.Windows.Media;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Dialogs;

public class RootEditDialogViewModelTests
{
    [Fact]
    public void UpdateCommand_CanExecute_DependsOnSelectedAndValidRoot()
    {
        var root = CreateRoot(playTime: TimeSpan.FromMinutes(10));
        var game = CreateGame(totalPlayTime: TimeSpan.FromMinutes(30));
        game.Roots.Add(root);
        var viewModel = new RootEditDialogViewModel(new ErogeDiaryDbContext(), new StubMessageDialog());

        viewModel.OnDialogOpened(CreateDialogParameters(game));
        Assert.False(viewModel.UpdateCommand.CanExecute(null));

        viewModel.SelectedRoot = root;
        Assert.True(viewModel.UpdateCommand.CanExecute(null));

        var selectedVerifiableRoot = viewModel.SelectedVerifiableRoot;
        Assert.NotNull(selectedVerifiableRoot);
        selectedVerifiableRoot.Name = "";
        Assert.False(viewModel.UpdateCommand.CanExecute(null));
    }

    private static DialogParameters CreateDialogParameters(Game game)
        => new()
        {
            { "game", game },
        };

    private static Game CreateGame(TimeSpan totalPlayTime)
        => new()
        {
            Title = "Test Game",
            Brand = "Test Brand",
            ReleaseDate = new DateOnly(2024, 1, 1),
            ImageFileName = "cover.jpg",
            InstallationType = InstallationType.DmmGamePlayer,
            WindowTitle = "Test Window",
            RegisteredAt = new DateTime(2024, 1, 2),
            TotalPlayTime = totalPlayTime,
        };

    private static Root CreateRoot(TimeSpan playTime)
        => new()
        {
            Name = "Route A",
            PlayTime = playTime,
            Color = Colors.Red,
        };

}
