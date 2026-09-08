using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public Customer Customer { get; set; }
        public Bouquet Bouquet { get; set; }
        public Florist Florist { get; set; }
        public Delivery Delivery { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "New";
        public string SpecialRequirements { get; set; } 

        public override string ToString() => $"Заказ {Id}: {Customer?.Name} - {Status}";
    }
}
