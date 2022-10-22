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
        public string Title { get; set; }
        public string Message { get; set; }
        public string CloseButtonText { get; set; }
        public string PrimaryButtonText { get; set; }
        public string SecondaryButtonText { get; set; }
    }
}
