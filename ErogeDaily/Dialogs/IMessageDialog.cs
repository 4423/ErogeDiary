﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Dialogs
{
    public interface IMessageDialog
    {
        public Task<MessageDialogResult> ShowAsync(MessageDialogParameters parameters);
    }
}
