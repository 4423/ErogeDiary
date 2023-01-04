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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ErogeDiary.Views.Dialogs
{
    public partial class GameEditDialog : UserControl
    {
        public GameEditDialog()
        {
            InitializeComponent();
            this.Loaded += GameEditDialogLoaded;
        }

        private void GameEditDialogLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            WindowIcon.RemoveIcon(window);
        }
    }
}
