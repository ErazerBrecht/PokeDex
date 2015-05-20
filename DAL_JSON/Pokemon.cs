using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DAL_JSON
{
    public class Pokemon
    {
        public int Pkdx_id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Species { get; set; }
        public string ImageURL { get; set; }

        public string SoundURL
        {
            get
            {
                string id = Pkdx_id.ToString("000");
                return Path.Combine(Application.LocalUserAppDataPath, "PokemonCries", id + ".wav");
            }
        }

        public double Weight { get; set; }
        public double Height { get; set; }

        [JsonProperty(PropertyName = "male_female_ratio")]
        public string MaleFemaleRatio { get; set; }

        [JsonProperty(PropertyName = "catch_rate")]
        public double CatchRate { get; set; }

        public List<Evolution> Evolutions { get; set; }
        public List<Ability> Abilities { get; set; }
        public List<Move> MovesWeb { get; set; }
        public List<Move> MachinesWeb { get; set; }
        public List<Type> Types { get; set; }
        public List<Description> Descriptions { get; set; }

        //Skills
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        [JsonProperty(PropertyName = "sp_atk")]
        public int SPAttack { get; set; }

        [JsonProperty(PropertyName = "sp_def")]
        public int SPDefense { get; set; }

        public int Speed { get; set; }

        public string SpeedAsImage
        {
            get
            {
                if (Speed >= 90)
                    return "Resources/Pikachu.png";
                else if (Speed >= 50)
                    return "Resources/Kirlia.png";
                else
                    return "Resources/Shuckle.png";
            }
        }
    }

    public class Ability
    {
        public string Name { get; set; }
    }

    public class Move
    {

        //[JsonProperty(PropertyName = "learn_type")]
        //public string LearnType { get; set; }

        public string Name { get; set; }

        public string Level { get; set; }
        public int Accuracy { get; set; }
        public int Power { get; set; }
        public int PP { get; set; }
        public string Type { get; set; }

        public string TypeURL
        {
            get { return "http://veekun.com/dex/media/types/en/" + Type.ToLower() + ".png"; }
        }

        //public string Description { get; set; }
    }

    public class Type
    {
        public string Name { get; set; }
    }

    public class Evolution
    {
        public int Level { get; set; }
        public string Method { get; set; }
        public string To { get; set; }
        public string ImageURL { get; set; }
        public string Resource_uri { get; set; }
    }

    public class Description
    {
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string DescriptionText { get; set; }
        public string Resource_uri { get; set; }
    }

}
