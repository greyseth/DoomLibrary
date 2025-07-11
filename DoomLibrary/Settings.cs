using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DoomLibrary.model;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DoomLibrary
{
    public class SettingsObject
    {
        public string modsLocation { get; set; }
        public string wadsLocation { get; set; }
        public ObservableCollection<SourcePort> sourcePorts { get; set; }

        public SettingsObject() { }

        public SettingsObject(string modsLocation, string wadsLocation, ObservableCollection<SourcePort> sourcePorts)
        {
            this.modsLocation = modsLocation;
            this.wadsLocation = wadsLocation;
            this.sourcePorts = sourcePorts;
        }
    }
    
    class Settings
    {
        public static SettingsObject savedSettings;
        
        public static void SaveSettings(SettingsObject settings)
        {
            string serialized = JsonSerializer.Serialize(settings);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\config.json", serialized);

            MessageBox.Show("Saved Settings Data");
        }

        public static SettingsObject LoadSettings()
        {
            SettingsObject deserialized = new SettingsObject("", "", new ObservableCollection<SourcePort>());
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory+"\\config.json"))
            {
                string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\config.json");
                deserialized = JsonSerializer.Deserialize<SettingsObject>(json);
            }

            savedSettings = deserialized;
            return deserialized;
        }
    }
}
