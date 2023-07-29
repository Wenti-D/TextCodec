using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Storage;
using Windows.System;
using WinRT.Interop;
using Vanara.PInvoke;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TextCodec
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IntPtr hwnd;
        private AppWindow appWindow;

        public MainWindow()
        {
            InitializeComponent();

            hwnd = WindowNative.GetWindowHandle(this);
            WindowId id = Win32Interop.GetWindowIdFromWindow(hwnd);
            appWindow = AppWindow.GetFromWindowId(id);

            NavigationViewController.SelectedItem = NavigationViewController.MenuItems[0];
            ContentFrame.Navigate(typeof(Views.Pages.CodecPage), null, new EntranceNavigationTransitionInfo());

            Closed += MainWindow_Closed;
            if (ApplicationData.Current.LocalSettings.Values["IsMainWindowMaximum"] is true)
            {
                User32.ShowWindow(hwnd, ShowWindowCommand.SW_SHOWMAXIMIZED);
            }
            else if (ApplicationData.Current.LocalSettings.Values["MainWindowRect"] is ulong value)
            {
                var rect = new WindowRect(value);
                var scr_area = DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Primary);

                if (rect.Left >= 0 && rect.Top >= 0 && rect.Right <= scr_area.WorkArea.Width && rect.Bottom <= scr_area.WorkArea.Height)
                {
                    appWindow.MoveAndResize(rect.ToRectInt32());
                }
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            //throw new NotImplementedException();
            var window_placement = new User32.WINDOWPLACEMENT();
            if (User32.GetWindowPlacement(hwnd, ref window_placement))
            {
                ApplicationData.Current.LocalSettings.Values["IsMainWindowMaximum"] = window_placement.showCmd == ShowWindowCommand.SW_MAXIMIZE;
                var pos = appWindow.Position;
                var size = appWindow.Size;
                var rect = new WindowRect(pos.X, pos.Y, size.Width, size.Height);
                ApplicationData.Current.LocalSettings.Values["MainWindowRect"] = rect.val;
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

        [StructLayout(LayoutKind.Explicit)]
        private struct WindowRect
        {
            [FieldOffset(0)]
            public ulong val;
            [FieldOffset(0)]
            public short x;
            [FieldOffset(2)]
            public short y;
            [FieldOffset(4)]
            public short width;
            [FieldOffset(6)]
            public short height;

            public int Left => x;
            public int Top => y;
            public int Right => x + width;
            public int Bottom => y + height;

            public WindowRect(int X, int Y, int Width, int Height)
            {
                x = (short)X; y = (short)Y;
                width = (short)Width; height = (short)Height;
            }

            public WindowRect(ulong Value)
            {
                val = Value;
            }

            public RectInt32 ToRectInt32()
            {
                return new RectInt32(x, y, width, height);
            }
        }
    }
}
