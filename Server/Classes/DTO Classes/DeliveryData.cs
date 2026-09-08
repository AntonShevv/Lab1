using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public class DeliveryData
    {
        public int OrderId { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CourierName { get; set; }
        public string CourierPhone { get; set; }
        public string Notes { get; set; }
    }
}
