using System;
using ErogeDiary.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary.Dialogs
{
    public static class DialogExtensions
    {
        public static async Task<MessageDialogResult> ShowErrorAsync(this IMessageDialog dialog, string errorMessage)
        {
            return await dialog.ShowAsync(new MessageDialogParameters()
            {
                Title = Strings.Dialog_ErrorTitle,
                Message = errorMessage,
                CloseButtonText = Strings.Common_Ok,
            });
        }
    }
}
