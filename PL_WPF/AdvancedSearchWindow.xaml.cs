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
        private string _chosenOrder;
        private bool _ascendingOrDescending;
        private Order _typeOrder;

        public AdvancedSearchWindow(ListClass listClass)
        {
            InitializeComponent();
            _listClass = listClass;

            _chosenType = _listClass.ChosenType;
            RadioButtonStackPanel.FindChild<RadioButton>(_chosenType).IsChecked = true;

            _chosenOrder = _listClass.ChosenOrder;
            if (_chosenOrder != null)
                OrderStackPanel.Children.OfType<RadioButton>().Where(t => t.Content.ToString() == _chosenOrder).FirstOrDefault().IsChecked = true;

            if (_chosenType == null)
                _chosenType = "Normal";
            else
                CbxEnableTypeFiltering.IsChecked = true;

            if (_chosenOrder != null)
                CbxEnableStatsOrdering.IsChecked = true;

            //Use of anonymous object to split UI and Code! I find it easier to make changes in .cs than in .xaml file!
            var imageType = new
            {
                Normal = "http://veekun.com/dex/media/types/en/normal.png",
                Fighting = "http://veekun.com/dex/media/types/en/fighting.png",
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
            CbxEnableTypeFiltering.IsChecked = true;
            RadioButton rb = (RadioButton)sender;
            _chosenType = rb.Name;
            _listClass.ChosenType = _chosenType;
        }

        private void RadioButtonStats_Click(object sender, RoutedEventArgs e)
        {
            CbxEnableStatsOrdering.IsChecked = true;
            RadioButton rb = (RadioButton)sender;
            _chosenOrder = rb.Content.ToString();
            _typeOrder = Order.Skill;
        }

        private void RadioButtonType_Click(object sender, RoutedEventArgs e)
        {
            CbxEnableStatsOrdering.IsChecked = true;
            _chosenOrder = "Type";
            _typeOrder = Order.Type;
        }

        private void RadioButtonSpecies_Click(object sender, RoutedEventArgs e)
        {
            CbxEnableStatsOrdering.IsChecked = true;
            _chosenOrder = "Species";
            _typeOrder = Order.Species;
        }

        private void RadioButtonAbility_Click(object sender, RoutedEventArgs e)
        {
            CbxEnableStatsOrdering.IsChecked = true;
            _chosenOrder = "Ability";
            _typeOrder = Order.Ability;
        }

        private void RadioButtonAscDesc_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;

            if (rb.Name == "Asc")
                _ascendingOrDescending = false;
            else
                _ascendingOrDescending = true;
        }

        private void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            var source = _listClass.OriginalListPokemons;

            if (CbxEnableTypeFiltering.IsChecked == true)
            {
                //Searched damn long for this query...
                var result = source.Where(p => p.Types.Any(t => t.Name == _chosenType)).ToList();
                source = new ObservableCollection<Pokemon>(result);
            }
            else
                _listClass.ChosenType = null;

            if (CbxEnableStatsOrdering.IsChecked == true)
            {
                List<Pokemon> result;

                if (_typeOrder == Order.Skill)
                {
                    //Get value of specific parameter As example if want to sort on hp, I need to check the hp of every pokemon
                    //I made it that I don't have to change the code when I want to search on an another parameter as example Speed!
                    //Equal functionality a lot of if / else if ... much more code!!!
                    var parameter = typeof (Pokemon).GetProperty(_chosenOrder);
                    result = source.OrderBy(p => parameter.GetValue(p, null)).ToList();
                }

                else if (_typeOrder == Order.Type)
                    result = source.OrderBy(p => p.Types[0].Name).ToList();
                else if (_typeOrder == Order.Species)
                    result = source.OrderBy(p => p.Species[0].ToString()).ToList();
                else
                    result = source.OrderBy(p => p.Abilities[0].Name).ToList();

                if (_ascendingOrDescending)
                    result.Reverse();

                source = new ObservableCollection<Pokemon>(result);

            }
            else
                _chosenOrder = null;

            _listClass.ChosenOrder = _chosenOrder;
            _listClass.ListPokemons = source;
            this.Close();
        }

        enum Order
        {
            Skill, Type, Species, Ability
        }

        private void CbxEnableStatsOrdering_OnClick(object sender, RoutedEventArgs e)
        {
            FirstOrderRadioButton.IsChecked = true;
        }
    }
}
