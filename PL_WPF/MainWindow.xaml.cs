using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using BS_PokedexManager;
using DAL_JSON;

namespace PL_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static ObservableCollection<Pokemon> _listPokemons;
        static public ObservableCollection<Pokemon> ListPokemons {
            get
            {
                return _listPokemons;
            }
            set
            {
                _listPokemons = value;
                NoticeMe("ListPokemons");
            }
        }

        
        static public void NoticeMe(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(p));
            }
        }

        static public event EventHandler<PropertyChangedEventArgs> PropertyChanged;


        public MainWindow(ObservableCollection<Pokemon> listPokemons )
        {
            InitializeComponent();
            ListPokemons = listPokemons;
            PokemonListBox.ItemsSource = ListPokemons;
            GenerationTextBlock.Text = Business.StringGen;
            StatsGrid.DataContext = Business.MaxStatsValue;
        }

        private void PokemonListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PokemonListBox.SelectedIndex >= 0)
            {
                var selectedPokemon = PokemonListBox.SelectedItem as Pokemon;
                MainGrid.DataContext = selectedPokemon;
                TypesListBox.ItemsSource = selectedPokemon.Types;
                MoveListBox.ItemsSource = selectedPokemon.MovesWeb;
                MachineListBox.ItemsSource = selectedPokemon.MachinesWeb;
                EvolutionsListBox.ItemsSource = selectedPokemon.Evolutions;
                AbilityListBox.ItemsSource = selectedPokemon.Abilities;
            }
        }

        private void MoveListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListBox;
            if (list.SelectedIndex >= 0)
            {               
                var selectedMove = list.SelectedItem as Move;
                var registerwindow = new Moves(selectedMove);
                registerwindow.Show();
            }
        }

        private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
        {
            var v = new SettingWindow();
            v.Show();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            var result = _listPokemons.Where(v => v.Name.ToLower().Contains(SearchNameTextBox.Text.ToLower()));
            PokemonListBox.ItemsSource = result;
        }

        private void BtnAdvancedSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var v = new AdvancedSearchWindow(_listPokemons);
            v.Show();
        }
    }



    public class Convertor : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double temp = (double)value;

            if (temp < 6)
                return "Red";

            if (temp < 16)
                return "Orange";

            return "Green";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
