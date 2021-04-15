using ErogeDaily.Dialogs;
using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.ViewModels.Dialogs
{
    public class GameEditDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private IDatabaseAccess database;
        private IOpenFileDialog openFileDialog;
        private Game originalGame;


        public GameEditDialogViewModel(IDatabaseAccess database, IOpenFileDialog openFileDialog)
        {
            this.database = database;
            this.openFileDialog = openFileDialog;

            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            CloseCommand = new DelegateCommand(CloseDialog);
            UpdateCommand = new DelegateCommand(UpdateGame);
        }


        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            originalGame = parameters.GetValue<Game>("game");
            Game = originalGame.Clone();
        }


        private Game game;
        public Game Game
        {
            get { return game; }
            set { SetProperty(ref game, value); }
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

        private async void UpdateGame()
        {
            if (!Game.IsValid())
            {
                return;
            }

            originalGame.CopyFrom(Game);
            await database.UpdateAsync(originalGame);
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }


        public event Action<IDialogResult> RequestClose;

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
            => RequestClose?.Invoke(dialogResult);

        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogClosed() { }

        private string title = "";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
