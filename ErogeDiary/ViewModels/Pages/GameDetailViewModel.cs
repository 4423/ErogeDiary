using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Properties;
using ErogeDiary.ViewModels.Contents;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace ErogeDiary.ViewModels.Pages;

public partial class GameDetailViewModel(
    ErogeDiaryDbContext database,
    IRegionManager regionManager,
    IMessageDialog messageDialog,
    IDialogService dialogService) : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private Game? game;

    partial void OnGameChanged(Game? value)
    {
        Roots = value == null ? null : new RootsViewModel(dialogService, value);
        PlayLogs = value == null ? null : new PlayLogsViewModel(value, database);
    }

    [ObservableProperty]
    private RootsViewModel? roots;

    [ObservableProperty]
    private PlayLogsViewModel? playLogs;


    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters["Game"] is Game game)
        {
            Game = game;
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
        => true;

    public void OnNavigatedFrom(NavigationContext navigationContext) {}

    [RelayCommand]
    private async Task StartGameAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Game?.ExecutableFilePath))
            {
                await messageDialog.ShowErrorAsync(Strings.Game_ExecutablePathMissing);
                return;
            }

            Process.Start(Game.ExecutableFilePath);
        }
        catch (Exception ex)
        {
            await messageDialog.ShowAsync(new MessageDialogParameters()
            {
                Title = Strings.Dialog_ErrorTitle,
                Message = string.Format(Strings.Game_StartFailedFormat, ex.Message),
                CloseButtonText = Strings.Common_Ok,
            });
        }
    }

    [RelayCommand]
    private void EditGame()
    {
        var dialogParams = new DialogParameters()
        {
            { "game", Game }
        };
        dialogService.ShowDialog(nameof(Views.Dialogs.GameEditDialog), dialogParams, null);
    }

    [RelayCommand]
    private async Task DeleteGameAsync()
    {
        var result = await messageDialog.ShowAsync(new MessageDialogParameters()
        {
            Title = Strings.Dialog_ConfirmTitle,
            Message = Strings.Game_DeleteRegistrationConfirmation,
            PrimaryButtonText = Strings.Common_Delete,
            CloseButtonText = Strings.Common_Cancel,
        });
        if (result == MessageDialogResult.Primary)
        {
            if (Game == null)
            {
                await messageDialog.ShowErrorAsync(Strings.Game_InfoMissingForDelete);
                return;
            }

            await database.RemoveAsync(Game);
            await messageDialog.ShowAsync(new MessageDialogParameters()
            {
                Title = Strings.Dialog_InfoTitle,
                Message = Strings.Game_DeleteRegistrationSucceeded,
                CloseButtonText = Strings.Common_Ok,
            });
            NavigationHelper.GetNavigationService(regionManager)?.Journal?.GoBack();
        }
    }
}
