using ErogeDiary.Dialogs;
using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
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
using System.IO;
using System.Threading;
using System.Windows;
using Unity;

namespace ErogeDiary;

public partial class App : PrismApplication
{
    // 二重起動させない
    private static Mutex singleInstanceMutex = new Mutex(false, "ErogeDiary");
    private static bool isNewInstance = false;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

        isNewInstance = singleInstanceMutex.WaitOne(0, false);
        if (!isNewInstance)
        {
            // TODO: MessageBox を出したいが表示されない
            singleInstanceMutex.Close();
            Shutdown();
            return;
        }

        using (var dbContext = new ErogeDiaryDbContext())
        {
            dbContext.Database.Migrate();
        }

        base.OnStartup(e);
    }

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
        ViewModelLocationProvider.Register<HomePage, HomeViewModel>();
        ViewModelLocationProvider.Register<GameDetailPage, GameDetailViewModel>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 本当は context の生存期間を短くしたほうがよい
        containerRegistry.RegisterInstance(new ErogeDiaryDbContext());

        containerRegistry.Register<GameMonitor>();
        containerRegistry.Register<ErogameScapeClient>();

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

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        CloseMutex();
    }

    private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                var message = $"{ex.Message}\n{ex.TargetSite?.Name}";
                MessageBox.Show(message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
                File.WriteAllText("errorlog.txt", ex.ToString());
            }
        }
        finally
        {
            CloseMutex();
        }
    }

    private void CloseMutex()
    {
        if (isNewInstance)
        {
            singleInstanceMutex.ReleaseMutex();
        }
        singleInstanceMutex.Close();
    }
}
