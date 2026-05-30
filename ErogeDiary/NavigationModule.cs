using ErogeDiary.Views.Pages;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary;

public class NavigationModule(IRegionManager regionManager) : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        NavigationHelper.RequestNavigateToHomePage(regionManager);
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<GameDetailPage>(nameof(GameDetailPage));
        containerRegistry.RegisterForNavigation<HomePage>(nameof(HomePage));
        containerRegistry.RegisterForNavigation<SettingsPage>(nameof(SettingsPage));
    }
}

public static class NavigationHelper
{
    public static string RegionName = "FrameRegion";

    public static IRegionNavigationService GetNavigationService(IRegionManager regionManager)
        => regionManager.Regions[RegionName].NavigationService;

    public static void RequestNavigate(IRegionManager regionManager, string source, NavigationParameters? parameters = null)
    {
        if (IsCurrentNavigationTarget(regionManager, source, parameters))
        {
            return;
        }

        if (parameters == null)
        {
            regionManager.RequestNavigate(RegionName, source);
        }
        else
        {
            regionManager.RequestNavigate(RegionName, source, parameters);
        }
    }

    public static void RequestNavigateToHomePage(IRegionManager regionManager, NavigationParameters? parameters = null)
        => RequestNavigate(regionManager, nameof(HomePage), parameters);

    public static void RequestNavigateToGameDetailPage(IRegionManager regionManager, NavigationParameters? parameters = null)
        => RequestNavigate(regionManager, nameof(GameDetailPage), parameters);

    public static void RequestNavigateToSettingsPage(IRegionManager regionManager, NavigationParameters? parameters = null)
        => RequestNavigate(regionManager, nameof(SettingsPage), parameters);

    private static bool IsCurrentNavigationTarget(
        IRegionManager regionManager,
        string source,
        NavigationParameters? parameters)
    {
        if (parameters != null)
        {
            return false;
        }

        var currentUri = GetNavigationService(regionManager).Journal.CurrentEntry?.Uri;
        var currentSource = currentUri?.OriginalString;

        return string.Equals(currentSource, source, StringComparison.Ordinal);
    }
}
