using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using WinRT.Interop;

namespace TextCodec
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private AppWindow appWindow;

        /// <summary>
        /// 初始化单例程序对象。
        /// 这里是程序入口。
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

        private Window m_window;
    }
}
