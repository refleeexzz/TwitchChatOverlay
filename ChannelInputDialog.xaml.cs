using System.Windows;

namespace TwitchChatOverlay;

public partial class ChannelInputDialog : Window
{
    public string ChannelName { get; private set; } = string.Empty;

    public ChannelInputDialog(string currentChannel = "")
    {
        InitializeComponent();
        TxtChannel.Text = currentChannel;
        TxtChannel.SelectAll();
        TxtChannel.Focus();
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        ChannelName = TxtChannel.Text.Trim();
        if (!string.IsNullOrWhiteSpace(ChannelName))
        {
            this.DialogResult = true;
            this.Close();
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
