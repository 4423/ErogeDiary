using ModernWpf.Controls;
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
        public Task<MessageDialogResult> ShowAsync(MessageDialogParameters parameters)
        {
            var d = new ContentDialog()
            {
                Title = parameters.Title,
                Content = parameters.Message,
                CloseButtonText = parameters.CloseButtonText,
            };
            if (!String.IsNullOrEmpty(parameters.PrimaryButtonText))
            {
                d.PrimaryButtonText = parameters.PrimaryButtonText;
            }
            if (!String.IsNullOrEmpty(parameters.SecondaryButtonText))
            {
                d.SecondaryButtonText = parameters.SecondaryButtonText;
            }

            return d.ShowAsync().ContinueWith(t => t.Result.ToMessageDialogResult());
        }
    }
}
