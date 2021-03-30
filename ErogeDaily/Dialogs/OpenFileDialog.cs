using Microsoft.Win32;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialogs
{
    public class OpenFileDialog : IOpenFileDialog
    {
        public string Show(string title, string filter)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog()
            {
                Title = title,
                Filter = filter,
                Multiselect = false,
                RestoreDirectory = true
            };

            return (bool)ofd.ShowDialog() ? ofd.FileName : null;
        }
    }
}
