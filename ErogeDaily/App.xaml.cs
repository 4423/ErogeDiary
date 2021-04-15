using ErogeDaily.Dialogs;
using ErogeDaily.Models;
using ErogeDaily.Models.Database;
using ErogeDaily.Models.ErogameScape;
using ErogeDaily.ViewModels;
using ErogeDaily.ViewModels.Dialogs;
using ErogeDaily.ViewModels.Pages;
using ErogeDaily.Views;
using ErogeDaily.Views.Dialogs;
using ErogeDaily.Views.Pages;
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

namespace ErogeDaily
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<GameRegistrationWindow, GameRegistrationViewModel>();
            ViewModelLocationProvider.Register<HomePage, HomeViewModel>();
            ViewModelLocationProvider.Register<GameDetailPage, GameDetailViewModel>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var sqldb = new SQLiteDatabaseAccess();
            sqldb.Database.EnsureCreated();
            containerRegistry.RegisterInstance<IDatabaseAccess>(sqldb);

            var gameMonitor = new GameMonitor(sqldb);
            containerRegistry.RegisterInstance<GameMonitor>(gameMonitor);

            containerRegistry.Register<IErogameScapeAccess, WebScrapingErogameScapeAccess>();
            containerRegistry.Register<IMessageDialog, MessageDialog>();
            containerRegistry.Register<IOpenFileDialog, OpenFileDialog>();

            containerRegistry.RegisterDialog<GameEditDialog, GameEditDialogViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule(typeof(NavigationModule));
        }
    }
}
