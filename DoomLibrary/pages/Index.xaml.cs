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

        public Index()
        {
            DataContext = this;
            mods = new ObservableCollection<Mod>();

            InitializeComponent();

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
        }

        void ReadModsDir()
        {
            if (Settings.savedSettings.modsLocation == "")
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

            Trace.WriteLine(System.Text.Json.JsonSerializer.Serialize(ModsManager.allMods));
        }

        void ToggleHidden(object sender, RoutedEventArgs e)
        {
            Mod mod = (sender as FrameworkElement).DataContext as Mod;
            Mod foundMod = ModsManager.allMods[ModsManager.allMods.FindIndex(m => m.name == mod.name)];
            foundMod.Hidden = !foundMod.Hidden;
            mod.Hidden = foundMod.Hidden;

            if (!showHidden && mod.Hidden == true)
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
    }
}
