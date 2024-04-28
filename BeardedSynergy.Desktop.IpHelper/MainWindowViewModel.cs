using System.Net.Http;
using System.Net;
using System.Windows.Media;

namespace BeardedSynergy.Desktop.IpHelper;

public class MainWindowViewModel : ViewModelBase
{
    private const string _urlForIp = "http://icanhazip.com";
    private readonly HttpClient _httpClient;
    private SolidColorBrush _backGroundColor;
    private string _carrierIpAddress;
    private string _currentIpAddress;
    private bool _topMost = true;

    public MainWindowViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public SolidColorBrush BackGroundColor
    {
        get { return _backGroundColor; }
        set
        {
            if (_backGroundColor != value)
            {
                _backGroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string CarrierIpAddress
    {
        get { return _carrierIpAddress; }
        set
        {
            if (_carrierIpAddress != value)
            {
                _carrierIpAddress = value;
                ConfigurationService.ConfigurationModel.CarrierIpAddress = _carrierIpAddress;
                ConfigurationService.WriteConfigFile(ConfigurationService.ConfigurationModel);
                OnPropertyChanged();
            }
        }
    }

    public string CurrentIpAddress
    {
        get { return _currentIpAddress; }
        set
        {
            if (value != _currentIpAddress)
            {
                _currentIpAddress = value;

                OnPropertyChanged();
            }
        }
    }

    public bool TopMost
    {
        get { return _topMost; }
        set
        {
            if (_topMost != value)
            {
                _topMost = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task<IPAddress> GetIpAddress()
    {
        var result = await _httpClient.GetStringAsync($"{_urlForIp}/{DateTime.Now.ToString("yyyy-MM-ddThh-mm-ss")}");
        var trim = result.Replace("\\r\\n", "").Replace("\\n", "").Trim();
        if (!IPAddress.TryParse(trim, out var ipAddress))
        {
            return null;
        }
        return ipAddress;
    }
}