using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDaily.Dialogs
{
    public class MessageDialogParameters
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public MessageDialogButton Button { get; set; }
        public MessageDialogImage Icon { get; set; }
    }

    public enum MessageDialogButton
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4,
    }

    public enum MessageDialogImage
    {
        None = 0,
        Hand = 0x00000010,
        Question = 0x00000020,
        Exclamation = 0x00000030,
        Asterisk = 0x00000040,
        Stop = Hand,
        Error = Hand,
        Warning = Exclamation,
        Information = Asterisk,
    }

    public static class MessageDialogParametersExtentions
    {
        public static MessageBoxButton ToMessageBoxButton(this MessageDialogButton button)
            => (MessageBoxButton)button;

        public static MessageBoxImage ToMessageBoxImage(this MessageDialogImage image)
            => (MessageBoxImage)image;
    }
}
