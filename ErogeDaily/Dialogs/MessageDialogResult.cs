using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDaily.Dialogs
{
    public enum MessageDialogResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
    }

    public static class MessageBoxResultExtentions
    {
        public static MessageDialogResult ToMessageDialogResult(this MessageBoxResult result)
            => (MessageDialogResult)result;
    }
}
