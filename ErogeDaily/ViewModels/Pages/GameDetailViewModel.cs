using ErogeDaily.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.ViewModels.Pages
{
    public class GameDetailViewModel : BindableBase, INavigationAware
    {
        private Game game;
        public Game Game
        {
            get => game; 
            set { SetProperty(ref game, value); }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var game = navigationContext.Parameters["Game"] as Game;
            if(game != null)
            {
                Game = game;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) {}
    }
}
