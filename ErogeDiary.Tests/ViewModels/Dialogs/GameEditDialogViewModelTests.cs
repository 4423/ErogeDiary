using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Tests.TestDoubles;
using ErogeDiary.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using System;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Dialogs;

public class GameEditDialogViewModelTests
{
    [Fact]
    public void UpdateCommand_CanExecute_DependsOnOpenedGameValidity()
    {
        var game = CreateGame();
        var viewModel = new GameEditDialogViewModel(
            new ErogeDiaryDbContext(),
            new StubMessageDialog(),
            new StubOpenFileDialog());

        viewModel.OnDialogOpened(CreateDialogParameters(game));
        Assert.True(viewModel.UpdateCommand.CanExecute(null));

        var verifiableGame = viewModel.VerifiableGame;
        Assert.NotNull(verifiableGame);
        verifiableGame.Title = "";
        Assert.False(viewModel.UpdateCommand.CanExecute(null));
    }

    private static DialogParameters CreateDialogParameters(Game game)
        => new()
        {
            { "game", game },
        };

    private static Game CreateGame()
        => new()
        {
            Title = "Test Game",
            Brand = "Test Brand",
            ReleaseDate = new DateOnly(2024, 1, 1),
            ImageFileName = "cover.jpg",
            InstallationType = InstallationType.DmmGamePlayer,
            WindowTitle = "Test Window",
            RegisteredAt = new DateTime(2024, 1, 2),
            TotalPlayTime = TimeSpan.FromHours(1),
        };

}
