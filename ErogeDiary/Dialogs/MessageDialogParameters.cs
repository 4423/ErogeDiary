using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDiary.Dialogs
{
    public class MessageDialogParameters
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public required string CloseButtonText { get; set; }
        public string? PrimaryButtonText { get; set; }
        public string? SecondaryButtonText { get; set; }
    }
}
