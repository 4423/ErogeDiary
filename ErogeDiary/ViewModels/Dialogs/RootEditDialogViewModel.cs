using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ErogeDiary.ViewModels.Dialogs
{
    public partial class RootEditDialogViewModel : BindableDialogBase
    {
        private ErogeDiaryDbContext database;
        private IMessageDialog messageDialog;
        private Game? game;


        public RootEditDialogViewModel(
            ErogeDiaryDbContext database,
            IMessageDialog messageDialog)
        {
            this.database = database;
            this.messageDialog = messageDialog;
        }

        [ObservableProperty]
        private ICollection<Root>? roots;

        [ObservableProperty]
        private Root? selectedRoot;

        partial void OnSelectedRootChanged(Root? value)
        {
            if (value != null)
            {
                SelectedVerifiableRoot = new VerifiableRoot()
                {
                    Name = value.Name,
                    PlayTime = value.PlayTime.ToZeroPaddingStringWithoutDays(),
                    IsCleared = value.IsCleared,
                    AccentColor = new AccentColor(value.Color),
                    ClearedAt = value.ClearedAt,
                };
                SelectedAccentColor = SelectedVerifiableRoot.AccentColor;
            }
            else
            {
                SelectedVerifiableRoot = null;
                SelectedAccentColor = null;
            }
        }

        [ObservableProperty]
        private VerifiableRoot? selectedVerifiableRoot;

        partial void OnSelectedVerifiableRootChanging(VerifiableRoot? value)
        {
            if (selectedVerifiableRoot != null)
            {
                selectedVerifiableRoot.PropertyChanged -= VerifiableRootPropertyChanged;
                selectedVerifiableRoot.ErrorsChanged -= VerifiableRootPropertyChanged;
            }
        }

        partial void OnSelectedVerifiableRootChanged(VerifiableRoot? value)
        {
            if (value != null)
            {
                value.PropertyChanged += VerifiableRootPropertyChanged;
                value.ErrorsChanged += VerifiableRootPropertyChanged;
            }

            UpdateCommand.NotifyCanExecuteChanged();
        }

        public AccentColors AccentColors { get; } = new AccentColors();

        [ObservableProperty]
        private AccentColor? selectedAccentColor;

        partial void OnSelectedAccentColorChanged(AccentColor? value)
        {
            if (value != null && SelectedVerifiableRoot != null)
            {
                SelectedVerifiableRoot.AccentColor = value;
            }
        }

        private void VerifiableRootPropertyChanged(object? sender, EventArgs e)
            => UpdateCommand.NotifyCanExecuteChanged();

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            Roots = game.Roots;
        }

        private bool CanExecuteUpdateRoot()
            => SelectedVerifiableRoot?.Valid() == true && game != null;

        [RelayCommand(CanExecute = nameof(CanExecuteUpdateRoot))]
        private async Task UpdateAsync()
        {
            var playTime = SelectedVerifiableRoot!.PlayTime!.ParseWithoutDays();
            var allocableTime = game!.GetUnallocatedTime() + SelectedRoot!.PlayTime;
            if (playTime > allocableTime)
            {
                var s = allocableTime.ToZeroPaddingStringWithoutDays();
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

            SelectedVerifiableRoot.Pretty();

            var root = game.Roots.Single(r => r.RootId == SelectedRoot.RootId);
            root.Name = SelectedVerifiableRoot.Name!;
            root.PlayTime = playTime;
            root.ClearedAt = SelectedVerifiableRoot.ClearedAt;
            root.Color = SelectedVerifiableRoot.AccentColor.Color;
            await database.UpdateRootAsync(root);

            CloseDialogOK();
        }

    }
}
