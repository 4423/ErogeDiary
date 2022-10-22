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
    public class RootEditDialogViewModel : VerifiableBindableBase, IDialogAware
    {
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private IDatabaseAccess database;
        private IMessageDialog messageDialog;
        private Game game;


        public RootEditDialogViewModel(
            IDatabaseAccess database,
            IMessageDialog messageDialog)
        {
            UpdateCommand = new DelegateCommand(UpdateRoot, CanExecuteUpdateRoot);
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
                if (value != null)
                {
                    value.PropertyChanged += SelectedRootPropertyChanged;
                }
                if (selectedRoot != null)
                {
                    selectedRoot.PropertyChanged -= SelectedRootPropertyChanged;
                }
                SetProperty(ref selectedRoot, value);
                PlayTime = selectedRoot?.PlayTime.ToZeroPaddingStringWithoutDays();
            }
        }

        private void SelectedRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateCommand.RaiseCanExecuteChanged();
        }

        private string playTime;
        [TimeSpanFormatRequired]
        public string PlayTime
        {
            get { return playTime; }
            set
            {
                SetProperty(ref playTime, value);
                ValidateProperty(value);
                UpdateCommand.RaiseCanExecuteChanged();
            }
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            Roots = new ObservableCollection<RootData>(game.Roots.Clone());
            SelectedRoot = Roots.FirstOrDefault();
        }

        private bool CanExecuteUpdateRoot()
            => !String.IsNullOrWhiteSpace(SelectedRoot?.Name) 
                && (!SelectedRoot?.HasErrors ?? false) 
                && !HasErrors;

        private async void UpdateRoot()
        {
            var t = PlayTime.ParseWithoutDays();
            var allocableTime = game.GetUnallocatedTime() + SelectedRoot.PlayTime;
            if (t > allocableTime)
            {
                var s = allocableTime.ToZeroPaddingStringWithoutDays();
                var m = $"ルートに割り当てるプレイ時間は {s} よりも大きくできません。";
                await messageDialog.ShowErrorAsync(m);
                return;
            }
            SelectedRoot.PlayTime = t;

            if (!SelectedRoot.IsCleared)
            {
                SelectedRoot.ClearedAt = null;
            }

            SelectedRoot.UpdatedAt = DateTime.Now;

            var root = game.Roots.Single(x => x.Id == SelectedRoot.Id);
            root.CopyFrom(SelectedRoot);
            await database.UpdateRootAsync(root);
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
