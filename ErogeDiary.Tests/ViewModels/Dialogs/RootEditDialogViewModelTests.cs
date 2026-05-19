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

    [Fact]
    public void SelectedRoot_ReplacesEditableRootAndTracksCurrentRootOnly()
    {
        var rootA = CreateRoot(name: "Route A", playTime: TimeSpan.FromMinutes(10), color: Colors.Red);
        var rootB = CreateRoot(name: "Route B", playTime: TimeSpan.FromMinutes(20), color: Colors.Blue);
        var game = CreateGame(totalPlayTime: TimeSpan.FromMinutes(60));
        game.Roots.Add(rootA);
        game.Roots.Add(rootB);
        var viewModel = new RootEditDialogViewModel(new ErogeDiaryDbContext(), new StubMessageDialog());
        viewModel.OnDialogOpened(CreateDialogParameters(game));

        viewModel.SelectedRoot = rootA;
        var editableRootA = viewModel.SelectedVerifiableRoot;
        Assert.NotNull(editableRootA);

        viewModel.SelectedRoot = rootB;
        var editableRootB = viewModel.SelectedVerifiableRoot;
        Assert.NotNull(editableRootB);
        Assert.NotSame(editableRootA, editableRootB);
        Assert.Equal("Route B", editableRootB.Name);

        var canExecuteChangedCount = 0;
        viewModel.UpdateCommand.CanExecuteChanged += (_, _) => canExecuteChangedCount++;

        editableRootA.Name = "";

        Assert.Equal(0, canExecuteChangedCount);
        Assert.True(viewModel.UpdateCommand.CanExecute(null));

        editableRootB.Name = "";

        Assert.True(canExecuteChangedCount > 0);
        Assert.False(viewModel.UpdateCommand.CanExecute(null));
    }

    [Fact]
    public void SelectedRoot_NullClearsEditableRootAndDisablesUpdate()
    {
        var root = CreateRoot(name: "Route A", playTime: TimeSpan.FromMinutes(10), color: Colors.Red);
        var game = CreateGame(totalPlayTime: TimeSpan.FromMinutes(30));
        game.Roots.Add(root);
        var viewModel = new RootEditDialogViewModel(new ErogeDiaryDbContext(), new StubMessageDialog());
        viewModel.OnDialogOpened(CreateDialogParameters(game));
        viewModel.SelectedRoot = root;
        Assert.NotNull(viewModel.SelectedVerifiableRoot);
        Assert.True(viewModel.UpdateCommand.CanExecute(null));

        viewModel.SelectedRoot = null;

        Assert.Null(viewModel.SelectedVerifiableRoot);
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
        => CreateRoot("Route A", playTime, Colors.Red);

    private static Root CreateRoot(string name, TimeSpan playTime, Color color)
        => new()
        {
            Name = name,
            PlayTime = playTime,
            Color = color,
        };

}
