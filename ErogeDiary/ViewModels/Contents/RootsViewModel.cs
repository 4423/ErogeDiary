using ErogeDiary.Controls;
using ErogeDiary.Models;
using ErogeDiary.Models.Database.Entities;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ErogeDiary.ViewModels.Contents;

public class RootsViewModel : BindableBase
{
    public DelegateCommand AddRootCommand { get; private set; }
    public DelegateCommand EditRootCommand { get; private set; }
    public DelegateCommand RemoveRootCommand { get; private set; }

    private IDialogService dialogService;
    private Game game;


    public RootsViewModel(IDialogService dialogService, Game game)
    {
        this.dialogService = dialogService;
        this.game = game;

        AddRootCommand = new DelegateCommand(AddRoot, HasPlayTime);
        EditRootCommand = new DelegateCommand(EditRoot, HasRoot);
        RemoveRootCommand = new DelegateCommand(RemoveRoot, HasRoot);

        this.game.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Game.TotalPlayTime) ||
                e.PropertyName == nameof(Game.Roots))
            {
                ReloadRootChartDataList();
            }
        };
        ReloadRootChartDataList();
    }


    private ObservableCollection<ChartData> rootChartDataList;
    public ObservableCollection<ChartData> RootChartDataList
    {
        get => rootChartDataList;
        set { SetProperty(ref rootChartDataList, value); }
    }

    private void ReloadRootChartDataList()
    {
        RootChartDataList = ToChartDataList(game.Roots);

        AddRootCommand.RaiseCanExecuteChanged();
        EditRootCommand.RaiseCanExecuteChanged();
        RemoveRootCommand.RaiseCanExecuteChanged();
    }
        
    private ObservableCollection<ChartData> ToChartDataList(IEnumerable<Root> roots)
    {
        var unallocatedTime = game.GetUnallocatedTime();
        var unallocatedData = new ChartData()
        {
            Label = "（未割り当てのルート）",
            Value = unallocatedTime.TotalSeconds,
            ToolTip = unallocatedTime.ToPlayTimeString(),
            Color = new SolidColorBrush(Colors.DimGray)
        };

        if (roots.Count() == 0)
        {
            return new ObservableCollection<ChartData>() { unallocatedData };
        }

        var charts = new ObservableCollection<ChartData>(roots.Select(r => {
            var tooltip = r.PlayTime.ToPlayTimeString();
            if (r.IsCleared)
            {
                tooltip += Environment.NewLine + $"{r.ClearedAt?.ToLongDateString()}に攻略";
            }
            return new ChartData()
            {
                Label = r.Name,
                Value = r.PlayTime.TotalSeconds,
                ToolTip = tooltip,
                Color = new SolidColorBrush(r.Color),
            };
        }));
        charts.Add(unallocatedData);
        return charts;
    }

    private bool HasPlayTime() => game.GetUnallocatedTime().TotalSeconds > 0;

    private bool HasRoot() => game.Roots.Count > 0;

    private void AddRoot()
    {
        ShowDialog(nameof(Views.Dialogs.RootRegistrationDialog));
        ReloadRootChartDataList();
    }

    private void EditRoot()
    {
        ShowDialog(nameof(Views.Dialogs.RootEditDialog));
        ReloadRootChartDataList();
    }

    private void RemoveRoot()
    {
        ShowDialog(nameof(Views.Dialogs.RootRemoveDialog));
        ReloadRootChartDataList();
    }

    private void ShowDialog(string dialogName)
    {
        var dialogParams = new DialogParameters()
        {
            { "game", game }
        };
        dialogService.ShowDialog(dialogName, dialogParams, null);
    }
}
