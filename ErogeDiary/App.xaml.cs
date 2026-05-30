using ErogeDiary.Dialogs;
using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Properties;
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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace ErogeDiary;

public partial class App : PrismApplication
{
    // 二重起動させない
    private static Mutex singleInstanceMutex = new Mutex(false, "ErogeDiary");
    private static bool isNewInstance = false;

    private int isFatalErrorShown = 0;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        DispatcherUnhandledException += HandleDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += HandleUnobservedTaskException;

        isNewInstance = singleInstanceMutex.WaitOne(0, false);
        if (!isNewInstance)
        {
            // TODO: MessageBox を出したいが表示されない
            singleInstanceMutex.Close();
            Shutdown();
            return;
        }

        UpgradeSettingsIfNeeded();
        ApplySavedUiCulture();

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
        ViewModelLocationProvider.Register<SettingsPage, SettingsViewModel>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 本当は context の生存期間を短くしたほうがよい
        containerRegistry.RegisterInstance(new ErogeDiaryDbContext());

        containerRegistry.Register<GameMonitor>();
        containerRegistry.Register<ErogameScapeClient>();

        containerRegistry.Register<IMessageDialog, MessageDialog>();
        containerRegistry.Register<IOpenFileDialog, OpenFileDialog>();

        containerRegistry.RegisterDialogWindow<ThemedDialogWindow>();

        containerRegistry.RegisterDialog<GameEditDialog, GameEditDialogViewModel>();
        containerRegistry.RegisterDialog<GameRegistrationDialog, GameRegistrationDialogViewModel>();
        containerRegistry.RegisterDialog<RootRegistrationDialog, RootRegistrationDialogViewModel>();
        containerRegistry.RegisterDialog<RootEditDialog, RootEditDialogViewModel>();
        containerRegistry.RegisterDialog<RootRemoveDialog, RootRemoveDialogViewModel>();
        containerRegistry.RegisterDialog<ThemeAccentColorDialog, ThemeAccentColorDialogViewModel>();
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
            if (e.ExceptionObject is Exception ex)
            {
                ReportFatalException(ex, source: nameof(AppDomain.CurrentDomain.UnhandledException));
            }
        }
        finally
        {
            CloseMutex();
        }
    }

    private void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            ReportFatalException(e.Exception, source: nameof(DispatcherUnhandledException));
        }
        finally
        {
            e.Handled = true;
            Shutdown(-1);
        }
    }

    private void HandleUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        try
        {
            ReportFatalException(e.Exception, source: nameof(TaskScheduler.UnobservedTaskException));
        }
        finally
        {
            e.SetObserved();
        }
    }

    private void ReportFatalException(Exception ex, string source)
    {
        WriteErrorLog(ex, source);

        // avoid showing multiple error dialogs
        if (Interlocked.Exchange(ref isFatalErrorShown, 1) != 0)
        {
            return;
        }

        var message =
            Strings.App_UnexpectedError + "\n"
            + $"Source: {source}\n"
            + $"{ex.Message}";
        MessageBox.Show(message, Strings.Dialog_ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void UpgradeSettingsIfNeeded()
    {
        // User-scoped settings are stored per app version, so copy previous values after an upgrade.
        if (!Settings.Default.UpgradeRequired)
        {
            return;
        }

        Settings.Default.Upgrade();
        Settings.Default.UpgradeRequired = false;
        Settings.Default.Save();
    }

    private void ApplySavedUiCulture()
    {
        var culture = SupportedLanguages.GetCultureOrDefault(Settings.Default.Language);

        // Apply only the UI language; keep regional formatting and week starts from the user's environment.
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        Strings.Culture = culture;
    }

    private void WriteErrorLog(Exception ex, string source)
    {
        try
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);

            var fileName = $"error_{DateTime.Now:yyyyMMdd_HHmmss}_{source}.txt";
            var path = Path.Combine(logDir, fileName);
            File.WriteAllText(path, ex.ToString());
        }
        catch
        {
            // ignore
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
