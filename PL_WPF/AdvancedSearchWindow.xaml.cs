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
using MahApps.Metro.Controls;
using Type = DAL_JSON.Type;

namespace PL_WPF
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class AdvancedSearchWindow
    {
        private ListClass _listClass;
        private string _chosenType;
        private string _chosenStat;
        private bool _ascendingOrDescending;

        public AdvancedSearchWindow(ListClass listClass)
        {
            InitializeComponent();
            _listClass = listClass;

            _chosenType = _listClass.ChosenType;
            RadioButtonStackPanel.FindChild<RadioButton>(_chosenType).IsChecked = true;

            if (_chosenType == null)
                _chosenType = "Normal";
            else
                CbxEnableTypeFiltering.IsChecked = true;

            //Use of anonymous object to split UI and Code! I find it easier to make changes in .cs than in .xaml file!
            var imageType = new
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

            RadioButtonStackPanel.DataContext = imageType;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            _chosenType = rb.Name;
            _listClass.ChosenType = _chosenType;
        }

        private void RadioButtonStats_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            _chosenStat = rb.Content.ToString();
        }

        private void RadioButtonAscDesc_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton) sender;

            if (rb.Name == "Asc")
                _ascendingOrDescending = false;
            else
                _ascendingOrDescending = true;
        }

        private void AdvancedSearchWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (CbxEnableTypeFiltering.IsChecked == true)
            {
                //Searched damn long for this query...
                var result =
                    _listClass.OriginalListPokemons.Where(p => p.Types.Any(t => t.Name == _chosenType)).ToList();
                _listClass.ListPokemons = new ObservableCollection<Pokemon>(result);
            }
            else if (CbxEnableStatsOrdering.IsChecked == true)
            {
                var parameter = typeof (Pokemon).GetProperty(_chosenStat);
                var result = _listClass.OriginalListPokemons.OrderBy(p => parameter.GetValue(p, null)).ToList();

                if (_ascendingOrDescending)
                    result.Reverse();

                _listClass.ListPokemons = new ObservableCollection<Pokemon>(result);
            }

            else
            {
                _listClass.ListPokemons = _listClass.OriginalListPokemons;
            }
        }
    }
}
