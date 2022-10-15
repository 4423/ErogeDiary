using ErogeDaily.Dialogs;
using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.ViewModels.Dialogs
{
    public class GameEditDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand SelectThumbnailFileNameCommand { get; private set; }
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private IDatabaseAccess database;
        private IMessageDialog messageDialog;
        private IOpenFileDialog openFileDialog;
        private Game originalGame;


        public GameEditDialogViewModel(
            IDatabaseAccess database, 
            IMessageDialog messageDialog, 
            IOpenFileDialog openFileDialog)
        {
            this.database = database;
            this.messageDialog = messageDialog;
            this.openFileDialog = openFileDialog;

            SelectThumbnailFileNameCommand = new DelegateCommand(SelectThumbnailFileName);
            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            CloseCommand = new DelegateCommand(CloseDialog);
            UpdateCommand = new DelegateCommand(UpdateGame, CanExecuteUpdateGame);
        }


        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            originalGame = parameters.GetValue<Game>("game");
            Game = originalGame.Clone();
            Game.PropertyChanged += (_, __) => UpdateCommand.RaiseCanExecuteChanged();
            UpdateCommand.RaiseCanExecuteChanged();
        }

        public virtual void OnDialogClosed()
        {
            Game.ClearAllErrors();
            Game = null;
        }


        private Game game;
        public Game Game
        {
            get { return game; }
            set { SetProperty(ref game, value); }
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


        protected virtual void CloseDialog()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private bool CanExecuteUpdateGame()
        {
            return Game != null && Game.Valid();
        }

        private async void UpdateGame()
        {
            if (Game.ImageUri != originalGame.ImageUri)
            {
                try
                {
                    Game.ImageUri = new Uri(Game.ImageUri).IsFile ?
                        await ThumbnailHelper.CopyAndResize(Game.ImageUri) :
                        await ThumbnailHelper.DownloadAndResizeAsync(Game.ImageUri);
                }
                catch (Exception ex)
                {
                    await messageDialog.ShowErrorAsync($"サムネイル画像の取得に失敗しました。\n{ex.Message}");
                    return;
                }
            }

            Game.Pretty();

            originalGame.CopyFrom(Game);
            await database.UpdateAsync(originalGame);
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }


        public event Action<IDialogResult> RequestClose;

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
            => RequestClose?.Invoke(dialogResult);

        public virtual bool CanCloseDialog() => true;

        private string title = "";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
