using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialogs
{
    public static class DialogExtensions
    {
        public static async Task<MessageDialogResult> ShowErrorAsync(this IMessageDialog dialog, string errorMessage)
        {
            return await dialog.ShowAsync(new MessageDialogParameters()
            {
                Title = "エラー",
                Message = errorMessage,
                CloseButtonText = "OK",
            });
        }
    }
}
