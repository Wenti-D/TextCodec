using Microsoft.UI.Xaml.Controls;
using System;
using TextCodec.Views.Pages;
using Windows.Foundation;

namespace TextCodec.Services.Navigation;

public class NavigationService : INavigationService
{
    private Frame? _frame;
    private NavigationView? _navigationView;
    private NavigationViewItem? selectedNaviItem;

    private readonly TypedEventHandler<NavigationView, NavigationViewItemInvokedEventArgs> _navigationViewItemInvoked;
    private readonly TypedEventHandler<NavigationView, NavigationViewBackRequestedEventArgs> _navigationViewBackRequested;

    public Type? Current { get; }

    public NavigationService()
    {
        _navigationViewItemInvoked = ItemInvoked;
        _navigationViewBackRequested = BackRequested;
    }

    private NavigationView? NavigationView
    {
        get { return _navigationView; }
        set
        {
            if (_navigationView is not null)
            {
                _navigationView.ItemInvoked -= _navigationViewItemInvoked;
                _navigationView.BackRequested -= _navigationViewBackRequested;
            }

            _navigationView = value;

            if (_navigationView is not null)
            {
                _navigationView.ItemInvoked += _navigationViewItemInvoked;
                _navigationView.BackRequested += _navigationViewBackRequested;
            }
        }
    }

    public bool Navigate(Type pageType)
    {
        Type? currentType = _frame?.Content?.GetType();

        if (currentType == pageType) { return true; }

        SyncSelectedItem(pageType);

        try
        {
            return _frame?.Navigate(pageType, null) ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Navigate<T>() where T : Page
    {
        return Navigate(typeof(T));
    }

    public void Initialize(NavigationView navigationView, Frame frame)
    {
        _frame = frame;
        NavigationView = navigationView;
    }

    private bool SyncSelectedItem(Type? pageType)
    {
        if (NavigationView is null || pageType is null)
        {
            return false;
        }

        if (pageType == typeof(SettingsPage))
        {
            NavigationView.SelectedItem = NavigationView.SettingsItem;
        }
        else
        {
            foreach (object item in NavigationView.MenuItems)
            {
                if (Type.GetType((item as NavigationViewItem).Tag.ToString()) == pageType)
                {
                    NavigationView.SelectedItem = (NavigationViewItem)item;
                    break;
                }
            }
        }

        selectedNaviItem = (NavigationViewItem)NavigationView.SelectedItem;
        return true;
    }

    public void GoBack()
    {
        if (_frame?.CanGoBack ?? false)
        {
            _frame.GoBack();
            SyncSelectedItem(_frame.Content.GetType());
            GC.Collect();
        }
    }

    private void ItemInvoked(NavigationView sender,  NavigationViewItemInvokedEventArgs e)
    {
        selectedNaviItem = (NavigationViewItem)_navigationView?.SelectedItem;
        Type? target = e.IsSettingsInvoked ? typeof(SettingsPage) : Type.GetType(selectedNaviItem?.Tag.ToString());

        if (target is not null)
        {
            Navigate(target);
            GC.Collect();
        }
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        GoBack();
    }
}
