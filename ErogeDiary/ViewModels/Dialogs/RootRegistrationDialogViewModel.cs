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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class RootRegistrationDialogViewModel : VerifiableBindableBase, IDialogAware
    {
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private IDatabaseAccess database;
        private IMessageDialog messageDialog;
        private Game game;


        public RootRegistrationDialogViewModel(
            IDatabaseAccess database,
            IMessageDialog messageDialog)
        {
            RegisterCommand = new DelegateCommand(RegisterRootData, CanExecuteRegisterRootData);
            CancelCommand = new DelegateCommand(CloseDialog);

            RootData = new RootData();
            RootData.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();

            this.database = database;
            this.messageDialog = messageDialog;
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

        private string playTime;
        [TimeSpanFormatRequired]
        public string PlayTime
        {
            get { return playTime; }
            set
            {
                SetProperty(ref playTime, value);
                ValidateProperty(value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            PlayTime = game.GetUnallocatedTime().ToZeroPaddingStringWithoutDays();
        }

        private bool CanExecuteRegisterRootData()
            => !String.IsNullOrWhiteSpace(RootData.Name) && !RootData.HasErrors && !HasErrors;

        private async void RegisterRootData()
        {
            if (IsAllocatedAutomatically)
            {
                RootData.PlayTime = game.GetUnallocatedTime();
            }
            else
            {
                var t = PlayTime.ParseWithoutDays();
                var u = game.GetUnallocatedTime();
                if (t > u)
                {
                    var s = u.ToZeroPaddingStringWithoutDays();
                    var m = $"ルートに割り当てるプレイ時間は {s} 以下を指定してください。";
                    await messageDialog.ShowErrorAsync(m);
                    return;
                }
                if (t.TotalSeconds < 1)
                {
                    await messageDialog.ShowErrorAsync(
                        "ルートに割り当てるプレイ時間は1秒以上を指定してください。"
                    );
                    return;
                }
                RootData.PlayTime = t;
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
