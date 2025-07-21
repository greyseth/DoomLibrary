using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DoomLibrary.model;

namespace DoomLibrary.pages
{
    /// <summary>
    /// Interaction logic for Wads.xaml
    /// </summary>
    public partial class Wads : Page
    {
        public string[] mainWads = {
            "doom",
            "doom2",
            "tnt",
            "plutonia",
            "heretic",
            "hexen",
            "strife1",
            "chex"
        };

        private ObservableCollection<string> allWads;
        public ObservableCollection<string> AllWads
        {
            get { return allWads; }
            set { allWads = value; }
        }

        private ObservableCollection<string> otherWads;
        public ObservableCollection<string> OtherWads
        {
            get { return otherWads; } set { otherWads = value; }
        }

        public Wads()
        {
            DataContext = this;
            allWads = new ObservableCollection<string>();
            otherWads = new ObservableCollection<string>();
            
            InitializeComponent();

            ReadWads();

            btn_refresh.Click += (object sender, RoutedEventArgs e) => ReadWads();
            display_selected.Text = "Selected IWAD: " + (ModsManager.selectedWad == "" ? "None" : ModsManager.selectedWad);

            foreach(string mainWad in mainWads)
            {
                ((Button)container_main.FindName("btn_" + mainWad)).Click += (object sender, RoutedEventArgs e) => SelectWad(mainWad+".wad");
            }
        }

        void ReadWads()
        {
            if (Settings.savedSettings.wadsLocation == "")
            {
                container_yesFolder.Visibility = Visibility.Collapsed;
                container_noFolder.Visibility = Visibility.Visible;
                return;
            }

            var filesEnumerated = Directory.EnumerateFiles(Settings.savedSettings.wadsLocation, "*.*", SearchOption.TopDirectoryOnly);
            var files = filesEnumerated.Where(f => f.EndsWith(".wad")).ToList();

            allWads.Clear();
            foreach (string file in files)
            {
                string[] fileSplit = file.Split("\\");
                string wadName = fileSplit[^1]; // fileSplit.Length - 1

                allWads.Add(wadName);
            }

            foreach(string mainWad in mainWads)
            {
                if (allWads.ToList().FindIndex(w => w.ToLower() == mainWad.ToLower()+".wad") == -1)
                {
                    // Mark IWAD as not found
                    ((Button)container_main.FindName("btn_" + mainWad)).Visibility = Visibility.Collapsed;

                    StackPanel displayContainer = (StackPanel)container_main.FindName("display_" + mainWad);
                    displayContainer.Children.OfType<TextBlock>().FirstOrDefault().Text = "IWAD Not Detected";
                    displayContainer.Children.OfType<Image>().FirstOrDefault().Source = new BitmapImage(new Uri("../img/icon_cross.png", UriKind.Relative));
                }
            }

            otherWads.Clear();
            foreach(string _wad in allWads)
            {
                string wad = Path.GetFileNameWithoutExtension(_wad);

                if (!mainWads.Any(mw => string.Equals(mw, wad, StringComparison.OrdinalIgnoreCase))) otherWads.Add(_wad);
            }
        }

        void SelectWad(string wad)
        {
            ModsManager.selectedWad = wad;
            display_selected.Text = "Selected IWAD: " + wad;
        }

        void OtherWads_Select(object sender, RoutedEventArgs e)
        {
            SelectWad((string)((FrameworkElement)sender).DataContext);
        }
    }
}
