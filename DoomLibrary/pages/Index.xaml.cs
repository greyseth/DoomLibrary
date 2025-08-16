using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using System.Diagnostics;
using DoomLibrary.model;
using DoomLibrary.commands;
using System.ComponentModel;

namespace DoomLibrary.pages
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        private ObservableCollection<Mod> mods; // list of mods to be rendered

        public ObservableCollection<Mod> Mods
        {
            get { return mods; }
            set { mods = value; }
        }

        bool showHidden = false;

        public ObservableCollection<SourcePort> SourcePorts
        {
            get { return Settings.savedSettings != null ? Settings.savedSettings.sourcePorts : new ObservableCollection<SourcePort>(); }
            set { Settings.savedSettings.sourcePorts = value; }
        }

        private SourcePort selectedSourcePort;
        public SourcePort SelectedSourcePort
        {
            get { return selectedSourcePort; }
            set { selectedSourcePort = value; }
        }

        private ObservableCollection<Mod> appliedMods;
        public ObservableCollection<Mod> AppliedMods
        {
            get { return appliedMods; }
            set { appliedMods = value; }
        }

        public string GetSelectedWad
        {
            get { return ModsManager.selectedWad != "" ? ModsManager.selectedWad : "Not Set"; }
        }

        public Index()
        {
            DataContext = this;
            mods = new ObservableCollection<Mod>();
            appliedMods = new ObservableCollection<Mod>();
            selectedSourcePort = new SourcePort();

            InitializeComponent();

            // Source ports dropdown
            if (Settings.savedSettings != null && Settings.savedSettings.sourcePorts.Count > 0)
            {
                selectedSourcePort = Settings.savedSettings.sourcePorts.FirstOrDefault();
                input_sourcePorts.SelectedItem = selectedSourcePort;
            }
            else
            {
                input_sourcePorts.Visibility = Visibility.Collapsed;
                display_sourcePortUnavailable.Visibility = Visibility.Visible;
            }

            // IWAD display
            display_iwad.Click += (object sender, RoutedEventArgs e) => (Window.GetWindow(this) as MainWindow).AppContent.Navigate(new Uri("/pages/wads.xaml", UriKind.Relative));
            //display_iwad.Content = ModsManager.selectedWad != "" ? ModsManager.selectedWad : "Not Set";

            ReadModsDir();

            input_search.KeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.Key == Key.Enter) SearchMods(null, null);
            };
            btn_search.Click += SearchMods;

            btn_refreshList.Click += (object sender, RoutedEventArgs e) => ReadModsDir();
            
            btn_showHidden.Click += (object sender, RoutedEventArgs e) =>
            {
                showHidden = !showHidden;
                if (showHidden) btn_showHidden.Content = "Hide Hidden";
                else btn_showHidden.Content = "Show Hidden";

                ReadModsDir();
            };

            btn_unapplyAll.Click += (object sender, RoutedEventArgs e) =>
            {
                foreach (Mod mod in ModsManager.allMods) mod.LoadOrder = 0;
                foreach (Mod mod in mods) mod.LoadOrder = 0;
                ModsManager.lastLoadOrder = 0;
            };

            btn_modifyOrder.Click += (object sender, RoutedEventArgs e) =>
            {
                appliedMods.Clear();
                foreach(Mod appliedMod in mods.ToList().Where<Mod>(m => m.LoadOrder > 0).OrderBy((m) => m.LoadOrder))
                {
                    appliedMods.Add(appliedMod);
                }

                appliedMods.ToList().Sort((a, b) => a.LoadOrder.CompareTo(b.LoadOrder));

                container_loadOrder.Visibility = Visibility.Visible;

                if (appliedMods.Count < 1)
                {
                    container_appliedMods.Visibility = Visibility.Collapsed;
                    display_noAppliedMods.Visibility = Visibility.Visible;
                }else
                {
                    container_appliedMods.Visibility = Visibility.Visible;
                    display_noAppliedMods.Visibility = Visibility.Collapsed;
                }
            };
            btn_launch.Click += LaunchGame;

            btn_closeLoadOrder.Click += (object sender, RoutedEventArgs e) => container_loadOrder.Visibility = Visibility.Collapsed;
        }

        void ReadModsDir()
        {
            if (Settings.savedSettings == null || Settings.savedSettings.modsLocation == "")
            {
                container_noFolder.Visibility = Visibility.Visible;
                container_yesFolder.Visibility = Visibility.Collapsed;
                return;
            }

            var filesEnumerated = Directory.EnumerateFiles(Settings.savedSettings.modsLocation, "*.*", SearchOption.AllDirectories);
            var files = filesEnumerated.Where(f => f.EndsWith(".wad") || f.EndsWith(".pk3")).ToList();

            display_noMods.Visibility = Visibility.Collapsed;
            container_listMods.Visibility = Visibility.Collapsed;
            
            if (mods.Count > 0) mods.Clear();
            foreach (string file in files)
            {
                string[] fileSplit = file.Split("\\");
                string modName = fileSplit[^1]; // fileSplit.Length - 1

                Mod newMod = new Mod(modName, ModsManager.GetLoadOrder(modName), ModsManager.IsHidden(modName));
                if (ModsManager.allMods.FindIndex(m => m.name == modName) == -1) ModsManager.allMods.Add(newMod);
                if (!showHidden)
                {
                    if (!ModsManager.IsHidden(modName)) mods.Add(newMod);
                }
                else mods.Add(newMod);
            }

            UpdateModsCount();

            if (input_search.Text != "") SearchMods(null, null);
        }

        void ToggleMod(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            Mod mod = (sender as FrameworkElement).DataContext as Mod;

            if (checkbox.IsChecked == true) ModsManager.ApplyMod(mod.name);
            else ModsManager.RemoveMod(mod.name);

            //Trace.WriteLine(System.Text.Json.JsonSerializer.Serialize(ModsManager.allMods));

            //Trace.WriteLine(AppliedMods.Count);
        }

        void ToggleHidden(object sender, RoutedEventArgs e)
        {
            Mod mod = (sender as FrameworkElement).DataContext as Mod;
            Mod foundMod = ModsManager.allMods[ModsManager.allMods.FindIndex(m => m.name == mod.name)];
            foundMod.Hidden = !foundMod.Hidden;
            mod.Hidden = foundMod.Hidden;

            if (!showHidden && mod.Hidden)
            {
                mods.Remove(mod);
                UpdateModsCount();
            }
        }

        void SearchMods(object sender, RoutedEventArgs e)
        {
            string query = input_search.Text;

            container_listMods.Visibility = Visibility.Visible;
            display_noMods.Visibility = Visibility.Collapsed;
            if (query == "") ReadModsDir();
            else
            {
                ObservableCollection<Mod> filteredMods = new ObservableCollection<Mod>(ModsManager.allMods.Where(m => m.name.ToLower().Contains(query.ToLower()) && (showHidden || !m.Hidden)));
                mods.Clear();
                foreach(Mod mod in filteredMods)
                {
                    mods.Add(mod);
                }

                if (mods.Count < 1)
                {
                    container_listMods.Visibility = Visibility.Collapsed;
                    display_noMods.Visibility = Visibility.Visible;
                    display_noMods.Text = "Could not find results for '" + query + "'";
                }
            }
        }

        void UpdateModsCount()
        {
            if (mods.Count > 0)
            {
                container_listMods.Visibility = Visibility.Visible;
                display_modsLocation.Text = "Reading from folder " + Settings.savedSettings.modsLocation;
            }
            else
            {
                display_noMods.Visibility = Visibility.Visible;
                display_noMods.Text = "No Mods Found on " + Settings.savedSettings.modsLocation;
            }
        }

        void LaunchGame(object sender, RoutedEventArgs e)
        {
            if (Settings.savedSettings == null || Settings.savedSettings.sourcePorts.Count < 1)
            {
                MessageBox.Show("You must select a source port"); 
                return;
            }

            if (ModsManager.selectedWad == "")
            {
                MessageBox.Show("You must select a valid IWAD file");
                return;
            }

            ModsManager.SaveMods();
        }

        void MoveLoadOrderDown(object sender, RoutedEventArgs e)
        {   
            Mod selectedMod = (sender as FrameworkElement).DataContext as Mod;
            int selectedModIndex = appliedMods.ToList().FindIndex(m => m == selectedMod);

            if (selectedModIndex == appliedMods.Count - 1) return;

            int prevLoadOrder = appliedMods[selectedModIndex].LoadOrder;
            int nextLoadOrder = appliedMods[selectedModIndex + 1].LoadOrder;

            appliedMods[selectedModIndex].LoadOrder = nextLoadOrder;
            appliedMods[selectedModIndex + 1].LoadOrder = prevLoadOrder;

            ModsManager.allMods[ModsManager.allMods.ToList().FindIndex(m => m == appliedMods[selectedModIndex])].LoadOrder = nextLoadOrder;
            ModsManager.allMods[ModsManager.allMods.ToList().FindIndex(m => m == appliedMods[selectedModIndex + 1])].LoadOrder = prevLoadOrder;

            MoveLoadOrderUpdate();
        }

        void MoveLoadOrderUp(object sender, RoutedEventArgs e)
        {
            Mod selectedMod = (sender as FrameworkElement).DataContext as Mod;
            int selectedModIndex = appliedMods.ToList().FindIndex(m => m == selectedMod);

            if (selectedModIndex == 0) return;

            int prevLoadOrder = appliedMods[selectedModIndex - 1].LoadOrder;
            int curLoadOrder = appliedMods[selectedModIndex].LoadOrder;

            appliedMods[selectedModIndex - 1].LoadOrder = curLoadOrder;
            appliedMods[selectedModIndex].LoadOrder = prevLoadOrder;

            ModsManager.allMods[ModsManager.allMods.ToList().FindIndex(m => m == appliedMods[selectedModIndex - 1])].LoadOrder = curLoadOrder;
            ModsManager.allMods[ModsManager.allMods.ToList().FindIndex(m => m == appliedMods[selectedModIndex])].LoadOrder = prevLoadOrder;

            MoveLoadOrderUpdate();
        }

        void MoveLoadOrderUpdate()
        {
            // Might be kinda slow, idk
            var sorted = appliedMods.OrderBy(m => m.LoadOrder).ToList();

            appliedMods.Clear();
            foreach (var mod in sorted)
                appliedMods.Add(mod);

            list_modLoadOrder.Items.Refresh();
        }
    }
}
