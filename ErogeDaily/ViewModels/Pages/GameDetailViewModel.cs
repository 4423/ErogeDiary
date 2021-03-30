﻿using ErogeDaily.Dialogs;
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
        private IDialogService dialogService;


        public GameDetailViewModel(
            IDatabaseAccess database,
            IRegionManager regionManager,
            IDialogService dialogService)
        {
            this.database = database;
            this.regionManager = regionManager;
            this.dialogService = dialogService;

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
            var result = dialogService.MessageDialog.Show(new MessageDialogParameters()
            {
                Title = "確認",
                Message = "プレイデータを削除しますか？\nセーブデータやゲーム本体は削除されません。",
                Icon = MessageDialogImage.Question,
                Button = MessageDialogButton.YesNo,
            });
            if (result == MessageDialogResult.Yes)
            {
                await database.RemoveAsync(Game);
                dialogService.MessageDialog.Show(new MessageDialogParameters()
                {
                    Title = "情報",
                    Message = "削除に成功しました。",
                    Icon = MessageDialogImage.Information,
                    Button = MessageDialogButton.OK,
                });
                NavigationHelper.GetNavigationService(regionManager)?.Journal?.GoBack();
            }
        }
    }
}
