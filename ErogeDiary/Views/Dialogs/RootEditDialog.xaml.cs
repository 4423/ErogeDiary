﻿using ErogeDiary.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ErogeDiary.Views.Dialogs
{
    public partial class RootEditDialog : UserControl
    {
        public RootEditDialog()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                var window = Window.GetWindow(this);
                WindowIcon.RemoveIcon(window);
            };
        }
    }
}
