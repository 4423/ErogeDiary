using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialogs
{
    public class DialogService : IDialogService
    {
        public IMessageDialog MessageDialog { get; private set; }
        public IOpenFileDialog OpenFileDialog { get; private set; }

        public DialogService()
        {
            MessageDialog = new MessageDialog();
            OpenFileDialog = new OpenFileDialog();
        }
    }
}
