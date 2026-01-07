using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using WpfApp = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace TwitchChatOverlay;

public class TrayIconManager : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly MainWindow _mainWindow;

    public TrayIconManager(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        
        _notifyIcon = new NotifyIcon
        {
            Icon = LoadCustomIcon(),
            Visible = true,
            Text = "Twitch Chat Overlay"
        };

        // left-click shows/focuses window
        _notifyIcon.Click += (s, e) =>
        {
            if (e is MouseEventArgs me && me.Button == MouseButtons.Left)
            {
                ShowMainWindow();
            }
        };

        CreateContextMenu();
    }

    private void CreateContextMenu()
    {
        var contextMenu = new ContextMenuStrip();

        // channel selection
        var channelItem = new ToolStripMenuItem("Set Channel...");
        channelItem.Click += (s, e) => ShowChannelDialog();
        contextMenu.Items.Add(channelItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // mode toggle
        var modeItem = new ToolStripMenuItem("Toggle Borders (Setup/Overlay)");
        modeItem.Click += (s, e) => _mainWindow.ToggleBorderVisibility();
        contextMenu.Items.Add(modeItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // opacity submenu
        var opacityMenu = new ToolStripMenuItem("Opacity");
        
        var increaseOpacity = new ToolStripMenuItem("Increase (+15)");
        increaseOpacity.Click += (s, e) => _mainWindow.IncreaseOpacity();
        opacityMenu.DropDownItems.Add(increaseOpacity);

        var decreaseOpacity = new ToolStripMenuItem("Decrease (-15)");
        decreaseOpacity.Click += (s, e) => _mainWindow.DecreaseOpacity();
        opacityMenu.DropDownItems.Add(decreaseOpacity);

        var resetOpacity = new ToolStripMenuItem("Reset (50%)");
        resetOpacity.Click += (s, e) => _mainWindow.ResetOpacity();
        opacityMenu.DropDownItems.Add(resetOpacity);

        contextMenu.Items.Add(opacityMenu);

        contextMenu.Items.Add(new ToolStripSeparator());

        // settings
        var settingsItem = new ToolStripMenuItem("Settings...");
        settingsItem.Click += (s, e) => _mainWindow.ShowSettings();
        contextMenu.Items.Add(settingsItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // reset position
        var resetItem = new ToolStripMenuItem("Reset Window Position");
        resetItem.Click += (s, e) => _mainWindow.ResetWindowPosition();
        contextMenu.Items.Add(resetItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // exit
        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (s, e) => WpfApp.Current.Shutdown();
        contextMenu.Items.Add(exitItem);

        _notifyIcon.ContextMenuStrip = contextMenu;
    }

    private void ShowMainWindow()
    {
        _mainWindow.Dispatcher.Invoke(() =>
        {
            if (_mainWindow.WindowState == WindowState.Minimized)
            {
                _mainWindow.WindowState = WindowState.Normal;
            }
            _mainWindow.Show();
            _mainWindow.Activate();
            _mainWindow.Focus();
        });
    }

    private Icon LoadCustomIcon()
    {
        try
        {
            if (System.IO.File.Exists("icon.ico"))
            {
                return new Icon("icon.ico");
            }
        }
        catch { }
        
        return SystemIcons.Application;
    }

    private void ShowChannelDialog()
    {
        _mainWindow.Dispatcher.Invoke(() =>
        {
            ShowMainWindow();
            var currentChannel = _mainWindow.GetCurrentChannel();
            var dialog = new ChannelInputDialog(currentChannel)
            {
                Owner = _mainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                _mainWindow.SetChannel(dialog.ChannelName);
            }
        });
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
    }
}
