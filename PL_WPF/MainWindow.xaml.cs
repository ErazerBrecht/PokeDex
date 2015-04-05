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
using DAL_JSON;

namespace PL_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var ListPokemons = DAL_JSON.JsonParse.GetPokemons();
            PokemonListBox.ItemsSource = ListPokemons;
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
            if (MoveListBox.SelectedIndex >= 0)
            {
                var list = sender as ListBox;
                var selectedMove = list.SelectedItem as JsonParse.Move;
                var registerwindow = new Moves(selectedMove);
                registerwindow.Show();
            }
        }
    }
}
