using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using TextCodec.Core;
using Vanara.PInvoke;
using Windows.Graphics;
using Windows.Storage;
using WinRT.Interop;

namespace TextCodec
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IntPtr hwnd;
        private AppWindow appWindow;
        private static AppSettings appSettings;

        public MainWindow()
        {
            InitializeComponent();

            Title = $"Text Codec";

            // 初始化程序设定
            appSettings = new AppSettings();
            appSettings.PropertyChanged += Settings_PropertyChanged;

            // 根据设定更改背景
            ChangeSystemBackdrop(appSettings.BackdropType);

            // 获取窗体句柄
            hwnd = WindowNative.GetWindowHandle(this);
            WindowId id = Win32Interop.GetWindowIdFromWindow(hwnd);
            appWindow = AppWindow.GetFromWindowId(id);

            // 设置图标
            IntPtr h_icon = User32.LoadImage(hwnd, "Assets/AppIcon.ico",
                User32.LoadImageType.IMAGE_ICON, 16, 16, User32.LoadImageOptions.LR_LOADFROMFILE);
            User32.SendMessage(hwnd, User32.WindowMessage.WM_SETICON, (IntPtr)0, h_icon);

            NavigationViewController.SelectedItem = NavigationViewController.MenuItems[0];
            ContentFrame.Navigate(typeof(Views.Pages.CodecPage), null, new EntranceNavigationTransitionInfo());

            Closed += MainWindow_Closed;

            // 按上次关闭时的窗口位置、大小显示窗口
            if (appSettings.IsMainWindowMaximum is true)
            {
                User32.ShowWindow(hwnd, ShowWindowCommand.SW_SHOWMAXIMIZED);
            }
            else
            {
                var rect = new WindowRect(appSettings.MainWindowRect);
                var scr_area = DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Primary);

                if (rect.Left >= 0 && rect.Top >= 0
                 && rect.Width > 0 && rect.Height > 0
                 && rect.Right <= scr_area.WorkArea.Width 
                 && rect.Bottom <= scr_area.WorkArea.Height)
                {
                    appWindow.MoveAndResize(rect.ToRectInt32());
                }
                else
                {
                    appWindow.Resize(new SizeInt32(1100, 800));
                }
            }
        }

        public static AppSettings AppSettings
        {
            get { return appSettings; }
            set { }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(appSettings.BackdropType))
            {
                ChangeSystemBackdrop(((AppSettings)sender!).BackdropType);
            }
        }

        /// <summary>
        /// 更改应用背景材质样式
        /// </summary>
        /// <param name="backdrop">背景材质</param>
        private void ChangeSystemBackdrop(BackdropTypes backdrop)
        {
            SystemBackdrop = backdrop switch
            {
                BackdropTypes.Mica => new MicaBackdrop() { Kind = MicaKind.Base },
                BackdropTypes.MicaAlt => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
                BackdropTypes.Acrylic => new DesktopAcrylicBackdrop(),
                _ => null
            };
        }

        /// <summary>
        /// 关闭窗口时的附加工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            var window_placement = new User32.WINDOWPLACEMENT();
            if (User32.GetWindowPlacement(hwnd, ref window_placement))
            {
                appSettings.IsMainWindowMaximum = window_placement.showCmd == ShowWindowCommand.SW_MAXIMIZE;
                var pos = appWindow.Position;
                var size = appWindow.Size;
                var rect = new WindowRect(pos.X, pos.Y, size.Width, size.Height);
                appSettings.MainWindowRect = rect.val;
            }
            if (appSettings.OnReset)
            {
                await ApplicationData.Current.ClearAsync(ApplicationDataLocality.Local);
            }
        }

        private void NavigationViewController_ItemInvoked(NavigationView sender,
            NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Views.Pages.SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null && args.InvokedItemContainer.Tag != null)
            {
                Type newPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                ContentFrame.Navigate(newPageType, null, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationViewController_BackRequested(NavigationView sender,
            NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack) { ContentFrame.GoBack(); }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs args)
        {
            NavigationViewController.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(Views.Pages.SettingsPage))
            {
                NavigationViewController.SelectedItem = (NavigationViewItem)NavigationViewController.SettingsItem;
            }
            else if (ContentFrame.SourcePageType != null)
            {
                NavigationViewController.SelectedItem = NavigationViewController.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
            }

            // 什么玩意，导航切来切去还能让内存水涨船高
            GC.Collect();
        }
    }
}
