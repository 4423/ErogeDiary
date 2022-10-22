using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.ErogameScape;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class RootRemoveDialogViewModel : VerifiableBindableBase, IDialogAware
    {
        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private IDatabaseAccess database;
        private IMessageDialog messageDialog;
        private Game game;


        public RootRemoveDialogViewModel(
            IDatabaseAccess database,
            IMessageDialog messageDialog)
        {
            RemoveCommand = new DelegateCommand(RemoveRoot, CanExecuteRemoveRoot);
            CancelCommand = new DelegateCommand(CloseDialog);

            this.database = database;
            this.messageDialog = messageDialog;
        }


        private ObservableCollection<RootData> roots;
        public ObservableCollection<RootData> Roots
        {
            get { return roots; }
            set { SetProperty(ref roots, value); }
        }

        private RootData selectedRoot;
        public RootData SelectedRoot
        {
            get { return selectedRoot; }
            set 
            { 
                SetProperty(ref selectedRoot, value);
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }


        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            Roots = new ObservableCollection<RootData>(game.Roots);
            SelectedRoot = Roots.FirstOrDefault();
        }

        private bool CanExecuteRemoveRoot()
            => SelectedRoot != null;

        private async void RemoveRoot()
        {
            try
            {
                await database.RemoveRootAsync(SelectedRoot);
                await database.UpdateAsync(game);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception)
            {
                await messageDialog.ShowErrorAsync("ルートの削除に失敗しました。");
            }
        }

        private void CloseDialog()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }


        public event Action<IDialogResult> RequestClose;

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
            => RequestClose?.Invoke(dialogResult);

        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogClosed() { }

        public string Title => "";
    }
}
