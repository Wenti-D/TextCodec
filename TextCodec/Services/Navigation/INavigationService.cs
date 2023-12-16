using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Services.Navigation;

public interface INavigationService
{
    // 当前页面类型
    Type? Current { get; }

    bool Navigate(Type pageType);

    bool Navigate<TPage>() where TPage : Page;

    void Initialize(NavigationView navigationView, Frame frame);

    void GoBack();  
}
