﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_JSON;
using Newtonsoft.Json;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace BS_PokedexManager
{
    public class Business
    {
        private static WebClient client = new WebClient();
        private static int _maxPokemonGen;
        private static int _gen;
        public static string StringGen { get; set; }
        public static MaxValue MaxStatsValue = new MaxValue();

        public enum Generation
        {
            I = 151,      //151
            II = 251,     //251
            III = 386,
            IV = 493,
            V = 649,
        }

        public static DescriptionProgressBar DescriptionProgress = new DescriptionProgressBar();

        public static ListClass CheckSetting()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.Generation))
            {
                StringGen = Properties.Settings.Default.Generation;
                ObservableCollection<Pokemon> pokeObservable = new ObservableCollection<Pokemon>(DAL_JSON.JsonParse.GetPokemons(Path.Combine(Application.LocalUserAppDataPath, "GeneratedList.txt")));
                CalculateMaxStats(StringGen);
                ListClass l = new ListClass(pokeObservable);
                return l;
            }

            return null;
        }

        private static void CalculateMaxStats(string generation) 
        {
            switch (generation)
            {
                case "I":
                    MaxStatsValue.MaxHp = 250;
                    MaxStatsValue.MaxAttack = 134;
                    MaxStatsValue.MaxDefense = 180;
                    MaxStatsValue.MaxSPAttack = 154;
                    MaxStatsValue.MaxSPDefense = 154;
                    MaxStatsValue.MaxSpeed = 140;
                    break;
                case "II":
                    MaxStatsValue.MaxHp = 255;
                    MaxStatsValue.MaxAttack = 134;
                    MaxStatsValue.MaxDefense = 230;
                    MaxStatsValue.MaxSPAttack = 154;
                    MaxStatsValue.MaxSPDefense = 230;
                    MaxStatsValue.MaxSpeed = 140;
                    break;
                case "III":
                    MaxStatsValue.MaxHp = 255;
                    MaxStatsValue.MaxAttack = 160;
                    MaxStatsValue.MaxDefense = 230;
                    MaxStatsValue.MaxSPAttack = 154;
                    MaxStatsValue.MaxSPDefense = 230;
                    MaxStatsValue.MaxSpeed = 160;
                    break;
                case "IV":
                    break;
                case "V":
                    break;
            }
        }

        public static ListClass GeneratePokeList(Generation g, BackgroundWorker b)
        {
            _maxPokemonGen = Convert.ToInt32(g);
            StringGen = g.ToString();

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
            ParseEvolutions(list);
            ParseMoves(list, b);
            ParseMachines(list, b);
            SavePokemons(list);
            Properties.Settings.Default.Generation = StringGen;
            Properties.Settings.Default.Save();

            CalculateMaxStats(StringGen);

            ObservableCollection<Pokemon> pokeObservable =
                new ObservableCollection<Pokemon>(list);

            ListClass l = new ListClass(pokeObservable);

            return l;

        }

        private static void RemovePokemons(List<Pokemon> pokemons)
        {
            pokemons.RemoveRange(_maxPokemonGen, pokemons.Count - _maxPokemonGen);
        }

        private static void ParseDescriptions(List<Pokemon> pokemons, BackgroundWorker b)
        {
            DescriptionProgress.Description = "Adding the correct description to the pokemons";
            foreach (Pokemon p in pokemons)
            {
                for (int i = 0; i < p.Descriptions.Count; i++)
                {
                    if (p.Descriptions[i].Name.Contains(_gen.ToString()))
                    {
                        string _jsonString = client.DownloadString(p.Descriptions[i].Resource_uri);
                        p.Descriptions[i] = JsonConvert.DeserializeObject<Description>(_jsonString);
                        p.Description = p.Descriptions[i].DescriptionText;
                        b.ReportProgress(Convert.ToInt32(p.Pkdx_id / (_maxPokemonGen / 33.3333)));
                        break;
                    }
                }
            }
        }

        private static void ParseEvolutions(List<Pokemon> pokemons)
        {
            foreach (Pokemon p in pokemons)
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
                            foreach (Evolution e in pokemons[pokemonid - 1].Evolutions)
                            {
                                if (Convert.ToInt32((e.Resource_uri.Split('/')[6])) <= _maxPokemonGen)
                                    p.Evolutions.Add(e);
                            }
                        }
                    }
                }

                //Add pictures uri to all evolutions!
                foreach (Evolution e in p.Evolutions)
                {
                    e.ImageURL = pokemons[Convert.ToInt32((e.Resource_uri.Split('/')[6])) - 1].ImageURL;
                }

            }
        }

        private static void ParseMoves(List<Pokemon> pokemons, BackgroundWorker b)
        {
            DescriptionProgress.Description = "Adding the correct moves to the pokemons (Webscraping)";

            foreach (var v in pokemons)
            {
                v.MovesWeb = WebScraper.ScrapeLevelMoves(v.Name, StringGen);
                b.ReportProgress(33 + (Convert.ToInt32(v.Pkdx_id / (_maxPokemonGen / 33.3333))));
            }
        }

        private static void ParseMachines(List<Pokemon> pokemons, BackgroundWorker b)
        {
            DescriptionProgress.Description = "Adding the correct machines (TM/HM) to the pokemons (Webscraping)";

            foreach (var v in pokemons)
            {
                v.MachinesWeb = WebScraper.ScrapeMachineMoves(v.Name, StringGen);
                b.ReportProgress(66 + (Convert.ToInt32(v.Pkdx_id / (_maxPokemonGen / 33.3333))));
            }
        }

        private static void SavePokemons(List<Pokemon> pokemons)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(Application.LocalUserAppDataPath, "GeneratedList.txt")))
            {
                sw.Write("{Property1:\n");
                sw.Write("[\n");
                foreach (Pokemon p in pokemons)
                {
                    string json = JsonConvert.SerializeObject(p, Formatting.Indented);
                    sw.Write(json);
                    sw.Write(",\n");
                }

                sw.Write("]\n");
                sw.Write("}\n");
            }
        }
    }

    public class MaxValue
    {
        public int MaxHp { get; set; }
        public int MaxAttack { get; set; }
        public int MaxDefense { get; set; }
        public int MaxSPAttack { get; set; }
        public int MaxSPDefense { get; set; }
        public int MaxSpeed { get; set; }
    }
}
