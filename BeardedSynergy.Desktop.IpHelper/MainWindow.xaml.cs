﻿using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace BeardedSynergy.Desktop.IpHelper;

public partial class MainWindow : Window
{
    #region Colors

    // If the IP matchs the given public IP, the background will be _red, else _green.
    private readonly SolidColorBrush _green = (SolidColorBrush)new BrushConverter().ConvertFrom("#88036E31");

    private readonly SolidColorBrush _red = (SolidColorBrush)new BrushConverter().ConvertFrom("#88FF0000");

    #endregion Colors

    // Seconds to check IP Address
    private const int _checkInterval = 10;

    private readonly ConfigurationModel _configurationModel;
    private readonly HttpClient _httpClient;
    private DateTime _checkDateTime;

    #region IP Address tacking

    private IPAddress _currentIpAddress;
    private IPAddress _previousIpAddress;

    #endregion IP Address tacking

    #region Move Window dll calls

    private const int HT_CAPTION = 0x2;
    private const int WM_NCLBUTTONDOWN = 0xA1;

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    #endregion Move Window dll calls

    public MainWindow()
    {
        // Create the http handler with a connection lifetime of 10 seconds, so that the connection is short, to get more accurate IP reading.
        var handler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromSeconds(10)
        };
        _httpClient = new HttpClient(handler);

        InitializeComponent();
        MainWindowViewModel = new(_httpClient);
        this.DataContext = this;
        _configurationModel = ConfigurationService.GetConfigurationFromDisk();
        MainWindowViewModel.CarrierIpAddress = _configurationModel.CarrierIpAddress;
        _checkDateTime = DateTime.Now;
        Task.Run(async () => await ExecuteMainLoop());
    }

    public MainWindowViewModel MainWindowViewModel { get; set; }

    private void ClickClose(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    public async Task ExecuteMainLoop()
    {
        while (true)
        {
            try
            {
                if (DateTime.Now > _checkDateTime)
                {
                    _currentIpAddress = await MainWindowViewModel.GetIpAddress();
                    if (_previousIpAddress == _currentIpAddress)
                    {
                        continue;
                    }
                    MainWindowViewModel.CurrentIpAddress = _currentIpAddress.ToString();
                    if (MainWindowViewModel.CurrentIpAddress == ConfigurationService.ConfigurationModel.CarrierIpAddress)
                    {
                        MainWindowViewModel.BackGroundColor = _red;
                    }
                    else
                    {
                        MainWindowViewModel.BackGroundColor = _green;
                    }
                    this.Dispatcher.Invoke(() => MainWindowViewModel.BackGroundColor.Freeze());
                    _checkDateTime = DateTime.Now.AddSeconds(_checkInterval);
                    _previousIpAddress = _currentIpAddress;
                }
            }
            catch (Exception ex)
            {
                var t = ex;
            }
        }
    }

    public void MoveWindow(object sender, MouseButtonEventArgs e)
    {
        ReleaseCapture();
        SendMessage(new WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
    }
}