using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.ViewModels.Contents;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace ErogeDiary.ViewModels.Pages
{
    public partial class GameDetailViewModel : ObservableObject, INavigationAware
    {
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
        }


        [ObservableProperty]
        private Game? game;

        partial void OnGameChanged(Game? value)
        {
            Roots = value == null ? null : new RootsViewModel(dialogService, value);
            PlayLogs = value == null ? null : new PlayLogsViewModel(value, database);
        }

        [ObservableProperty]
        private RootsViewModel? roots;

        [ObservableProperty]
        private PlayLogsViewModel? playLogs;


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

        [RelayCommand]
        private async Task StartGameAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Game?.ExecutableFilePath))
                {
                    await messageDialog.ShowErrorAsync("実行ファイルのパスが設定されていません。");
                    return;
                }

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

        [RelayCommand]
        private void EditGame()
        {
            var dialogParams = new DialogParameters()
            {
                { "game", Game }
            };
            dialogService.ShowDialog(nameof(Views.Dialogs.GameEditDialog), dialogParams, null);
        }

        [RelayCommand]
        private async Task DeleteGameAsync()
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
                if (Game == null)
                {
                    await messageDialog.ShowErrorAsync("ゲームの情報が見つからないため解除できませんでした。");
                    return;
                }

                await database.RemoveAsync(Game);
                await messageDialog.ShowAsync(new MessageDialogParameters()
                {
                    Title = "情報",
                    Message = "解除に成功しました。",
                    CloseButtonText = "OK",
                });
                NavigationHelper.GetNavigationService(regionManager)?.Journal?.GoBack();
            }
        }
    }
}
