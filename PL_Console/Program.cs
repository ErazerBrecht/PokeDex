using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var ListPokemons = DAL_JSON.JsonParse.GetPokemons();
        }
    }
}
