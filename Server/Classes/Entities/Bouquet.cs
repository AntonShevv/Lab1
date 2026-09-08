using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Bouquet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string FlowersComposition { get; set; } 
        public int Quantity { get; set; }

        public override string ToString() => $"{Name} - {Price} грн";
    }
}
