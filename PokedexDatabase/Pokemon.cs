using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokedexDatabase
{
    public class Pokemon
    {
        public int PokIDPK { get; set; }
        public int PokDexID { get; set; }
        public string PokName { get; set; }
        public string PokDescription { get; set; }
        public double PokSize { get; set; }
        public double PokWeight { get; set; }

        public int PokRegIDFK { get; set; }
    }
}
