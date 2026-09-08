using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Florist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Experience { get; set; }
        public string Specialty { get; set; }
        public bool IsAvailable { get; set; } = true;
        public List<Order> AssignedOrders { get; set; } = new();

        public override string ToString() => $"{Name} {Surname} (опыт: {Experience} лет)";
    }
}
