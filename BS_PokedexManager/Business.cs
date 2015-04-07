using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_JSON;
using Newtonsoft.Json;
using System.Net;
using System.Collections.ObjectModel;

namespace BS_PokedexManager
{
    public class Business
    {
        static WebClient client = new WebClient();

        public enum Generation
        {
            I = 151,
            II = 251,
            III = 386,
            IV = 493,
            V = 649,
        }

        public static ObservableCollection<JsonParse.Pokemon> GeneratePokeList()
        {
            var list = DAL_JSON.JsonParse.GetPokemons();
            ParseDescriptions(list);

            return list;
        }

        private static void ParseDescriptions(ObservableCollection<JsonParse.Pokemon> pokemons)
        {
            Console.WriteLine("\nStarting to add description from GEN 2 to Pokemon as string");
            foreach (DAL_JSON.JsonParse.Pokemon p in pokemons)
            {
                for (int i = 0; i < p.Descriptions.Count; i++)
                {
                    if (p.Descriptions[i].Name.Contains("2"))
                    {
                        string _jsonString = client.DownloadString(p.Descriptions[i].Resource_uri);
                        p.Descriptions[i] = JsonConvert.DeserializeObject<DAL_JSON.JsonParse.Description>(_jsonString);
                        p.Description = p.Descriptions[i].DescriptionText;
                        break;
                    }
                }
            }
        }
    }
}
