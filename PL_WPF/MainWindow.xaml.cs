using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public MainWindow(ObservableCollection<JsonParse.Pokemon> ListPokemons )
        {
            InitializeComponent();
            PokemonListBox.ItemsSource = ListPokemons;
            GenerationTextBlock.Text = Business.StringGen;
        }

        private void PokemonListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PokemonListBox.SelectedIndex >= 0)
            {
                var selectedPokemon = PokemonListBox.SelectedItem as JsonParse.Pokemon;
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
                var selectedMove = list.SelectedItem as JsonParse.Move;
                var registerwindow = new Moves(selectedMove);
                registerwindow.Show();
            }
        }

        private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
        {
            var v = new SettingWindow();
            v.Show();

        }
    }
}
