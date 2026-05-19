using ErogeDiary.Dialogs;
using ErogeDiary.ErogameScape;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Tests.TestDoubles;
using ErogeDiary.ViewModels.Dialogs;
using System;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Dialogs;

public class GameRegistrationDialogViewModelTests
{
    [Fact]
    public void RegisterCommand_CanExecute_DependsOnValidGame()
    {
        var viewModel = new GameRegistrationDialogViewModel(
            new ErogeDiaryDbContext(),
            new ErogameScapeClient(),
            new StubMessageDialog(),
            new StubOpenFileDialog());

        Assert.False(viewModel.RegisterCommand.CanExecute(null));

        FillValidGame(viewModel.VerifiableGame);
        Assert.True(viewModel.RegisterCommand.CanExecute(null));
    }

    [Fact]
    public void RegisterCommand_TracksReplacementGameOnly()
    {
        var viewModel = CreateViewModel();
        var originalGame = viewModel.VerifiableGame;
        FillValidGame(originalGame);
        Assert.True(viewModel.RegisterCommand.CanExecute(null));

        viewModel.VerifiableGame = new VerifiableGame();
        var canExecuteChangedCount = 0;
        viewModel.RegisterCommand.CanExecuteChanged += (_, _) => canExecuteChangedCount++;

        originalGame.Title = "";

        Assert.Equal(0, canExecuteChangedCount);
        Assert.False(viewModel.RegisterCommand.CanExecute(null));

        FillValidGame(viewModel.VerifiableGame);

        Assert.True(canExecuteChangedCount > 0);
        Assert.True(viewModel.RegisterCommand.CanExecute(null));
    }

    private static GameRegistrationDialogViewModel CreateViewModel()
        => new(
            new ErogeDiaryDbContext(),
            new ErogameScapeClient(),
            new StubMessageDialog(),
            new StubOpenFileDialog());

    private static void FillValidGame(VerifiableGame game)
    {
        game.Title = "Test Game";
        game.Brand = "Test Brand";
        game.ReleaseDate = new DateOnly(2024, 1, 1);
        game.ImageUri = "cover.jpg";
        game.InstallationType = InstallationType.DmmGamePlayer;
        game.WindowTitle = "Test Window";
    }

}
