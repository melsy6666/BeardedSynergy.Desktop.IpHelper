using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeardedSynergy.Desktop.IpHelper;

public static class ConfigurationService
{
    private static readonly string _fullPath;
    private static string _configFileName = "configuration.json";
    private static string _filePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\BeardedSynergy\\BeardedSynergy.Desktop.Helper\\";
    public static ConfigurationModel ConfigurationModel;

    static ConfigurationService()
    {
        _fullPath = Path.Join(_filePath, _configFileName);
    }

    public static ConfigurationModel GetConfigurationFromDisk()
    {
        if (Directory.Exists(_filePath) == false)
        {
            Directory.CreateDirectory(_filePath);
        }
        if (File.Exists(_fullPath) == false)
        {
            WriteConfigFile(new ConfigurationModel(""));
        }
        var config = File.ReadAllText(_fullPath);
        ConfigurationModel = JsonConvert.DeserializeObject<ConfigurationModel>(config);
        if (ConfigurationModel == null)
        {
            ConfigurationModel = new("");
            return null;
        }
        return ConfigurationModel;
    }

    public static void WriteConfigFile(ConfigurationModel config)
    {
        var ser = JsonConvert.SerializeObject(config);
        File.WriteAllText(_fullPath, ser);
    }
}

public class ConfigurationModel
{
    public ConfigurationModel(string carrierIp)
    {
        CarrierIpAddress = carrierIp;
    }

    public bool AlwaysOnTop { get; set; }
    public string CarrierIpAddress { get; set; }
    public bool HideOnNonCarrierIp { get; set; }
}