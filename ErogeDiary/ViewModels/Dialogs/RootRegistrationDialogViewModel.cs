using ErogeDiary.Dialogs;
using ErogeDiary.Helpers;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using System;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class RootRegistrationDialogViewModel : BindableDialogBase
    {
        public RelayCommand RegisterCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        private ErogeDiaryDbContext database;
        private IMessageDialog messageDialog;
        private Game? game;


        public RootRegistrationDialogViewModel(
            ErogeDiaryDbContext database,
            IMessageDialog messageDialog)
        {
            RegisterCommand = new RelayCommand(RegisterRootData, CanExecuteRegisterRootData);
            CancelCommand = new RelayCommand(CloseDialogCancel);

            this.database = database;
            this.messageDialog = messageDialog;
        }


        private VerifiableRoot? verifiableRoot;
        public VerifiableRoot? VerifiableRoot
        {
            get { return verifiableRoot; }
            set { SetProperty(ref verifiableRoot, value); }
        }

        public AccentColors AccentColors { get; } = new AccentColors();

        private AccentColor? selectedAccentColor;
        public AccentColor? SelectedAccentColor
        {
            get => selectedAccentColor;
            set
            {
                if (SetProperty(ref selectedAccentColor, value) && value != null && VerifiableRoot != null)
                {
                    VerifiableRoot.AccentColor = value;
                }
            }
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

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");

            VerifiableRoot = new VerifiableRoot()
            {
                PlayTime = game.GetUnallocatedTime().ToZeroPaddingStringWithoutDays(),
                AccentColor = AccentColors.Random(),
            };
            SelectedAccentColor = VerifiableRoot.AccentColor;
            VerifiableRoot.PropertyChanged += VerifiableRootChanged;
            VerifiableRoot.ErrorsChanged += VerifiableRootChanged;
        }

        private void VerifiableRootChanged(object? sender, EventArgs e)
            => RegisterCommand.NotifyCanExecuteChanged();

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
                Color = VerifiableRoot.AccentColor.Color,
                GameId = game.GameId,
            };
            await database.AddRootAsync(root);
            // TODO: 追加時に同期する
            // game.Roots.Add(root);
            // await database.UpdateAsync(game);

            CloseDialogOK();
        }

    }
}
