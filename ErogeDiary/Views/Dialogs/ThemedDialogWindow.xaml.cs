using Prism.Services.Dialogs;
using System.ComponentModel;
using System.Windows;

namespace ErogeDiary.Views.Dialogs
{
    public partial class ThemedDialogWindow : Window, IDialogWindow
    {
        public IDialogResult Result { get; set; } = null!;

        public ThemedDialogWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Result ??= new DialogResult(ButtonResult.Cancel);
            base.OnClosing(e);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Result ??= new DialogResult(ButtonResult.Cancel);
            Close();
        }
    }
}