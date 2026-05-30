using ErogeDiary.Dialogs;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Properties;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErogeDiary.ViewModels.Dialogs;

public partial class RootRemoveDialogViewModel(
    ErogeDiaryDbContext database,
    IMessageDialog messageDialog) : BindableDialogBase
{
    private Game? game;

    [ObservableProperty]
    private ICollection<Root>? roots;

    [ObservableProperty]
    private Root? selectedRoot;

    partial void OnSelectedRootChanged(Root? value)
    {
        RemoveCommand.NotifyCanExecuteChanged();
    }


    public override void OnDialogOpened(IDialogParameters parameters)
    {
        game = parameters.GetValue<Game>("game");
        Roots = game.Roots;
        SelectedRoot = game.Roots.FirstOrDefault();
    }

    private bool CanExecuteRemoveRoot()
        => SelectedRoot != null;

    [RelayCommand(CanExecute = nameof(CanExecuteRemoveRoot))]
    private async Task RemoveAsync()
    {
        try
        {
            await database.RemoveRootAsync(SelectedRoot!);
            CloseDialogOK();
        }
        catch (Exception)
        {
            await messageDialog.ShowErrorAsync(Strings.Root_RemoveFailed);
        }
    }
}
