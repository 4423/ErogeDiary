﻿using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
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
    public class RootRemoveDialogViewModel : BindableBase, IDialogAware
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
            CancelCommand = new DelegateCommand(CloseDialog);

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


        public virtual void OnDialogOpened(IDialogParameters parameters)
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
