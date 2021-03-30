using ErogeDaily.Models;
using Prism.Commands;
using Prism.Mvvm;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ErogeDaily.Models.ErogameScape;
using ErogeDaily.Dialogs;
using ErogeDaily.Models.Database;
using ModernWpf.Controls;

namespace ErogeDaily.ViewModels
{
    public class GameRegistrationViewModel : BindableBase
    {
        public DelegateCommand FlyoutCompleteCommand { get; private set; }
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand<Window> RegisterCommand { get; private set; }
        public DelegateCommand<Window> CancelCommand { get; private set; }
        public Action HideFlyout { get; set; }

        private IDatabaseAccess database;
        private IErogameScapeAccess erogameScape;
        private IDialogService dialogService;


        public GameRegistrationViewModel(
            IDatabaseAccess database,
            IErogameScapeAccess erogameScape,
            IDialogService dialogService)
        {
            FlyoutCompleteCommand = new DelegateCommand(FlyoutComplete);
            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            RegisterCommand = new DelegateCommand<Window>(RegisterGame);
            CancelCommand = new DelegateCommand<Window>(CloseWindow);

            Game = new Game();
            ValidationMessageVisibility = Visibility.Collapsed;
            ProgressRingVisibility = Visibility.Hidden;
            IsOpen = false;

            this.database = database;
            this.erogameScape = erogameScape;
            this.dialogService = dialogService;
        }

        private void CloseWindow(Window window)
        {
            window?.Close();
        }

        private async void RegisterGame(Window window)
        {
            if (!Game.IsValid())
            {
                return;
            }

            if (await database.FindGameByTitleAndBrandAsync(Game.Title, Game.Brand) == null)
            {
                await database.AddGameAsync(game);
                CloseWindow(window);
            }
            else
            {
                dialogService.MessageDialog.Show(new MessageDialogParameters()
                {
                    Title = "エラー",
                    Message = "既に同じゲームが登録されています。",
                    Icon = MessageDialogImage.Error,
                    Button = MessageDialogButton.OK,
                });
            }
        }

        private async void FlyoutComplete()
        {
            ProgressRingVisibility = Visibility.Visible;

            try
            {
                var gameInfo = await erogameScape.GetGameInfoFromGamePageUrl(ErogameScapeUrl);
                Game = gameInfo.ToGame();
            }
            catch (Exception)
            {
                ValidationMessageVisibility = Visibility.Visible;
                ProgressRingVisibility = Visibility.Hidden;
                return;
            }

            HideFlyout?.Invoke();
            ValidationMessageVisibility = Visibility.Collapsed;
            ProgressRingVisibility = Visibility.Hidden;
        }


        private void SelectExecutionFileName()
        {
            var fileName = dialogService.OpenFileDialog.Show(
                "ゲームの実行ファイルを選択してください", "実行ファイル(*.exe)|*.exe");
            if (fileName != null)
            {
                Game.FileName = fileName;
            }
        }


        private Game game;
        public Game Game
        {
            get { return game; }
            set { SetProperty(ref game, value); }
        }

        private Visibility validationMessageVisibility;
        public Visibility ValidationMessageVisibility
        {
            get { return validationMessageVisibility; }
            set { SetProperty(ref validationMessageVisibility, value); }
        }

        private Visibility progressRingVisibility;
        public Visibility ProgressRingVisibility
        {
            get { return progressRingVisibility; }
            set { SetProperty(ref progressRingVisibility, value); }
        }

        private bool isFlyoutOpen;
        public bool IsOpen
        {
            get { return isFlyoutOpen; }
            set { SetProperty(ref isFlyoutOpen, value); }
        }

        public string erogameScapeUrl;
        public string ErogameScapeUrl
        {
            get { return erogameScapeUrl; }
            set { SetProperty(ref erogameScapeUrl, value); }
        }
    }
}
