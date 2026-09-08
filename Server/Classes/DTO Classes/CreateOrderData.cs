using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public class CreateOrderData
    {
        public int CustomerId { get; set; }
        public int BouquetId { get; set; }
        public string SpecialRequirements { get; set; }
    }
}
