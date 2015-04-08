using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_JSON;
using Newtonsoft.Json;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BS_PokedexManager
{
    public class Business
    {
        private static WebClient client = new WebClient();
        private static int _maxPokemonGen;
        private static int _gen;
        private static string _stringGen;

        public enum Generation
        {
            I = 3,      //151
            II = 6,     //251
            III = 386,
            IV = 493,
            V = 649,
        }

        public static DescriptionProgressBar DescriptionProgress = new DescriptionProgressBar();

        public static ObservableCollection<JsonParse.Pokemon> GeneratePokeList(Generation g, BackgroundWorker b)
        {
            //TODO: Save latest generated list! Then add application variable to remember wich generation this was.
            //TODO: Then parse pokemons from that list (much much much faster!)

            _maxPokemonGen = Convert.ToInt32(g);
            _stringGen = g.ToString();

            switch (g)
            {
                case Generation.I:
                    _gen = 1;
                    break;
                case Generation.II:
                    _gen = 2;
                    break;
                case Generation.III:
                    _gen = 3;
                    break;
                case Generation.IV:
                    _gen = 4;
                    break;
                case Generation.V:
                    _gen = 5;
                    break;
            }

            //Get all pokemons from main JSON file. This file contains every pokemon but doesn't contain generation dependant information (moves, ...)
            var list = DAL_JSON.JsonParse.GetPokemons();

            //Add / Remove the generation dependant information 
            RemovePokemons(list);
            ParseDescriptions(list, b);
            ParseEvolutions(list, b);
            ParseMoves(list, b);
            ParseMachines(list, b);

            ObservableCollection<JsonParse.Pokemon> pokeObservable = new ObservableCollection<JsonParse.Pokemon>(list);
            return pokeObservable;
        }

        private static void RemovePokemons(List<JsonParse.Pokemon> pokemons)
        {
            pokemons.RemoveRange(_maxPokemonGen, pokemons.Count - _maxPokemonGen);
        }

        private static void ParseDescriptions(List<JsonParse.Pokemon> pokemons, BackgroundWorker b)
        {
            DescriptionProgress.Description = "Adding the correct description to the pokemons";
            foreach (DAL_JSON.JsonParse.Pokemon p in pokemons)
            {
                for (int i = 0; i < p.Descriptions.Count; i++)
                {
                    if (p.Descriptions[i].Name.Contains(_gen.ToString()))
                    {
                        string _jsonString = client.DownloadString(p.Descriptions[i].Resource_uri);
                        p.Descriptions[i] = JsonConvert.DeserializeObject<DAL_JSON.JsonParse.Description>(_jsonString);
                        p.Description = p.Descriptions[i].DescriptionText;
                        b.ReportProgress(Convert.ToInt32(p.Pkdx_id / (_maxPokemonGen / 33.3333)));
                        break;
                    }
                }
            }
        }

        private static void ParseEvolutions(List<JsonParse.Pokemon> pokemons, BackgroundWorker b)
        {
            foreach (JsonParse.Pokemon p in pokemons)
            {
                for (int i = p.Evolutions.Count - 1; i >= 0; i--)
                {
                    /*
                     * Remove evolution if it's above the max pokemon of the needed generation!
                     * As Example --> Porygon gets an evolution in GEN II to Porygon2
                     * But In GEN I Porygon2 doesn't exist!
                     * 
                    */

                    if (Convert.ToInt32((p.Evolutions[i].Resource_uri.Split('/')[6])) > _maxPokemonGen)
                    {
                        p.Evolutions.RemoveAt(i);
                    }

                    //If not check we check if the evolutions has evolutions!
                    //As example Bulbasaur has an evolution Ivysaur. But Ivysaur also has an evolution to Venusaur
                    else
                    {
                        int pokemonid = Convert.ToInt32((p.Evolutions[i].Resource_uri.Split('/')[6]));      //Example: Id of Ivysaur

                        //If the evolution is above 151 we shouldn't add it                           
                        if (pokemonid <= _maxPokemonGen && pokemons[pokemonid - 1].Evolutions.Count > 0)
                        {
                            //Add the evolution of the evolution to the list. Only when that evolution of the evolution is posible in the current generation
                            //As example we add Venusaur here!
                            if (Convert.ToInt32((pokemons[pokemonid - 1].Evolutions[0].Resource_uri.Split('/')[6])) <= _maxPokemonGen)
                                p.Evolutions.AddRange(pokemons[pokemonid - 1].Evolutions);
                        }
                    }
                }

                //Add pictures uri to all evolutions!
                foreach (JsonParse.Evolution e in p.Evolutions)
                {
                    e.ImageURL = pokemons[Convert.ToInt32((e.Resource_uri.Split('/')[6])) - 1].ImageURL;
                }

            }
        }

        private static void ParseMoves(List<JsonParse.Pokemon> pokemons, BackgroundWorker b)
        {
            DescriptionProgress.Description = "Adding the correct moves to the pokemons (Webscraping)";
            
            foreach (var v in pokemons)
            {
                v.MovesWeb = WebScraper.ScrapeLevelMoves(v.Name, _stringGen);
                b.ReportProgress(33 + (Convert.ToInt32(v.Pkdx_id / (_maxPokemonGen / 33.3333))));
            }
        }

        private static void ParseMachines(List<JsonParse.Pokemon> pokemons, BackgroundWorker b)
        {
            DescriptionProgress.Description = "Adding the correct machines (TM/HM) to the pokemons (Webscraping)";
            
            foreach (var v in pokemons)
            {
                v.MachinesWeb = WebScraper.ScrapeMachineMoves(v.Name, _stringGen);
                b.ReportProgress(66 + (Convert.ToInt32(v.Pkdx_id / (_maxPokemonGen / 33.3333))));
            }
        }
    }

    public class DescriptionProgressBar : INotifyPropertyChanged
    {
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NoticeMe(Description);
            }
        }

        public void NoticeMe(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
