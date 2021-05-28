using ErogeDaily.Controls;
using ErogeDaily.Dialogs;
using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ErogeDaily.ViewModels.Pages
{
    public class GameDetailViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand StartGameCommand { get; private set; }
        public DelegateCommand EditGameCommand { get; private set; }
        public DelegateCommand DeleteGameCommand { get; private set; }

        private IDatabaseAccess database;
        private IRegionManager regionManager;
        private IMessageDialog messageDialog;
        private IDialogService dialogService;


        public GameDetailViewModel(
            IDatabaseAccess database,
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


        private Game game;
        public Game Game
        {
            get => game; 
            set { SetProperty(ref game, value); }
        }

        private ObservableCollection<ChartData> rootChartDataList;
        public ObservableCollection<ChartData> RootChartDataList
        {
            get => rootChartDataList;
            set { SetProperty(ref rootChartDataList, value); }
        }

        private ObservableCollection<ChartData> ToChartDataList(IEnumerable<RootData> rootDataList)
        {
            if (rootDataList == null || rootDataList.Count() == 0)
            {
                var defaultData = new ChartData()
                {
                    Label = "（無題のルート）",
                    Value = Game.TotalPlayTime.TotalSeconds,
                    ToolTip = Game.TotalPlayTime.ToPlayTimeString(),
                    Color = new SolidColorBrush(Colors.DimGray)
                };
                return new ObservableCollection<ChartData>() { defaultData };
            }
            return new ObservableCollection<ChartData>(rootDataList.Select(r => new ChartData()
            {
                Label = r.Name,
                Value = Game.TotalPlayTime.TotalSeconds,
                ToolTip = Game.TotalPlayTime.ToPlayTimeString()
            }));
        }

        private void UpdateRootChartDataList()
        {
            RootChartDataList = ToChartDataList(Game.RootDataList);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var game = navigationContext.Parameters["Game"] as Game;
            if(game != null)
            {
                Game = game;
                Game.PropertyChanged += (_, __) => UpdateRootChartDataList();
                UpdateRootChartDataList();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) {}

        private async void StartGame()
        {
            try
            {
                Process.Start(Game.FileName);
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
                Message = "プレイデータを削除しますか？\nセーブデータやゲーム本体は削除されません。",
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
