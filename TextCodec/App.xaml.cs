using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TextCodec.Helpers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using WinRT;
using WinRT.Interop;

namespace TextCodec
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private AppWindow appWindow;
        private WindowsSystemDispatcherQueueHelper wsdqHelper;
        private Microsoft.UI.Composition.SystemBackdrops.MicaController micaController;
        private Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration backdropConfiguration;

        /// <summary>
        /// 初始化单例程序对象。
        /// 这里是程序入口。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 程序启动时唤起。
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                m_window.SizeChanged += SizeChanged;

                appWindow = GetAppWindow(m_window);
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            }
            m_window.Activate();
            TrySetMicaBackdrop();
        }

        private void SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            RectInt32[] rects = new RectInt32[]
                {new RectInt32(48, 0, (int)args.Size.Width - 48, 48)};
            appWindow.TitleBar.SetDragRectangles(rects);
        }

        private AppWindow GetAppWindow(Window window)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(window);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            return AppWindow.GetFromWindowId(windowId);
        }

        bool TrySetMicaBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                backdropConfiguration = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                m_window.Activated += Window_Activated;
                m_window.Closed += Window_Closed;
                ((FrameworkElement)m_window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                backdropConfiguration.IsInputActive = true;
                SetConfigurationSourceTheme();

                micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                micaController.AddSystemBackdropTarget(m_window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                micaController.SetSystemBackdropConfiguration(backdropConfiguration);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system

        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            backdropConfiguration.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (micaController != null)
            {
                micaController.Dispose();
                micaController = null;
            }
            m_window.Activated -= Window_Activated;
            backdropConfiguration = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (backdropConfiguration != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)m_window.Content).ActualTheme)
            {
                case ElementTheme.Dark: backdropConfiguration.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: backdropConfiguration.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: backdropConfiguration.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }

        private Window m_window;
    }
}
