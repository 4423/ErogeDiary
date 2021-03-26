using Unity;
using ErogeDaily.Models;
using ErogeDaily.Models.ErogameScape;
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
using System.Windows.Shapes;

namespace ErogeDaily.Views
{
    public partial class GameRegistrationWindow : Window
    {
        public GameRegistrationWindow()
        {
            InitializeComponent();

            // Flyout を binding できないためコードビハインドで実装する
            // System.Windows.Data Error: 4 : Cannot find source for binding with reference ...
            this.Loaded += (_, _) =>
            {
                ((ViewModels.GameRegistrationViewModel)DataContext).HideFlyout = 
                    () => InputFlyout.Hide();
            };
        }

        private void FlyoutHide(object sender, RoutedEventArgs e)
            => InputFlyout.Hide();
    }
}
