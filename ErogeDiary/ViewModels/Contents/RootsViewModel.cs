using ErogeDiary.Controls;
using ErogeDiary.Models;
using ErogeDiary.Models.Database.Entities;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ErogeDiary.ViewModels.Contents;

public class RootsViewModel : ObservableObject
{
    public RelayCommand AddRootCommand { get; private set; }
    public RelayCommand EditRootCommand { get; private set; }
    public RelayCommand RemoveRootCommand { get; private set; }

    private IDialogService dialogService;
    private Game game;


    public RootsViewModel(IDialogService dialogService, Game game)
    {
        this.dialogService = dialogService;
        this.game = game;

        AddRootCommand = new RelayCommand(AddRoot, HasPlayTime);
        EditRootCommand = new RelayCommand(EditRoot, HasRoot);
        RemoveRootCommand = new RelayCommand(RemoveRoot, HasRoot);

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


    private ObservableCollection<ChartData> rootChartDataList = new();
    public ObservableCollection<ChartData> RootChartDataList
    {
        get => rootChartDataList;
        set { SetProperty(ref rootChartDataList, value); }
    }

    private void ReloadRootChartDataList()
    {
        if (game.TotalPlayTime == TimeSpan.Zero)
        {
            RootChartDataList = BuildNoData();
        }
        else
        {
            RootChartDataList = ToChartDataList(game.Roots);
        }

        AddRootCommand.NotifyCanExecuteChanged();
        EditRootCommand.NotifyCanExecuteChanged();
        RemoveRootCommand.NotifyCanExecuteChanged();
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

    private ObservableCollection<ChartData> BuildNoData()
    {
        return new ObservableCollection<ChartData>()
        {
            new ChartData()
            {
                Label = "No Data",
                Value = 1,
                ToolTip = "No Data",
                Color = new SolidColorBrush(Colors.DimGray)
            }
        };
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
