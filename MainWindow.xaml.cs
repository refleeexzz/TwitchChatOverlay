using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WpfApp = System.Windows.Application;

namespace TwitchChatOverlay;

public partial class MainWindow : Window
{
    private AppSettings _settings;
    private WindowDisplayMode _currentMode = WindowDisplayMode.Setup;
    private IntPtr _hwnd;
    private TrayIconManager? _trayIcon;
    private TcpClient? _twitchIrcClient;
    private StreamReader? _twitchReader;
    private StreamWriter? _twitchWriter;
    private bool _isConnected = false;
    
    public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
    
    private readonly Thickness _noBorderThickness = new Thickness(0);
    private readonly Thickness _borderThickness = new Thickness(4);

    public MainWindow()
    {
        InitializeComponent();
        
        _settings = AppSettings.Load();
        ApplyWindowSettings();
        ChatMessages.ItemsSource = Messages;
        
        // show placeholder if no channel set
        if (string.IsNullOrWhiteSpace(_settings.TwitchChannel))
        {
            AddSystemMessage("Waiting for you to set a Twitch channel...");
            AddSystemMessage("Right-click the tray icon → Set Channel");
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _hwnd = new WindowInteropHelper(this).Handle;
        
        // apply toolwindow style to remain hidden from screen capture software
        if (_settings.HideFromOBS)
        {
            WindowHelper.SetWindowToolWindow(_hwnd);
        }
        
        SetDisplayMode(_settings.AutoHideBorders ? WindowDisplayMode.Overlay : WindowDisplayMode.Setup);
        WindowHelper.SetWindowTopMost(_hwnd);
        
        // initialize tray icon
        _trayIcon = new TrayIconManager(this);
        
        // connect to twitch if channel is set
        if (!string.IsNullOrWhiteSpace(_settings.TwitchChannel))
        {
            ConnectToTwitch(_settings.TwitchChannel);
        }
        else
        {
            UpdateStatus("No channel configured");
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _settings.WindowLeft = this.Left;
        _settings.WindowTop = this.Top;
        _settings.WindowWidth = this.Width;
        _settings.WindowHeight = this.Height;
        _settings.Save();
        
        DisconnectFromTwitch();
        _trayIcon?.Dispose();
    }

    private void ApplyWindowSettings()
    {
        this.Left = _settings.WindowLeft;
        this.Top = _settings.WindowTop;
        this.Width = _settings.WindowWidth;
        this.Height = _settings.WindowHeight;
        
        SetBackgroundOpacity(_settings.OpacityLevel);
    }

    #region Display Mode Management

    private void SetDisplayMode(WindowDisplayMode mode)
    {
        _currentMode = mode;

        switch (mode)
        {
            case WindowDisplayMode.Setup:
                ShowSetupMode();
                break;

            case WindowDisplayMode.Overlay:
                ShowOverlayMode();
                break;
        }

        if (_hwnd != IntPtr.Zero)
        {
            WindowHelper.SetWindowTopMost(_hwnd);
        }
    }

    private void ShowSetupMode()
    {
        if (_hwnd != IntPtr.Zero)
        {
            WindowHelper.SetWindowInteractable(_hwnd);
        }

        HeaderBar.Visibility = Visibility.Visible;
        FooterBar.Visibility = Visibility.Visible;
        this.ResizeMode = ResizeMode.CanResizeWithGrip;
        this.ShowInTaskbar = true;
        MainGrid.Margin = _borderThickness;
        
        BtnToggleBorders.ToolTip = "Hide Borders (Overlay Mode)";
        BtnToggleBorders.Content = "○";
        
        UpdateStatus("Setup Mode - Move and resize the window");
    }

    private void ShowOverlayMode()
    {
        if (_hwnd != IntPtr.Zero)
        {
            WindowHelper.SetWindowClickThrough(_hwnd);
        }

        HeaderBar.Visibility = Visibility.Collapsed;
        FooterBar.Visibility = Visibility.Collapsed;
        this.ResizeMode = ResizeMode.NoResize;
        
        // keep taskbar icon always visible
        this.ShowInTaskbar = true;
        
        MainGrid.Margin = _noBorderThickness;
        
        UpdateStatus("Overlay Mode - Click-through enabled");
    }

    #endregion

    #region Opacity Management

    private void SetBackgroundOpacity(byte opacity)
    {
        double remappedOpacity = opacity / 255.0;
        
        // keep window slightly visible during setup to avoid losing it
        if (_currentMode == WindowDisplayMode.Setup && remappedOpacity < 0.01)
        {
            remappedOpacity = 0.01;
        }
        
        OverlayBackground.Opacity = remappedOpacity;
        _settings.OpacityLevel = opacity;
    }

    public void IncreaseOpacity()
    {
        byte newOpacity = (byte)Math.Clamp(_settings.OpacityLevel + 15, 0, 255);
        SetBackgroundOpacity(newOpacity);
        UpdateStatus($"Opacity: {(int)(newOpacity / 255.0 * 100)}%");
    }

    public void DecreaseOpacity()
    {
        byte newOpacity = (byte)Math.Clamp(_settings.OpacityLevel - 15, 0, 255);
        SetBackgroundOpacity(newOpacity);
        UpdateStatus($"Opacity: {(int)(newOpacity / 255.0 * 100)}%");
    }

    public void ResetOpacity()
    {
        SetBackgroundOpacity(128);
        UpdateStatus("Opacity reset to 50%");
    }

    #endregion

    #region Twitch Integration

    private async void ConnectToTwitch(string channel)
    {
        try
        {
            DisconnectFromTwitch();
            
            _isConnected = false;
            UpdateStatus($"Connecting to #{channel}...");

            _twitchIrcClient = new TcpClient();
            await _twitchIrcClient.ConnectAsync("irc.chat.twitch.tv", 6667);

            _twitchReader = new StreamReader(_twitchIrcClient.GetStream());
            _twitchWriter = new StreamWriter(_twitchIrcClient.GetStream()) { AutoFlush = true };

            // anonymous login (no oauth needed for reading)
            var username = "justinfan" + new Random().Next(10000, 99999);
            await _twitchWriter.WriteLineAsync($"NICK {username}");
            await _twitchWriter.WriteLineAsync($"JOIN #{channel.ToLower()}");

            _isConnected = true;
            Messages.Clear();
            AddSystemMessage($"Connected to #{channel}");
            UpdateStatus($"Connected to #{channel}");

            // start reading messages
            _ = Task.Run(ReadTwitchChat);
        }
        catch (Exception ex)
        {
            _isConnected = false;
            AddSystemMessage($"Connection error: {ex.Message}");
            UpdateStatus("Connection failed");
        }
    }

    private async Task ReadTwitchChat()
    {
        try
        {
            while (_isConnected && _twitchReader != null)
            {
                var line = await _twitchReader.ReadLineAsync();
                if (line == null) break;

                // respond to ping
                if (line.StartsWith("PING"))
                {
                    await _twitchWriter!.WriteLineAsync("PONG :tmi.twitch.tv");
                    continue;
                }

                // parse chat messages
                // format: :username!username@username.tmi.twitch.tv PRIVMSG #channel :message
                if (line.Contains("PRIVMSG"))
                {
                    try
                    {
                        var userEnd = line.IndexOf('!');
                        var messageStart = line.IndexOf(':', 1);
                        
                        if (userEnd > 1 && messageStart > userEnd)
                        {
                            var username = line.Substring(1, userEnd - 1);
                            var message = line.Substring(messageStart + 1);
                            
                            AddChatMessage(username, message);
                        }
                    }
                    catch { /* skip malformed messages */ }
                }
            }
        }
        catch (Exception ex)
        {
            if (_isConnected)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    AddSystemMessage($"Disconnected: {ex.Message}");
                    UpdateStatus("Disconnected");
                });
            }
        }
        finally
        {
            _isConnected = false;
        }
    }

    private void DisconnectFromTwitch()
    {
        _isConnected = false;
        
        try
        {
            _twitchWriter?.Close();
            _twitchReader?.Close();
            _twitchIrcClient?.Close();
        }
        catch { /* ignore cleanup errors */ }
        
        _twitchWriter?.Dispose();
        _twitchReader?.Dispose();
        _twitchIrcClient?.Dispose();
        
        _twitchWriter = null;
        _twitchReader = null;
        _twitchIrcClient = null;
    }

    #endregion

    #region Chat Message Management

    private void AddSystemMessage(string message)
    {
        Dispatcher.Invoke(() =>
        {
            Messages.Add(new ChatMessage 
            { 
                Username = "System", 
                Message = message,
                IsSystemMessage = true
            });
            
            ChatScroll.ScrollToBottom();
            
            if (Messages.Count > 100)
            {
                Messages.RemoveAt(0);
            }
        });
    }

    public void AddChatMessage(string username, string message)
    {
        Dispatcher.Invoke(() =>
        {
            Messages.Add(new ChatMessage { Username = username, Message = message });
            ChatScroll.ScrollToBottom();
            
            // cap at 100 messages to avoid memory bloat
            if (Messages.Count > 100)
            {
                Messages.RemoveAt(0);
            }
        });
    }

    #endregion

    #region UI Event Handlers

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && _currentMode == WindowDisplayMode.Setup)
        {
            try
            {
                this.DragMove();
            }
            catch { /* drag cancelled or interrupted */ }
        }
    }

    private void HeaderBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_currentMode == WindowDisplayMode.Setup)
        {
            try
            {
                this.DragMove();
            }
            catch { /* drag cancelled or interrupted */ }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        WpfApp.Current.Shutdown();
    }

    private void BtnToggleBorders_Click(object sender, RoutedEventArgs e)
    {
        ToggleBorderVisibility();
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsDialog();
    }

    #endregion

    #region Context Menu Handlers

    private void MenuItem_ToggleBorders_Click(object sender, RoutedEventArgs e)
    {
        ToggleBorderVisibility();
    }

    private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsDialog();
    }

    private void MenuItem_IncreaseOpacity_Click(object sender, RoutedEventArgs e)
    {
        IncreaseOpacity();
    }

    private void MenuItem_DecreaseOpacity_Click(object sender, RoutedEventArgs e)
    {
        DecreaseOpacity();
    }

    private void MenuItem_ResetOpacity_Click(object sender, RoutedEventArgs e)
    {
        ResetOpacity();
    }

    private void MenuItem_ResetPosition_Click(object sender, RoutedEventArgs e)
    {
        ResetWindowPosition();
    }

    private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
    {
        WpfApp.Current.Shutdown();
    }

    #endregion

    #region Helper Methods

    public void ToggleBorderVisibility()
    {
        if (_currentMode == WindowDisplayMode.Overlay)
        {
            SetDisplayMode(WindowDisplayMode.Setup);
        }
        else
        {
            SetDisplayMode(WindowDisplayMode.Overlay);
        }
    }

    public void ShowSettings()
    {
        ShowSettingsDialog();
    }

    public string GetCurrentChannel()
    {
        return _settings.TwitchChannel;
    }

    public void SetChannel(string channel)
    {
        _settings.TwitchChannel = channel;
        _settings.Save();
        UpdateStatus($"Channel set to: {channel}");
        ChannelNameText.Text = $"#{channel}";
        
        // reconnect to new channel
        ConnectToTwitch(channel);
    }

    public void ResetWindowPosition()
    {
        this.WindowState = WindowState.Normal;
        this.Left = 10;
        this.Top = 10;
        this.Width = 350;
        this.Height = 600;
        
        SetDisplayMode(WindowDisplayMode.Setup);
        
        UpdateStatus("Window position reset");
    }

    private void ShowSettingsDialog()
    {
        // can't interact with dialogs while in overlay mode
        if (_currentMode == WindowDisplayMode.Overlay)
        {
            SetDisplayMode(WindowDisplayMode.Setup);
        }

        var dialog = new SettingsDialog(_settings)
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true)
        {
            _settings = dialog.UpdatedSettings;
            _settings.Save();
            ApplyWindowSettings();
            UpdateStatus("Settings saved");
        }
    }

    private void UpdateStatus(string message)
    {
        if (StatusText != null)
        {
            StatusText.Text = message;
        }
    }

    #endregion
}

public class ChatMessage
{
    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsSystemMessage { get; set; } = false;
}
