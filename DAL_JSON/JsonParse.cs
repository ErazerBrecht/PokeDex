using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DAL_JSON
{
    public class JsonParse
    {
        private static List<Pokemon> _pokeList;

        public static List<Pokemon> GetPokemons(string filename = "JSON.txt")
        {
            _pokeList = new List<Pokemon>();

            using (StreamReader sr = new StreamReader(filename))
            {
                string json = sr.ReadToEnd();
                Rootobject jPokemons = JsonConvert.DeserializeObject<Rootobject>(json);

                foreach (Pokemon p in jPokemons.AllPokemons)
                {
                    _pokeList.Add(p);
                }
            }

            return _pokeList;
        }

        public class Rootobject
        {
            [JsonProperty(PropertyName = "Property1")]
            public Pokemon[] AllPokemons { get; set; }
        }

    }
}
