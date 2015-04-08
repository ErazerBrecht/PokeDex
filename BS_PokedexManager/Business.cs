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
        private static int GEN = 151;

        public enum Generation
        {
            I = 151,
            II = 251,
            III = 386,
            IV = 493,
            V = 649,
        }

        public static ObservableCollection<JsonParse.Pokemon> GeneratePokeList(BackgroundWorker b)
        {
            var list = DAL_JSON.JsonParse.GetPokemons();
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
            pokemons.RemoveRange(GEN, pokemons.Count - GEN);
        }

        private static void ParseDescriptions(List<JsonParse.Pokemon> pokemons, BackgroundWorker b)
        {
            Console.WriteLine("\nStarting to add description from GEN 2 to Pokemon as string");
            foreach (DAL_JSON.JsonParse.Pokemon p in pokemons)
            {
                for (int i = 0; i < p.Descriptions.Count; i++)
                {
                    if (p.Descriptions[i].Name.Contains("1"))
                    {
                        string _jsonString = client.DownloadString(p.Descriptions[i].Resource_uri);
                        p.Descriptions[i] = JsonConvert.DeserializeObject<DAL_JSON.JsonParse.Description>(_jsonString);
                        p.Description = p.Descriptions[i].DescriptionText;
                        b.ReportProgress(Convert.ToInt32(p.Pkdx_id / (GEN / 33.3)));
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

                    if (Convert.ToInt32((p.Evolutions[i].Resource_uri.Split('/')[6])) > GEN)
                    {
                        p.Evolutions.RemoveAt(i);
                    }

                    //If not check we check if the evolutions has evolutions!
                    //As example Bulbasaur has an evolution Ivysaur. But Ivysaur also has an evolution to Venusaur
                    else
                    {
                        int pokemonid = Convert.ToInt32((p.Evolutions[i].Resource_uri.Split('/')[6]));      //Example: Id of Ivysaur

                        //If the evolution is above 151 we shouldn't add it                           
                        if (pokemonid <= GEN && pokemons[pokemonid - 1].Evolutions.Count > 0)
                        {
                            //Add the evolution of the evolution to the list. Only when that evolution of the evolution is posible in the current generation
                            //As example we add Venusaur here!
                            if (Convert.ToInt32((pokemons[pokemonid - 1].Evolutions[0].Resource_uri.Split('/')[6])) <= GEN)
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
            foreach (var v in pokemons)
            {
                v.MovesWeb = WebScraper.ScrapeLevelMoves(v.Name);
                b.ReportProgress(33 + (Convert.ToInt32(v.Pkdx_id / (GEN / 33.3))));
            }
        }

        private static void ParseMachines(List<JsonParse.Pokemon> pokemons, BackgroundWorker b)
        {
            foreach (var v in pokemons)
            {
                v.MachinesWeb = WebScraper.ScrapeMachineMoves(v.Name);
                b.ReportProgress(66 + (Convert.ToInt32(v.Pkdx_id / (GEN / 33.3))));
            }
        }
    }
}
