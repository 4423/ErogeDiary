using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialog
{
    public interface IMessageBoxDialog
    {
        public void ShowErrorDialog(string message, string title);
    }
}
