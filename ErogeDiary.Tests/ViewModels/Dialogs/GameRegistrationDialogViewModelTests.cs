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

        Assert.False(viewModel.RegisterCommand.CanExecute());

        FillValidGame(viewModel.VerifiableGame);
        Assert.True(viewModel.RegisterCommand.CanExecute());
    }

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
