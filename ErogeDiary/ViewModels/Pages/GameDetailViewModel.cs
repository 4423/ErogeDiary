using ErogeDiary.Controls;
using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
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

namespace ErogeDiary.ViewModels.Pages
{
    public class GameDetailViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand StartGameCommand { get; private set; }
        public DelegateCommand EditGameCommand { get; private set; }
        public DelegateCommand DeleteGameCommand { get; private set; }
        public DelegateCommand AddRootCommand { get; private set; }
        public DelegateCommand EditRootCommand { get; private set; }
        public DelegateCommand RemoveRootCommand { get; private set; }

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
            AddRootCommand = new DelegateCommand(AddRoot);
            EditRootCommand = new DelegateCommand(EditRoot);
            RemoveRootCommand = new DelegateCommand(RemoveRoot);
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

        private ObservableCollection<ChartData> ToChartDataList(IEnumerable<RootData> roots)
        {
            var unallocatedTime = Game.GetUnallocatedTime();
            var unallocatedData = new ChartData()
            {
                Label = "（未割り当てのルート）",
                Value = unallocatedTime.TotalSeconds,
                ToolTip = unallocatedTime.ToPlayTimeString(),
                Color = new SolidColorBrush(Colors.DimGray)
            };

            if (roots == null || roots.Count() == 0)
            {
                return new ObservableCollection<ChartData>() { unallocatedData };
            }

            var charts = new ObservableCollection<ChartData>(roots.Select(r => new ChartData()
            {
                Label = r.Name,
                Value = r.PlayTime.TotalSeconds,
                ToolTip = r.PlayTime.ToPlayTimeString()
            }));
            charts.Add(unallocatedData);
            return charts;
        }

        private void UpdateRootChartDataList()
        {
            RootChartDataList = ToChartDataList(Game.Roots);
        }

        private Timeline timeline;
        public Timeline Timeline
        {
            get => timeline;
            set { SetProperty(ref timeline, value); }
        }

        private void UpdateTimeline()
        {
            Timeline = new Timeline()
            {
                PlayLogs = new ObservableCollection<PlayLog>(database.FindPlayLogsByGameId(Game.Id))
            };
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var game = navigationContext.Parameters["Game"] as Game;
            if(game != null)
            {
                Game = game;
                Game.PropertyChanged += (s, e) =>
                {
                    UpdateRootChartDataList();
                    if (e.PropertyName == nameof(Game.TotalPlayTime))
                    {
                        UpdateTimeline();
                    }
                };
                UpdateRootChartDataList();
                UpdateTimeline();
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

        private void AddRoot()
        {
            var dialogParams = new DialogParameters()
            {
                { "game", Game }
            };
            dialogService.ShowDialog(nameof(Views.Dialogs.RootRegistrationDialog), dialogParams, null);
            UpdateRootChartDataList();
        }

        private void EditRoot()
        {
            var dialogParams = new DialogParameters()
            {
                { "game", Game }
            };
            dialogService.ShowDialog(nameof(Views.Dialogs.RootEditDialog), dialogParams, null);
            UpdateRootChartDataList();
        }


        private void RemoveRoot()
        {
            var dialogParams = new DialogParameters()
            {
                { "game", Game }
            };
            dialogService.ShowDialog(nameof(Views.Dialogs.RootRemoveDialog), dialogParams, null);
            UpdateRootChartDataList();
        }
    }
}
