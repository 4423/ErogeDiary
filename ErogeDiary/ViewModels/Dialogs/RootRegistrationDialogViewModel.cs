using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
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
    public class RootRegistrationDialogViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private ErogeDiaryDbContext database;
        private IMessageDialog messageDialog;
        private Game? game;


        public RootRegistrationDialogViewModel(
            ErogeDiaryDbContext database,
            IMessageDialog messageDialog)
        {
            RegisterCommand = new DelegateCommand(RegisterRootData, CanExecuteRegisterRootData);
            CancelCommand = new DelegateCommand(CloseDialog);

            this.database = database;
            this.messageDialog = messageDialog;
        }


        private VerifiableRoot? verifiableRoot;
        public VerifiableRoot? VerifiableRoot
        {
            get { return verifiableRoot; }
            set { SetProperty(ref verifiableRoot, value); }
        }

        private bool isAllocatedAutomatically = true;
        public bool IsAllocatedAutomatically
        {
            get { return isAllocatedAutomatically; }
            set
            {
                SetProperty(ref isAllocatedAutomatically, value);
                if (isAllocatedAutomatically && VerifiableRoot != null)
                {
                    VerifiableRoot.PlayTime = game?.GetUnallocatedTime().ToZeroPaddingStringWithoutDays();
                }
            }
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");

            VerifiableRoot = new VerifiableRoot()
            {
                PlayTime = game.GetUnallocatedTime().ToZeroPaddingStringWithoutDays()
            };
            VerifiableRoot.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteRegisterRootData()
            => VerifiableRoot?.Valid() == true && game != null;

        private async void RegisterRootData()
        {
            TimeSpan playTime;
            if (IsAllocatedAutomatically)
            {
                playTime = game!.GetUnallocatedTime();
            }
            else
            {
                playTime = VerifiableRoot!.PlayTime!.ParseWithoutDays();
                var u = game!.GetUnallocatedTime();
                if (playTime > u)
                {
                    var s = u.ToZeroPaddingStringWithoutDays();
                    var m = $"ルートに割り当てるプレイ時間は {s} 以下を指定してください。";
                    await messageDialog.ShowErrorAsync(m);
                    return;
                }
                if (playTime.TotalSeconds < 1)
                {
                    await messageDialog.ShowErrorAsync(
                        "ルートに割り当てるプレイ時間は1秒以上を指定してください。"
                    );
                    return;
                }
            }

            VerifiableRoot!.Pretty();

            var root = new Root()
            {
                Name = VerifiableRoot.Name!,
                PlayTime = playTime,
                ClearedAt = VerifiableRoot.ClearedAt,
                GameId = game.GameId,
            };
            await database.AddRootAsync(root);
            // TODO: 追加時に同期する
            // game.Roots.Add(root);
            // await database.UpdateAsync(game);

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
