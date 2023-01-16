using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class RootRemoveDialogViewModel : BindableDialogBase
    {
        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private ErogeDiaryDbContext database;
        private IMessageDialog messageDialog;
        private Game? game;


        public RootRemoveDialogViewModel(
            ErogeDiaryDbContext database,
            IMessageDialog messageDialog)
        {
            RemoveCommand = new DelegateCommand(RemoveRoot, CanExecuteRemoveRoot);
            CancelCommand = new DelegateCommand(CloseDialogCancel);

            this.database = database;
            this.messageDialog = messageDialog;
        }


        private ICollection<Root>? roots;
        public ICollection<Root>? Roots
        {
            get { return roots; }
            set { SetProperty(ref roots, value); }
        }

        private Root? selectedRoot;
        public Root? SelectedRoot
        {
            get { return selectedRoot; }
            set 
            { 
                SetProperty(ref selectedRoot, value);
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }


        public override void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            Roots = game.Roots;
            SelectedRoot = game.Roots.FirstOrDefault();
        }

        private bool CanExecuteRemoveRoot()
            => SelectedRoot != null;

        private async void RemoveRoot()
        {
            try
            {
                await database.RemoveRootAsync(SelectedRoot!);
                CloseDialogOK();
            }
            catch (Exception)
            {
                await messageDialog.ShowErrorAsync("ルートの削除に失敗しました。");
            }
        }
    }
}
