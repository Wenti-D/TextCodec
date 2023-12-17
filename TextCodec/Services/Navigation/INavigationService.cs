using Microsoft.UI.Xaml.Controls;
using System;

namespace TextCodec.Services.Navigation;
#nullable enable

public interface INavigationService
{
    // 当前页面类型
    Type? Current { get; }

    bool Navigate(Type pageType);

    bool Navigate<TPage>() where TPage : Page;

    void Initialize(NavigationView navigationView, Frame frame);

    void GoBack();  
}
