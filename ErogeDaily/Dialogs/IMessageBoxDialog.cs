using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialogs
{
    public interface IMessageBoxDialog
    {
        public void ShowErrorDialog(string message, string title);
        public bool ShowYesNoDialog(string message, string title);
        public void ShowInfoDialog(string message, string title);
    }
}
