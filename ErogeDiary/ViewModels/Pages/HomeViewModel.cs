using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Properties;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDiary.ViewModels.Pages
{
    public partial class HomeViewModel : ObservableObject
    {
        private ErogeDiaryDbContext database;
        private IRegionManager regionManager;
        private IDialogService dialogService;
        private IMessageDialog messageDialog;


        public HomeViewModel(
            ErogeDiaryDbContext database, 
            IRegionManager regionManager,
            IDialogService dialogService,
            IMessageDialog messageDialog)
        {
            this.database = database;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.messageDialog = messageDialog;

            LoadFromDatabase();

            OrderItems = new List<GameOrder>()
            {
                new GameOrder(Strings.Home_Order_ByRecentPlay, nameof(Game.LastPlayedAt), ListSortDirection.Descending),
                new GameOrder(Strings.Home_Order_ByLongPlayTime, nameof(Game.TotalPlayTime), ListSortDirection.Descending),
                new GameOrder(Strings.Home_Order_ByNewReleaseDate, nameof(Game.ReleaseDate), ListSortDirection.Descending),
                new GameOrder(Strings.Home_Order_ByNewRegistration, nameof(Game.RegisteredAt), ListSortDirection.Descending),
                new GameOrder(Strings.Home_Order_ByBrand, nameof(Game.Brand), ListSortDirection.Ascending),
            };
            OrderSelectedItem = OrderItems[0];

            FilterItems = new List<GameFilter>()
            {
                new GameFilter(Strings.Home_Filter_All, x => true),
                new GameFilter(Strings.Home_Filter_Cleared, x => x.IsCleared),
                new GameFilter(Strings.Home_Filter_NotCleared, x => !x.IsCleared),
            };
            FilterSelectedItem = FilterItems[0];
        }


        private async void LoadFromDatabase()
        {
            Games = await database.GetGamesAsync();
        }

        [RelayCommand]
        private async Task StartGameAsync(Game? game)
        {
            if (game == null)
            {
                return;
            }

            try
            {
                Process.Start(game.ExecutableFilePath!);
            }
            catch (Exception ex)
            {
                await messageDialog.ShowAsync(new MessageDialogParameters()
                {
                    Title = Strings.Dialog_ErrorTitle,
                    Message = string.Format(Strings.Game_StartFailedFormat, ex.Message),
                    CloseButtonText = Strings.Common_Ok,
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
        }

        [ObservableProperty]
        private ObservableCollection<Game> games = new();


        [RelayCommand]
        private void GameRegistration()
        {
            dialogService.ShowDialog(nameof(Views.Dialogs.GameRegistrationDialog), null, null);
        }


        [ObservableProperty]
        private List<GameOrder> orderItems = new();

        [ObservableProperty]
        private List<GameFilter> filterItems = new();

        [ObservableProperty]
        private GameOrder orderSelectedItem = null!;

        partial void OnOrderSelectedItemChanged(GameOrder value)
        {
            var descriptions = CollectionViewSource.GetDefaultView(Games).SortDescriptions;
            descriptions.Clear();
            descriptions.Add(value.ToSortDescription());
        }

        [ObservableProperty]
        private GameFilter filterSelectedItem = null!;

        partial void OnFilterSelectedItemChanged(GameFilter value)
        {
            CollectionViewSource.GetDefaultView(Games).Filter = value.Predicate;
        }

        [ObservableProperty]
        private Game? selectedGame;

        partial void OnSelectedGameChanged(Game? value)
        {
            if (value != null)
            {
                var parameters = new NavigationParameters();
                parameters.Add("Game", value);
                NavigationHelper.RequestNavigateToGameDetailPage(regionManager, parameters);
            }
        }
    }
}
