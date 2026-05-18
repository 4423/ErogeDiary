using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Tests.TestDoubles;
using ErogeDiary.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using System;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Dialogs;

public class RootRegistrationDialogViewModelTests
{
    [Fact]
    public void RegisterCommand_CanExecute_DependsOnValidRootAndGame()
    {
        var viewModel = new RootRegistrationDialogViewModel(new ErogeDiaryDbContext(), new StubMessageDialog());

        viewModel.OnDialogOpened(CreateDialogParameters(CreateGame(totalPlayTime: TimeSpan.FromMinutes(30))));
        var verifiableRoot = viewModel.VerifiableRoot;
        Assert.NotNull(verifiableRoot);
        verifiableRoot.Name = "Route A";
        Assert.True(viewModel.RegisterCommand.CanExecute(null));

        verifiableRoot.Name = "";
        Assert.False(viewModel.RegisterCommand.CanExecute(null));
    }

    [Fact]
    public void RegisterCommand_ReevaluatesAfterValidationErrorsAreCleared()
    {
        var viewModel = new RootRegistrationDialogViewModel(new ErogeDiaryDbContext(), new StubMessageDialog());
        viewModel.OnDialogOpened(CreateDialogParameters(CreateGame(totalPlayTime: TimeSpan.FromMinutes(30))));
        var verifiableRoot = viewModel.VerifiableRoot;
        Assert.NotNull(verifiableRoot);
        verifiableRoot.Name = "";
        var canExecuteAfterChange = false;
        viewModel.RegisterCommand.CanExecuteChanged += (_, _) =>
        {
            canExecuteAfterChange = viewModel.RegisterCommand.CanExecute(null);
        };

        verifiableRoot.Name = "Route A";

        Assert.True(canExecuteAfterChange);
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

}
