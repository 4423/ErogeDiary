using ErogeDiary.Models.Database.Entities;
using ErogeDiary.ViewModels.Contents;
using System;
using System.Linq;
using System.Windows.Media;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Contents;

public class RootsViewModelTests
{
    [Fact]
    public void CommandsAndChartData_ReflectGameWithoutPlayTime()
    {
        var game = CreateGame(totalPlayTime: TimeSpan.Zero);

        var viewModel = new RootsViewModel(dialogService: null!, game);

        Assert.False(viewModel.AddRootCommand.CanExecute(null));
        Assert.False(viewModel.EditRootCommand.CanExecute(null));
        Assert.False(viewModel.RemoveRootCommand.CanExecute(null));
        var chartData = Assert.Single(viewModel.RootChartDataList);
        Assert.Equal("No Data", chartData.Label);
    }

    [Fact]
    public void CommandsAndChartData_ReflectUnallocatedPlayTimeWithoutRoots()
    {
        var game = CreateGame(totalPlayTime: TimeSpan.FromMinutes(30));

        var viewModel = new RootsViewModel(dialogService: null!, game);

        Assert.True(viewModel.AddRootCommand.CanExecute(null));
        Assert.False(viewModel.EditRootCommand.CanExecute(null));
        Assert.False(viewModel.RemoveRootCommand.CanExecute(null));
        var chartData = Assert.Single(viewModel.RootChartDataList);
        Assert.Equal("（未割り当てのルート）", chartData.Label);
        Assert.Equal(TimeSpan.FromMinutes(30).TotalSeconds, chartData.Value);
    }

    [Fact]
    public void CommandsAndChartData_ReflectExistingRoots()
    {
        var game = CreateGame(totalPlayTime: TimeSpan.FromMinutes(30));
        game.Roots.Add(new Root
        {
            Name = "Route A",
            PlayTime = TimeSpan.FromMinutes(10),
            Color = Colors.Red,
        });

        var viewModel = new RootsViewModel(dialogService: null!, game);

        Assert.True(viewModel.AddRootCommand.CanExecute(null));
        Assert.True(viewModel.EditRootCommand.CanExecute(null));
        Assert.True(viewModel.RemoveRootCommand.CanExecute(null));
        Assert.Contains(viewModel.RootChartDataList, x => x.Label == "Route A");
        var unallocated = viewModel.RootChartDataList.Single(x => x.Label == "（未割り当てのルート）");
        Assert.Equal(TimeSpan.FromMinutes(20).TotalSeconds, unallocated.Value);
    }

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
