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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDaily.ViewModels.Pages
{
    public class HomeViewModel : BindableBase
    {
        public DelegateCommand<Game> StartGameCommand { get; private set; }
        public DelegateCommand GameRegistrationCommand { get; private set; }

        private IDatabaseAccess database;
        private IRegionManager regionManager;
        private IDialogService dialogService;
        private IMessageDialog messageDialog;


        public HomeViewModel(
            IDatabaseAccess database, 
            IRegionManager regionManager,
            IDialogService dialogService,
            IMessageDialog messageDialog)
        {
            StartGameCommand = new DelegateCommand<Game>(StartGame);
            GameRegistrationCommand = new DelegateCommand(RegisterGame);

            this.database = database;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.messageDialog = messageDialog;

            LoadFromDatabase();

            OrderItems = new List<GameOrder>()
            {
                new GameOrder("最近プレイした順", "LatestDate", ListSortDirection.Descending),
                new GameOrder("プレイ時間が長い順", "TotalPlayTime", ListSortDirection.Descending),
                new GameOrder("発売日が新しい順", "ReleaseDate", ListSortDirection.Descending),
                new GameOrder("ブランド順", "Brand", ListSortDirection.Ascending),
            };
            OrderSelectedItem = OrderItems[0];

            FilterItems = new List<GameFilter>()
            {
                new GameFilter("すべて", x => true),
                new GameFilter("攻略済み", x => x.IsCleared),
                new GameFilter("未攻略", x => !x.IsCleared),
            };
            FilterSelectedItem = FilterItems[0];
        }


        private async void LoadFromDatabase()
        {
            Games = await database.GetGamesAsync();
        }

        private async void StartGame(Game game)
        {
            try
            {
                Process.Start(game.FileName);
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

        public class GameOrder
        {
            public string DisplayName { get; set; }
            public string PropertyName { get; set; }
            public ListSortDirection SortDirection { get; set; }

            public GameOrder(string displayName, string propertyName, ListSortDirection direction)
            {
                DisplayName = displayName;
                PropertyName = propertyName;
                SortDirection = direction;
            }
            public GameOrder() { }

            public SortDescription ToSortDescription()
                => new SortDescription(PropertyName, SortDirection);
        }

        public class GameFilter
        {
            public string DisplayName { get; set; }
            public Predicate<Object> Predicate { get; set; }

            public GameFilter(string displayName, Predicate<Game> predicate)
            {
                DisplayName = displayName;
                Predicate = x => predicate.Invoke((Game)x);
            }
            public GameFilter() { }
        }

        private ObservableCollection<Game> games;
        public ObservableCollection<Game> Games
        {
            get { return games; }
            set { SetProperty(ref games, value); }
        }


        private void RegisterGame()
        {
            dialogService.ShowDialog(nameof(Views.Dialogs.GameRegistrationDialog), null, null);
        }


        private List<GameOrder> orderItems;
        public List<GameOrder> OrderItems
        {
            get { return orderItems; }
            set { SetProperty(ref orderItems, value); }
        }

        private List<GameFilter> filterItems;
        public List<GameFilter> FilterItems
        {
            get { return filterItems; }
            set
            {
                SetProperty(ref filterItems, value);
            }
        }

        private GameOrder orderSelectedItem;
        public GameOrder OrderSelectedItem
        {
            get { return orderSelectedItem; }
            set
            {
                SetProperty(ref orderSelectedItem, value);
                var descriptions = CollectionViewSource.GetDefaultView(Games).SortDescriptions;
                descriptions.Clear();
                descriptions.Add(value.ToSortDescription());
            }
        }

        private GameFilter filterSelectedItem;
        public GameFilter FilterSelectedItem
        {
            get { return filterSelectedItem; }
            set
            {
                SetProperty(ref filterSelectedItem, value);
                CollectionViewSource.GetDefaultView(Games).Filter = value.Predicate;
            }
        }

        private Game selectedGame;
        public Game SelectedGame
        {
            get { return selectedGame; }
            set
            {
                SetProperty(ref selectedGame, value);
                if (value != null)
                {
                    var parameters = new NavigationParameters();
                    parameters.Add("Game", value);
                    NavigationHelper.RequestNavigateToGameDetailPage(regionManager, parameters);
                }
            }
        }
    }
}
