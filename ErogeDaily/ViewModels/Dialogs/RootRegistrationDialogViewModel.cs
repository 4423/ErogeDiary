using ErogeDaily.Dialogs;
using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using ErogeDaily.Models.ErogameScape;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDaily.ViewModels.Dialogs
{
    public class RootRegistrationDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private IDatabaseAccess database;
        private Game game;


        public RootRegistrationDialogViewModel(IDatabaseAccess database)
        {
            RegisterCommand = new DelegateCommand(RegisterRootData, CanExecuteRegisterRootData);
            CancelCommand = new DelegateCommand(CloseDialog);

            RootData = new RootData();
            RootData.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();

            this.database = database;
        }


        private RootData rootData;
        public RootData RootData
        {
            get { return rootData; }
            set { SetProperty(ref rootData, value); }
        }

        private bool isAllocatedAutomatically = true;
        public bool IsAllocatedAutomatically
        {
            get { return isAllocatedAutomatically; }
            set { SetProperty(ref isAllocatedAutomatically, value); }
        }


        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            RootData.PlayTime = game.GetUnallocatedTime();
        }

        private bool CanExecuteRegisterRootData()
            => !String.IsNullOrWhiteSpace(RootData.Name) && !RootData.HasErrors;

        private async void RegisterRootData()
        {
            if (IsAllocatedAutomatically)
            {
                RootData.PlayTime = game.GetUnallocatedTime();
            }

            if (!RootData.IsCleared)
            {
                RootData.ClearedAt = null;
            }
            var now = DateTime.Now;
            RootData.UpdatedAt = now;
            RootData.CreatedAt = now;

            await database.AddRootAsync(rootData);

            if (game.Roots == null)
            {
                game.Roots = new List<RootData>();
            }
            game.Roots.Add(RootData);
            await database.UpdateAsync(game);

            RaiseRequestClose(new DialogResult(ButtonResult.OK));
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
