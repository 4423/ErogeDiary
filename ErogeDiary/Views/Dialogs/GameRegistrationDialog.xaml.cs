using ErogeDiary.Models.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ErogeDiary.Views.Dialogs
{
    public partial class GameRegistrationDialog : UserControl
    {
        public GameRegistrationDialog()
        {
            InitializeComponent();
            this.Loaded += GameRegistrationDialogLoaded;
        }

        private void GameRegistrationDialogLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            NativeMethods.RemoveIcon(window);

            ((ViewModels.Dialogs.GameRegistrationDialogViewModel)DataContext).HideFlyout =
                () => InputFlyout.Hide();
        }

        private void FlyoutHide(object sender, RoutedEventArgs e)
            => InputFlyout.Hide();
    }
}
