using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDiary.Dialogs
{
    public enum MessageDialogResult
    {
        None,
        Primary,
        Secondary,
    }

    public static class MessageBoxResultExtentions
    {
        public static MessageDialogResult ToMessageDialogResult(this ContentDialogResult result)
            => (MessageDialogResult)result;
    }
}
