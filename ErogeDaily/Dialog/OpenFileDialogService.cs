using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialog
{
    public class OpenFileDialogService : IOpenFileDialogService
    {
        public string Show(string title, string filter)
        {
            var ofd = new OpenFileDialog()
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
