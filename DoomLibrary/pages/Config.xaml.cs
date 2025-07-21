using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using wf = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DoomLibrary.model;
using System.Text.Json;

namespace DoomLibrary.pages
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Page
    {
        string modsLocation;
        string wadsLocation;

        private ObservableCollection<SourcePort> sourcePorts;

        public ObservableCollection<SourcePort> SourcePorts
        {
            get { return sourcePorts; }
            set { sourcePorts = value; }
        }
        public Config()
        {
            DataContext = this;
            sourcePorts = new ObservableCollection<SourcePort>();

            InitializeComponent();

            SettingsObject so = Settings.savedSettings != null ? Settings.savedSettings : Settings.NewEmpty();
            modsLocation = so.modsLocation;
            wadsLocation = so.wadsLocation;
            foreach(SourcePort sp in so.sourcePorts)
            {
                sourcePorts.Add(sp);
            }

            UpdateLocations();
            UpdateList();

            btn_modsLocation.Click += SetModsLocation;
            btn_wadsLocation.Click += SetWadsLocation;

            btn_addSourcePort.Click += AddSourcePort;
            btn_saveSettings.Click += SaveSettings;
        }

        private void UpdateLocations()
        {
            if (modsLocation != "") display_modsLocation.Text = modsLocation;
            if (wadsLocation != "") display_wadsLocation.Text = wadsLocation;
        }

        private void UpdateList()
        {
            if (sourcePorts.Count < 1) display_noSourcePort.Visibility = Visibility.Visible;
            else display_noSourcePort.Visibility = Visibility.Hidden;
        }

        private void SetModsLocation(object sender, RoutedEventArgs e)
        {
            wf.FolderBrowserDialog dialog = new wf.FolderBrowserDialog();
            wf.DialogResult result = dialog.ShowDialog();

            if (result == wf.DialogResult.OK)
            {
                modsLocation = dialog.SelectedPath;
                UpdateLocations();
            }
        }

        private void SetWadsLocation(object sender, RoutedEventArgs e)
        {
            wf.FolderBrowserDialog dialog = new wf.FolderBrowserDialog();
            wf.DialogResult result = dialog.ShowDialog();

            if (result == wf.DialogResult.OK)
            {
                wadsLocation = dialog.SelectedPath;
                UpdateLocations();
            }
        }

        private void AddSourcePort(object sender, RoutedEventArgs e)
        {
            sourcePorts.Add(new SourcePort("Not Set"));

            UpdateList();
        }

        private void SetSourcePort(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ((sender as FrameworkElement).DataContext as SourcePort).Index;
            SourcePort selectedObject = null;
            foreach(SourcePort sp in sourcePorts) {
                if (sp.Index == selectedIndex) selectedObject = sp;
            }

            if (selectedObject != null)
            {
                wf.OpenFileDialog dialog = new wf.OpenFileDialog();
                dialog.Title = "Select source port location";
                dialog.Filter = "Executable|*.exe";

                if (dialog.ShowDialog() == wf.DialogResult.OK)
                {
                    int listIndex = sourcePorts.IndexOf(selectedObject);
                    sourcePorts.RemoveAt(listIndex);
                    selectedObject.Path = dialog.FileName;
                    sourcePorts.Insert(listIndex, selectedObject);
                }
            }
        }

        private void RemoveSourcePort(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ((sender as FrameworkElement).DataContext as SourcePort).Index;
            SourcePort selectedObject = null;
            foreach (SourcePort sp in sourcePorts)
            {
                if (sp.Index == selectedIndex) selectedObject = sp;
            }

            if (selectedObject != null)
            {
                int listIndex = sourcePorts.IndexOf(selectedObject);
                sourcePorts.RemoveAt(listIndex);
            }

            UpdateList();
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            Settings.SaveSettings(new SettingsObject(modsLocation, wadsLocation, sourcePorts));
        }
    }
}
