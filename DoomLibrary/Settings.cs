using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DoomLibrary.model;

namespace DoomLibrary
{
    public class SettingsObject
    {
        string modsLocation;
        string wadsLocation;
        ObservableCollection<SourcePort> sourcePorts;

        public SettingsObject(string modsLocation, string wadsLocation, ObservableCollection<SourcePort> sourcePorts)
        {
            this.modsLocation = modsLocation;
            this.wadsLocation = wadsLocation;
            this.sourcePorts = sourcePorts;
        }
    }
    
    class Settings
    {
        public static void SaveSettings(SettingsObject settings)
        {

        }

        public static SettingsObject LoadSettings()
        {
            return new SettingsObject("", "", new ObservableCollection<SourcePort>());
        }
    }
}
