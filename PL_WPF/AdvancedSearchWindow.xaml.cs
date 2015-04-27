using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BS_PokedexManager;
using DAL_JSON;
using Type = DAL_JSON.Type;

namespace PL_WPF
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class AdvancedSearchWindow
    {
        public ObservableCollection<Pokemon> ListPokemons;
        private string _chosenType;

        public AdvancedSearchWindow(ObservableCollection<Pokemon> listPokemons)
        {
            InitializeComponent();
            ListPokemons = listPokemons;

            var Test = new
            {
                Normal = "http://veekun.com/dex/media/types/en/normal.png",
                Fighting= "http://veekun.com/dex/media/types/en/fighting.png",
                Flying = "http://veekun.com/dex/media/types/en/flying.png",
                Poison = "http://veekun.com/dex/media/types/en/poison.png",
                Ground = "http://veekun.com/dex/media/types/en/ground.png",
                Rock = "http://veekun.com/dex/media/types/en/rock.png",
                Bug = "http://veekun.com/dex/media/types/en/bug.png",
                Ghost = "http://veekun.com/dex/media/types/en/ghost.png",
                Steel = "http://veekun.com/dex/media/types/en/steel.png",
                Fire = "http://veekun.com/dex/media/types/en/fire.png",
                Water = "http://veekun.com/dex/media/types/en/water.png",
                Grass = "http://veekun.com/dex/media/types/en/grass.png",
                Electric = "http://veekun.com/dex/media/types/en/electric.png",
                Psychic = "http://veekun.com/dex/media/types/en/psychic.png",
                Ice = "http://veekun.com/dex/media/types/en/ice.png",
                Dragon = "http://veekun.com/dex/media/types/en/dragon.png",
                Dark = "http://veekun.com/dex/media/types/en/dark.png",
                Fairy = "http://veekun.com/dex/media/types/en/fairy.png",
                Unknown = "http://veekun.com/dex/media/types/en/unknown.png"
            };

            RadioButtonStackPanel.DataContext = Test;

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            _chosenType = rb.Name;
            //var result = ListPokemons.Select(m => m.Types.Where(t => t.Name == _chosenType));
        }

        private void AdvancedSearchWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Type temp = new Type() { Name = _chosenType };
            var result = ListPokemons.Select(m => m.Types.Where(t => t.Name == _chosenType));
            //ListPokemons = result;
        }
    }
}
