using ErogeDaily.Dialogs;
using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using ErogeDaily.Models.ErogameScape;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDaily.ViewModels.Dialogs
{
    public class GameRegistrationDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand FlyoutCompleteCommand { get; private set; }
        public DelegateCommand SelectThumbnailFileNameCommand { get; private set; }
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public Action HideFlyout { get; set; }

        private IDatabaseAccess database;
        private IErogameScapeAccess erogameScape;
        private IMessageDialog messageDialog;
        private IOpenFileDialog openFileDialog;


        public GameRegistrationDialogViewModel(
            IDatabaseAccess database,
            IErogameScapeAccess erogameScape,
            IMessageDialog messageDialog,
            IOpenFileDialog openFileDialog)
        {
            FlyoutCompleteCommand = new DelegateCommand(FlyoutComplete);
            SelectThumbnailFileNameCommand = new DelegateCommand(SelectThumbnailFileName);
            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            RegisterCommand = new DelegateCommand(RegisterGame);
            CancelCommand = new DelegateCommand(CloseDialog);

            Game = new Game();
            IsOpen = false;

            this.database = database;
            this.erogameScape = erogameScape;
            this.messageDialog = messageDialog;
            this.openFileDialog = openFileDialog;
        }


        private async void RegisterGame()
        {
            if (!Game.IsValid())
            {
                return;
            }

            if (await database.FindGameByTitleAndBrandAsync(Game.Title, Game.Brand) == null)
            {
                if (new Uri(Game.ImageUri).IsFile)
                {
                    Game.ImageUri = await ThumbnailDownloadHelper.CopyToThumbnailDirectoryAsync(Game.ImageUri);
                }
                else
                {
                    Game.ImageUri = await ThumbnailDownloadHelper.DownloadAsync(Game.ImageUri);
                }

                await database.AddGameAsync(Game);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            else
            {
                await messageDialog.ShowAsync(new MessageDialogParameters()
                {
                    Title = "エラー",
                    Message = "既に同じゲームが登録されています。",
                    CloseButtonText = "OK",
                });
            }
        }

        private void CloseDialog()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private async void FlyoutComplete()
        {
            IsWorking = true;

            try
            {
                var gameInfo = await erogameScape.GetGameInfoFromGamePageUrl(ErogameScapeUrl);
                Game = gameInfo.ToGame();
            }
            catch (Exception)
            {
                IsInvalidErogameScapeUrl = true;
                IsWorking = false;
                return;
            }

            HideFlyout?.Invoke();
            IsInvalidErogameScapeUrl = false;
            IsWorking = false;
        }

        private void SelectThumbnailFileName()
        {
            var imageUri = openFileDialog.Show(
                "サムネイル画像を選択してください", 
                "画像ファイル(*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp");
            if (imageUri != null)
            {
                Game.ImageUri = imageUri;
            }
        }

        private void SelectExecutionFileName()
        {
            var fileName = openFileDialog.Show(
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

        private bool isInvalidErogameScapeUrl;
        public bool IsInvalidErogameScapeUrl
        {
            get { return isInvalidErogameScapeUrl; }
            set { SetProperty(ref isInvalidErogameScapeUrl, value); }
        }

        private bool isWorking;
        public bool IsWorking
        {
            get { return isWorking; }
            set { SetProperty(ref isWorking, value); }
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



        public event Action<IDialogResult> RequestClose;

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
            => RequestClose?.Invoke(dialogResult);

        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogOpened(IDialogParameters parameters) { }
        
        public virtual void OnDialogClosed() { }

        public string Title => "";
    }
}
