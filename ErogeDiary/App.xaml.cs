using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.ErogameScape;
using ErogeDiary.ViewModels;
using ErogeDiary.ViewModels.Dialogs;
using ErogeDiary.ViewModels.Pages;
using ErogeDiary.Views;
using ErogeDiary.Views.Dialogs;
using ErogeDiary.Views.Pages;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace ErogeDiary
{
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var dbContext = new ErogeDiaryDbContext())
            {
                dbContext.Database.Migrate();
            }
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<HomePage, HomeViewModel>();
            ViewModelLocationProvider.Register<GameDetailPage, GameDetailViewModel>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ErogeDiaryDbContext>();
            containerRegistry.Register<GameMonitor>();
            containerRegistry.Register<IErogameScapeAccess, WebScrapingErogameScapeAccess>();

            containerRegistry.Register<IMessageDialog, MessageDialog>();
            containerRegistry.Register<IOpenFileDialog, OpenFileDialog>();

            containerRegistry.RegisterDialog<GameEditDialog, GameEditDialogViewModel>();
            containerRegistry.RegisterDialog<GameRegistrationDialog, GameRegistrationDialogViewModel>();
            containerRegistry.RegisterDialog<RootRegistrationDialog, RootRegistrationDialogViewModel>();
            containerRegistry.RegisterDialog<RootEditDialog, RootEditDialogViewModel>();
            containerRegistry.RegisterDialog<RootRemoveDialog, RootRemoveDialogViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule(typeof(NavigationModule));
        }
    }
}
