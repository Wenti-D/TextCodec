using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using TextCodec.Core;
using TextCodec.Services.Navigation;
using TextCodec.ViewModels;
using TextCodec.Views.Pages;
using Windows.Graphics;
using WinRT.Interop;

namespace TextCodec;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private AppWindow appWindow;
    private IServiceProvider serviceProvider;

    /// <summary>
    /// 初始化程序对象单例，这里是程序入口。
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 程序启动时唤起。
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        serviceProvider = ConfigureService();
        Window mainWindow = serviceProvider.GetRequiredService<MainWindow>();

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            mainWindow.SizeChanged += SizeChanged;

            appWindow = GetAppWindow(mainWindow);
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
        mainWindow.Activate();
    }

    /// <summary>
    /// 配置依赖注入服务，可通过 IoC 获取服务提供器。
    /// </summary>
    /// <returns>配置好的服务提供器</returns>
    private static ServiceProvider ConfigureService()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton<INavigationService, NavigationService>()

            .AddSingleton<AppSettings>()

            .AddSingleton<MainWindow>()
            .AddSingleton<CodecPage>()
            .AddSingleton<CodecViewModel>()
            .AddSingleton<HashPage>()
            .AddSingleton<HashViewModel>()
            .AddSingleton<SettingsPage>()
            .AddSingleton<SettingsViewModel>()
            .BuildServiceProvider(true);

        Ioc.Default.ConfigureServices(serviceProvider);

        return serviceProvider;
    }

    private void SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        RectInt32[] rects = new RectInt32[] { new(48, 0, (int)args.Size.Width - 48, 48) };
        appWindow.TitleBar.SetDragRectangles(rects);
    }

    private static AppWindow GetAppWindow(Window window)
    {
        IntPtr hwnd = WindowNative.GetWindowHandle(window);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        return AppWindow.GetFromWindowId(windowId);
    }
}
