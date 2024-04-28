using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace BeardedSynergy.Desktop.IpHelper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private NotifyIcon _icon;
    public MainWindowViewModel MainWindowViewModel { get; set; }

    private void ExitClicked(object sender, EventArgs e)
    {
        Environment.Exit(1);
    }

    private void Icon_Click(object sender, EventArgs e)
    {
        _icon.ContextMenuStrip = new ContextMenuStrip()
        {
        };
        _icon.ContextMenuStrip.Items.Add("Exit");
        _icon.ContextMenuStrip.Items[0].Click += ExitClicked;
        _icon.ContextMenuStrip.Items.Add("Make Visible");
        _icon.ContextMenuStrip.Items[1].Click += MakeVisibleClick;

        //throw new NotImplementedException();
    }

    private void MakeVisibleClick(object sender, EventArgs e)
    {
        ((MainWindow)System.Windows.Application.Current.MainWindow).MainWindowViewModel.HideOnNonCarrierIp = false;
        ((MainWindow)System.Windows.Application.Current.MainWindow).MainWindowViewModel.MainWindowVisibility= Visibility.Visible;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _icon = new()
        {
            Icon = ProjectResources.icon_SysTray,
            Visible = true,
            Text = "BeardedSynergy.Desktop.IpHandler"
        };
        _icon.Click += Icon_Click;
    }
}