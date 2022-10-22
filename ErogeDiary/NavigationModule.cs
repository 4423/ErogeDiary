using ErogeDiary.Views.Pages;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary
{
    public class NavigationModule : IModule
    {
        private IRegionManager regionManager;

        public NavigationModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            NavigationHelper.RequestNavigateToHomePage(regionManager);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<GameDetailPage>(nameof(GameDetailPage));
            containerRegistry.RegisterForNavigation<HomePage>(nameof(HomePage));
        }
    }

    public static class NavigationHelper
    {
        public static string RegionName = "FrameRegion";

        public static IRegionNavigationService GetNavigationService(IRegionManager regionManager)
            => regionManager.Regions[RegionName].NavigationService;

        public static void RequestNavigate(IRegionManager regionManager, string source, NavigationParameters parameters=null)
        {
            if (parameters == null)
            {
                regionManager.RequestNavigate(RegionName, source);
            }
            else
            {
                regionManager.RequestNavigate(RegionName, source, parameters);
            }
        }

        public static void RequestNavigateToHomePage(IRegionManager regionManager, NavigationParameters parameters = null)
            => RequestNavigate(regionManager, nameof(HomePage), parameters);

        public static void RequestNavigateToGameDetailPage(IRegionManager regionManager, NavigationParameters parameters = null)
            => RequestNavigate(regionManager, nameof(GameDetailPage), parameters);
    }
}
