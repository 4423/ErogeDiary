using ErogeDiary.Dialogs;
using ErogeDiary.Helpers;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using System;

namespace ErogeDiary.ViewModels.Dialogs
{
    public partial class RootRegistrationDialogViewModel : BindableDialogBase
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


        [ObservableProperty]
        private VerifiableRoot? verifiableRoot;

        partial void OnVerifiableRootChanging(VerifiableRoot? value)
        {
            if (verifiableRoot != null)
            {
                verifiableRoot.PropertyChanged -= VerifiableRootChanged;
                verifiableRoot.ErrorsChanged -= VerifiableRootChanged;
            }
        }

        partial void OnVerifiableRootChanged(VerifiableRoot? value)
        {
            if (value != null)
            {
                value.PropertyChanged += VerifiableRootChanged;
                value.ErrorsChanged += VerifiableRootChanged;
            }
            RegisterCommand.NotifyCanExecuteChanged();
        }

        public AccentColors AccentColors { get; } = new AccentColors();

        [ObservableProperty]
        private AccentColor? selectedAccentColor;

        partial void OnSelectedAccentColorChanged(AccentColor? value)
        {
            if (value != null && VerifiableRoot != null)
            {
                VerifiableRoot.AccentColor = value;
            }
        }

        [ObservableProperty]
        private bool isAllocatedAutomatically = true;

        partial void OnIsAllocatedAutomaticallyChanged(bool value)
        {
            if (value && VerifiableRoot != null)
            {
                VerifiableRoot.PlayTime = game?.GetUnallocatedTime().ToZeroPaddingStringWithoutDays();
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
