using System;
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;

namespace TwitchChatOverlay;

public partial class SettingsDialog : Window
{
    public AppSettings UpdatedSettings { get; private set; }

    public SettingsDialog(AppSettings currentSettings)
    {
        InitializeComponent();
        
        UpdatedSettings = new AppSettings
        {
            TwitchChannel = currentSettings.TwitchChannel,
            ZoomLevel = currentSettings.ZoomLevel,
            OpacityLevel = currentSettings.OpacityLevel,
            AutoHideBorders = currentSettings.AutoHideBorders,
            HideFromOBS = currentSettings.HideFromOBS,
            HideTaskbarIcon = currentSettings.HideTaskbarIcon,
            WindowLeft = currentSettings.WindowLeft,
            WindowTop = currentSettings.WindowTop,
            WindowWidth = currentSettings.WindowWidth,
            WindowHeight = currentSettings.WindowHeight
        };
        
        LoadSettings();
    }

    private void LoadSettings()
    {
        TxtChannelName.Text = UpdatedSettings.TwitchChannel;
        SliderOpacity.Value = UpdatedSettings.OpacityLevel;
        SliderZoom.Value = UpdatedSettings.ZoomLevel;
        ChkAutoHideBorders.IsChecked = UpdatedSettings.AutoHideBorders;
        ChkHideFromOBS.IsChecked = UpdatedSettings.HideFromOBS;
        ChkHideTaskbarIcon.IsChecked = UpdatedSettings.HideTaskbarIcon;
        
        UpdateOpacityDisplay();
        UpdateZoomDisplay();
    }

    private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateOpacityDisplay();
    }

    private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateZoomDisplay();
    }

    private void UpdateOpacityDisplay()
    {
        if (TxtOpacityValue != null && SliderOpacity != null)
        {
            int percentage = (int)(SliderOpacity.Value / 255.0 * 100);
            TxtOpacityValue.Text = $"{percentage}%";
        }
    }

    private void UpdateZoomDisplay()
    {
        if (TxtZoomValue != null && SliderZoom != null)
        {
            int percentage = (int)(SliderZoom.Value * 100);
            TxtZoomValue.Text = $"{percentage}%";
        }
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        string channelName = TxtChannelName.Text.Trim();
        if (string.IsNullOrWhiteSpace(channelName))
        {
            WpfMessageBox.Show("Please enter a Twitch channel name.", "Validation Error", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        UpdatedSettings.TwitchChannel = channelName;
        UpdatedSettings.OpacityLevel = (byte)SliderOpacity.Value;
        UpdatedSettings.ZoomLevel = SliderZoom.Value;
        UpdatedSettings.AutoHideBorders = ChkAutoHideBorders.IsChecked ?? false;
        UpdatedSettings.HideFromOBS = ChkHideFromOBS.IsChecked ?? true;
        UpdatedSettings.HideTaskbarIcon = ChkHideTaskbarIcon.IsChecked ?? true;
        
        this.DialogResult = true;
        this.Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
