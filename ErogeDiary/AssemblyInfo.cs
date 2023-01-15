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
    // ��{�I�ɂ� Prism �� BindableBase ���g���Ă��邽�ߓK�p�͈͂𐧌�
    PropertyChanged.FilterType("ErogeDiary.Models.Database.Entities."),
]
