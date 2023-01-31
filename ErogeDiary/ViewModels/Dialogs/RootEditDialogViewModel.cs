using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class RootEditDialogViewModel : BindableDialogBase
    {
        public DelegateCommand<SelectionChangedEventArgs> SelectColorCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private ErogeDiaryDbContext database;
        private IMessageDialog messageDialog;
        private Game? game;


        public RootEditDialogViewModel(
            ErogeDiaryDbContext database,
            IMessageDialog messageDialog)
        {
            UpdateCommand = new DelegateCommand(UpdateRoot, CanExecuteUpdateRoot);
            CancelCommand = new DelegateCommand(CloseDialogCancel);
            SelectColorCommand = new DelegateCommand<SelectionChangedEventArgs>(SelectColor);

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
                if (selectedRoot != null)
                {
                    SelectedVerifiableRoot = new VerifiableRoot()
                    {
                        Name = selectedRoot.Name,
                        PlayTime = selectedRoot.PlayTime.ToZeroPaddingStringWithoutDays(),
                        IsCleared= selectedRoot.IsCleared,
                        AccentColor = new AccentColor(selectedRoot.Color),
                        ClearedAt= selectedRoot.ClearedAt,
                    };
                }
            }
        }

        private VerifiableRoot? selectedVerifiableRoot;
        public VerifiableRoot? SelectedVerifiableRoot
        {
            get { return selectedVerifiableRoot; }
            set
            {
                // 元の値から event を解除
                if (selectedVerifiableRoot != null)
                {
                    selectedVerifiableRoot.PropertyChanged -= VerifiableRootPropertyChanged;
                }
                // 新しい値に event を登録
                if (value != null)
                {
                    value.PropertyChanged += VerifiableRootPropertyChanged;
                }

                SetProperty(ref selectedVerifiableRoot, value);

                UpdateCommand.RaiseCanExecuteChanged();
            }
        }

        public AccentColors AccentColors { get; } = new AccentColors();

        private void VerifiableRootPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => UpdateCommand.RaiseCanExecuteChanged();

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            game = parameters.GetValue<Game>("game");
            Roots = game.Roots;
        }

        private bool CanExecuteUpdateRoot()
            => SelectedVerifiableRoot?.Valid() == true && game != null;

        private async void UpdateRoot()
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

        private void SelectColor(SelectionChangedEventArgs e)
        {
            var accentColor = e.AddedItems[0] as AccentColor;
            if (accentColor != null && SelectedVerifiableRoot != null)
            {
                SelectedVerifiableRoot.AccentColor = accentColor;
            }
        }
    }
}
