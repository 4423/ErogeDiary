using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand RootFrameNavigatedCommand { get; private set; }

        private IDatabaseAccess database;
        private GameMonitor gameMonitor;
        private IRegionManager regionManager;
        private IRegionNavigationService navigationService { get =>
            NavigationHelper.GetNavigationService(regionManager); }


        public MainWindowViewModel(IDatabaseAccess database, GameMonitor gameMonitor, IRegionManager regionManager)
        {
            this.database = database;
            this.gameMonitor = gameMonitor;
            this.regionManager = regionManager;

            gameMonitor.GameStarted += GameStarted;
            gameMonitor.GameEnded += GameEnded;
            gameMonitor.ProgressChanged += ProgressChanged;

            GoBackCommand = new DelegateCommand(GoBack);
            RootFrameNavigatedCommand = new DelegateCommand(RootFrameNavigated);
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

            game.TotalPlayTime += playTime;
            game.LatestDate = DateTime.Now;
            await database.UpdateAsync(game);

            var playLog = new PlayLog(game.Id, playTime);
            await database.AddPlayLogAsync(playLog);

            System.Diagnostics.Debug.WriteLine(game.Title);
            System.Diagnostics.Debug.WriteLine(playTime);
        }

        private void ProgressChanged(Game game, TimeSpan currentPlayTime, TimeSpan totalPlayTime)
        {
            CurrentPlayTime = currentPlayTime;
            TotalPlayTime = totalPlayTime;
        }

        private Game activeGame;
        public Game ActiveGame
        {
            get => activeGame;
            set { SetProperty(ref activeGame, value); }
        }

        private TimeSpan currentPlayTime;
        public TimeSpan CurrentPlayTime
        {
            get => currentPlayTime;
            set { SetProperty(ref currentPlayTime, value); }
        }

        private TimeSpan totalPlayTime;
        public TimeSpan TotalPlayTime
        {
            get => totalPlayTime;
            set { SetProperty(ref totalPlayTime, value); }
        }

        private void GoBack()
        {
            if (navigationService.Journal.CanGoBack)
            {
                navigationService.Journal.GoBack();
            }
        }

        private void RootFrameNavigated()
        {
            IsBackButtonVisible = navigationService.Journal.CanGoBack;
        }

        private bool isBackButtonVisible;
        public bool IsBackButtonVisible
        {
            get => isBackButtonVisible;
            set { SetProperty(ref isBackButtonVisible, value); }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                SetProperty(ref isPlaying, value);
                UpdateFrameHeight();
            }
        }

        private void UpdateFrameHeight()
        {
            var h = IsPlaying ? 58 : 18;
            FrameHeight = WindowHeight - h;
        }

        public int windowHeight = 470;
        public int WindowHeight
        {
            get => windowHeight;
            set
            {
                SetProperty(ref windowHeight, value);
                UpdateFrameHeight();
            }
        }

        public int frameHeight;
        public int FrameHeight
        {
            get => frameHeight;
            set { SetProperty(ref frameHeight, value); }
        }
    }
}
