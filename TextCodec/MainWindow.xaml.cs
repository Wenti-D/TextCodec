using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using TextCodec.Core;
using TextCodec.Services.Navigation;
using TextCodec.Views.Pages;
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
        private readonly AppSettings appSettings;
        private readonly IServiceProvider serviceProvider;
        private readonly INavigationService navigationService;

        public MainWindow()
        {
            Title = $"Text Codec";
            serviceProvider = Ioc.Default;
            appSettings = serviceProvider.GetService<AppSettings>();
            appSettings.PropertyChanged += Settings_PropertyChanged;
            navigationService = serviceProvider.GetService<INavigationService>();

            InitializeComponent();
            ConfigureWindow();

            navigationService.Initialize(NavigationViewController, ContentFrame);
            navigationService.Navigate(typeof(CodecPage));

            Closed += MainWindow_Closed;
        }

        /// <summary>
        /// ���ô��ڴ�С��ͼ�����ʽ
        /// </summary>
        private void ConfigureWindow()
        {
            // ��ȡ������
            hwnd = WindowNative.GetWindowHandle(this);
            WindowId id = Win32Interop.GetWindowIdFromWindow(hwnd);
            appWindow = AppWindow.GetFromWindowId(id);

            // ����ͼ��
            IntPtr h_icon = User32.LoadImage(hwnd, "Assets/AppIcon.ico",
                User32.LoadImageType.IMAGE_ICON, 16, 16, User32.LoadImageOptions.LR_LOADFROMFILE);
            User32.SendMessage(hwnd, User32.WindowMessage.WM_SETICON, (IntPtr)0, h_icon);

            // ���ϴιر�ʱ�Ĵ���λ�á���С��ʾ����
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

            // �����趨���ı���
            ChangeSystemBackdrop(appSettings.BackdropType);
        }

        /// <summary>
        /// �ڴ��ڱ����������ø���ʱ���´���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(appSettings.BackdropType))
            {
                ChangeSystemBackdrop(((AppSettings)sender!).BackdropType);
            }
        }

        /// <summary>
        /// ����Ӧ�ñ���������ʽ
        /// </summary>
        /// <param name="backdrop">��������</param>
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
        /// �رմ���ʱ�ĸ��ӹ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainWindow_Closed(object sender, WindowEventArgs args)
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
        }
    }
}
