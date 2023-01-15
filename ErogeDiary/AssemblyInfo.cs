using System.Windows;

[assembly: 
    ThemeInfo(
        ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                         //(used if a resource is not found in the page,
                                         // or application resource dictionaries)
        ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                                  //(used if a resource is not found in the page,
                                                  // app, or any theme specific resource dictionaries)
    ), 
    // 基本的には Prism の BindableBase を使っているため適用範囲を制限
    PropertyChanged.FilterType("ErogeDiary.Models.Database.Entities."),
]
