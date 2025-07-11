using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoomLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Settings.LoadSettings();
        }

        private void ToIndex(object sender, RoutedEventArgs e)
        {
            AppContent.Navigate(new Uri("/pages/index.xaml", UriKind.Relative));
        }

        private void ToConfig(object sender, RoutedEventArgs e)
        {
            AppContent.Navigate(new Uri("/pages/config.xaml", UriKind.Relative));
        }
    }
}
