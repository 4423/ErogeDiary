using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialog
{
    public interface IOpenFileDialogService
    {
        string Show(string title, string filter);
    }
}
