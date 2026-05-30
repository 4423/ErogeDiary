using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ErogeDiary.Properties;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ErogeDiary.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private ErogeDiaryDbContext database;
    private GameMonitor gameMonitor;
    private IRegionManager regionManager;
    private IRegionNavigationService navigationService { get =>
        NavigationHelper.GetNavigationService(regionManager); }


    public MainWindowViewModel(ErogeDiaryDbContext database, GameMonitor gameMonitor, IRegionManager regionManager)
    {
        this.database = database;
        this.gameMonitor = gameMonitor;
        this.regionManager = regionManager;

        gameMonitor.GameStarted += GameStarted;
        gameMonitor.GameEnded += GameEnded;
        gameMonitor.ProgressChanged += ProgressChanged;
        Settings.Default.PropertyChanged += SettingsChanged;

        themeAccentBrush = new(ThemeAccentColor.ParseOrDefault(Settings.Default.ThemeAccentColor));
    }


    private void GameStarted(Game game)
    {
        ActiveGame = game;
        TotalPlayTime = game.TotalPlayTime;
        System.Diagnostics.Debug.WriteLine(game.Title);
        IsPlaying = true;
    }

    private async void GameEnded(Game game, TimeSpan playTime)
    {
        IsPlaying = false;
        ActiveGame = null;
        CurrentPlayTime = TimeSpan.Zero;
        TotalPlayTime = TimeSpan.Zero;

        var now = DateTime.Now;

        var playLog = new PlayLog()
        {
            StartedAt = now - playTime,
            EndedAt = now,
            Game = game,
            GameId = game.GameId,
        };
        // TODO: game.PlayLogs に Add したほうがいい？ EF の best practice を見直す
        await database.AddPlayLogAsync(playLog);

        // Game 側の更新は PlayLogs の後にする（Game 側の更新時には関連テーブルが同期されているみたいな雑なルール）
        game.TotalPlayTime += playTime;
        game.LastPlayedAt = now;
        await database.UpdateAsync(game);

        System.Diagnostics.Debug.WriteLine(game.Title);
        System.Diagnostics.Debug.WriteLine(playTime);
    }

    private void ProgressChanged(Game game, TimeSpan currentPlayTime, TimeSpan totalPlayTime)
    {
        CurrentPlayTime = currentPlayTime;
        TotalPlayTime = totalPlayTime;
    }

    private void SettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.ThemeAccentColor))
        {
            ThemeAccentBrush = new(ThemeAccentColor.ParseOrDefault(Settings.Default.ThemeAccentColor));
        }
    }

    [ObservableProperty]
    private Game? activeGame;

    [ObservableProperty]
    private TimeSpan currentPlayTime;

    [ObservableProperty]
    private TimeSpan totalPlayTime;

    [RelayCommand]
    private void GoBack()
    {
        if (navigationService.Journal.CanGoBack)
        {
            navigationService.Journal.GoBack();
        }
    }

    [RelayCommand]
    private void OpenSettings()
    {
        NavigationHelper.RequestNavigateToSettingsPage(regionManager);
    }

    [RelayCommand]
    private void OpenHome()
    {
        NavigationHelper.RequestNavigateToHomePage(regionManager);
    }

    [RelayCommand]
    private void RootFrameNavigated()
    {
        IsBackButtonVisible = navigationService.Journal.CanGoBack;
    }

    [ObservableProperty]
    private bool isBackButtonVisible;

    [ObservableProperty]
    private bool isPlaying;

    [ObservableProperty]
    private SolidColorBrush themeAccentBrush = default!;
}
