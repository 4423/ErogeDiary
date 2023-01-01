using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.DataAnnotations;
using ErogeDiary.Models.Database;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class RootEditDialogViewModel : VerifiableBindableBase, IDialogAware
    {
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
            CancelCommand = new DelegateCommand(CloseDialog);

            this.database = database;
            this.messageDialog = messageDialog;
        }

        private ObservableCollection<RootData>? roots;
        public ObservableCollection<RootData>? Roots
        {
            get { return roots; }
            set { SetProperty(ref roots, value); }
        }

        private RootData? selectedRoot;
        public RootData? SelectedRoot
        {
            get { return selectedRoot; }
            set
            {
                // 元の値から event を解除
                if (selectedRoot != null)
                {
                    selectedRoot.PropertyChanged -= SelectedRootPropertyChanged;
                }

                // 新しい名前に変更したときに ComboBox の表示も変わらないよう clone する
                SetProperty(ref selectedRoot, value?.Clone());

                // clone された値に対して event を登録
                if (selectedRoot != null)
                {
                    selectedRoot.PropertyChanged += SelectedRootPropertyChanged;
                }
                PlayTime = selectedRoot?.PlayTime.ToZeroPaddingStringWithoutDays();
            }
        }

        private void SelectedRootPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => UpdateCommand.RaiseCanExecuteChanged();

        // Converter を使って SelectedRoot.PlayTime の TimeSpan を直接ダイアログ上で編集することも可能だが、
        // 012:34:56 のような文字を入力したときに、012 が Convert → ConvertBack で一度 TimeSpan を経由することで
        // 12 として表示されるという挙動が心地よくないので、あえてここでは単に string で受けている
        private string? playTime;
        [TimeSpanFormatRequired]
        public string? PlayTime
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
        }

        private bool CanExecuteUpdateRoot()
            => SelectedRoot != null && SelectedRoot.Valid() && !HasErrors;

        private async void UpdateRoot()
        {
            var t = PlayTime!.ParseWithoutDays();
            var allocableTime = game!.GetUnallocatedTime() + SelectedRoot!.PlayTime;
            if (t > allocableTime)
            {
                var s = allocableTime.ToZeroPaddingStringWithoutDays();
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
            SelectedRoot.PlayTime = t;

            SelectedRoot.Pretty();

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
