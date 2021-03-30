using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialogs
{
    public interface IOpenFileDialog
    {
        string Show(string title, string filter);
    }
}
