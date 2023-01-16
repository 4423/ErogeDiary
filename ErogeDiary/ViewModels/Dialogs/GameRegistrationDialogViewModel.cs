using ErogeDiary.Dialogs;
using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using Prism.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class GameRegistrationDialogViewModel : BindableDialogBase
    {
        public DelegateCommand FlyoutCompleteCommand { get; private set; }
        public DelegateCommand SelectThumbnailFileNameCommand { get; private set; }
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public Action HideFlyout { get; set; }

        private ErogeDiaryDbContext database;
        private ErogameScapeClient erogameScapeClient;
        private IMessageDialog messageDialog;
        private IOpenFileDialog openFileDialog;


        public GameRegistrationDialogViewModel(
            ErogeDiaryDbContext database,
            ErogameScapeClient erogameScapeClient,
            IMessageDialog messageDialog,
            IOpenFileDialog openFileDialog)
        {
            FlyoutCompleteCommand = new DelegateCommand(FlyoutComplete);
            SelectThumbnailFileNameCommand = new DelegateCommand(SelectThumbnailFileName);
            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            RegisterCommand = new DelegateCommand(RegisterGame, CanExecuteRegisterGame);
            CancelCommand = new DelegateCommand(CloseDialogCancel);
            
            VerifiableGame = new VerifiableGame();
            VerifiableGame.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();

            IsOpen = false;

            this.database = database;
            this.erogameScapeClient = erogameScapeClient;
            this.messageDialog = messageDialog;
            this.openFileDialog = openFileDialog;
        }


        private bool CanExecuteRegisterGame()
            => VerifiableGame.Valid();

        private async void RegisterGame()
        {
            try
            {
                IsRegistering = true;
                // TODO: 消す
                await Task.Delay(2000);
                await RegisterGameCore();
            }
            finally
            {
                IsRegistering = false;
            }
        }

        private async Task RegisterGameCore()
        {
            // TODO: ExecutableFilePath も重複確認
            if (await database.FindGameByTitleAndBrandAsync(VerifiableGame.Title!, VerifiableGame.Brand!) == null)
            {
                var imageUri = new Uri(VerifiableGame.ImageUri!).IsFile ?
                    await ThumbnailHelper.CopyAndResize(VerifiableGame.ImageUri!) :
                    await ThumbnailHelper.DownloadAndResizeAsync(VerifiableGame.ImageUri!);

                VerifiableGame.Pretty();

                var game = new Game()
                {
                    Title = VerifiableGame.Title!,
                    Brand = VerifiableGame.Brand!,
                    ReleaseDate = VerifiableGame.ReleaseDate!.Value,
                    ImageFileName = Path.GetFileName(imageUri),
                    ErogameScapeGameId = VerifiableGame.ErogameScapeGameId,
                    InstallationType = VerifiableGame.InstallationType,
                    WindowTitle = VerifiableGame.WindowTitle,
                    ExecutableFilePath = VerifiableGame.ExecutableFilePath,
                    ClearedAt = VerifiableGame.ClearedAt,
                };
                await database.AddGameAsync(game);
                CloseDialogOK();
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

        private async void FlyoutComplete()
        {
            IsWorking = true;

            try
            {
                var gameInfo = await erogameScapeClient.FetchGameInfoAsync(ErogameScapeUrl);
                VerifiableGame = new VerifiableGame(gameInfo);
                VerifiableGame.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();
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
                VerifiableGame.ImageUri = imageUri;
            }
        }

        private void SelectExecutionFileName()
        {
            var filePath = openFileDialog.Show(
                "ゲームの実行ファイルを選択してください", "実行ファイル(*.exe)|*.exe");
            if (filePath != null)
            {
                VerifiableGame.ExecutableFilePath = filePath;
            }
        }


        private VerifiableGame verifiableGame;
        public VerifiableGame VerifiableGame
        {
            get { return verifiableGame; }
            set { SetProperty(ref verifiableGame, value); }
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

        private bool isRegistering;
        public bool IsRegistering
        {
            get { return isRegistering; }
            set { SetProperty(ref isRegistering, value); }
        }
    }
}
