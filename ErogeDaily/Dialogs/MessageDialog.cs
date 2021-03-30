using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDaily.Dialogs
{
    public class MessageDialog : IMessageDialog
    {
        public MessageDialogResult Show(MessageDialogParameters parameters)
        {
            var result = MessageBox.Show(parameters.Message, parameters.Title, 
                parameters.Button.ToMessageBoxButton(), parameters.Icon.ToMessageBoxImage());
            return result.ToMessageDialogResult();
        }
    }
}
