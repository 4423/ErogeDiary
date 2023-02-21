using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.ViewModels.Contents;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
namespace ErogeDiary.ViewModels.Pages
{
    public class GameDetailViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand StartGameCommand { get; private set; }
        public DelegateCommand EditGameCommand { get; private set; }
        public DelegateCommand DeleteGameCommand { get; private set; }

        private ErogeDiaryDbContext database;
        private IRegionManager regionManager;
        private IMessageDialog messageDialog;
        private IDialogService dialogService;


        public GameDetailViewModel(
            ErogeDiaryDbContext database,
            IRegionManager regionManager,
            IMessageDialog messageDialog,
            IDialogService dialogService)
        {
            this.database = database;
            this.regionManager = regionManager;
            this.messageDialog = messageDialog;
            this.dialogService = dialogService;

            StartGameCommand = new DelegateCommand(StartGame);
            EditGameCommand = new DelegateCommand(EditGame);
            DeleteGameCommand = new DelegateCommand(DeleteGame);
        }


        private Game? game;
        public Game? Game
        {
            get => game;
            set
            {
                SetProperty(ref game, value);
                Roots = game == null ? null : new RootsViewModel(dialogService, game);
                PlayLogs = game == null ? null : new PlayLogsViewModel(game, database);
            }
        }

        private RootsViewModel? roots;
        public RootsViewModel? Roots
        {
            get => roots;
            set { SetProperty(ref roots, value); }
        }

        private PlayLogsViewModel? playLogs;
        public PlayLogsViewModel? PlayLogs
        {
            get => playLogs;
            set { SetProperty(ref playLogs, value); }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var game = navigationContext.Parameters["Game"] as Game;
            if(game != null)
            {
                Game = game;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) {}

        private async void StartGame()
        {
            try
            {
                Process.Start(Game.ExecutableFilePath);
            }
            catch (Exception ex)
            {
                await messageDialog.ShowAsync(new MessageDialogParameters()
                {
                    Title = "エラー",
                    Message = $"ゲームの起動に失敗しました。\n{ex.Message}",
                    CloseButtonText = "OK",
                });
            }
        }

        private void EditGame()
        {
            var dialogParams = new DialogParameters()
            {
                { "game", Game }
            };
            dialogService.ShowDialog(nameof(Views.Dialogs.GameEditDialog), dialogParams, null);
        }

        private async void DeleteGame()
        {
            var result = await messageDialog.ShowAsync(new MessageDialogParameters()
            {
                Title = "確認",
                Message = "ゲームの登録を解除しますか？\nセーブデータやゲーム本体は削除されません。",
                PrimaryButtonText = "削除",
                CloseButtonText = "キャンセル",
            });
            if (result == MessageDialogResult.Primary)
            {
                await database.RemoveAsync(Game);
                await messageDialog.ShowAsync(new MessageDialogParameters()
                {
                    Title = "情報",
                    Message = "削除に成功しました。",
                    CloseButtonText = "OK",
                });
                NavigationHelper.GetNavigationService(regionManager)?.Journal?.GoBack();
            }
        }
    }
}
