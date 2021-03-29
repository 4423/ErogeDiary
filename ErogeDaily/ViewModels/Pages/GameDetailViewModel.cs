using ErogeDaily.Dialog;
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

namespace ErogeDaily.ViewModels.Pages
{
    public class GameDetailViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand DeleteGameCommand { get; private set; }

        private IDatabaseAccess database;
        private IRegionManager regionManager;
        private IMessageBoxDialog messageDialog;


        public GameDetailViewModel(
            IDatabaseAccess database,
            IRegionManager regionManager,
            IMessageBoxDialog messageDialog)
        {
            this.database = database;
            this.regionManager = regionManager;
            this.messageDialog = messageDialog;

            DeleteGameCommand = new DelegateCommand(DeleteGame);
        }


        private Game game;
        public Game Game
        {
            get => game; 
            set { SetProperty(ref game, value); }
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


        private async void DeleteGame()
        {
            var isOk = messageDialog.ShowYesNoDialog(
                "プレイデータを削除しますか？\nセーブデータやゲーム本体は削除されません。", "確認");
            if (isOk)
            {
                await database.RemoveAsync(Game);
                messageDialog.ShowInfoDialog("削除に成功しました。", "情報");
                NavigationHelper.GetNavigationService(regionManager)?.Journal?.GoBack();
            }
        }
    }
}
