using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Delivery
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal DeliveryCost { get; set; }
        public string Status { get; set; } = "Pending";
        public string CourierName { get; set; }
        public string CourierPhone { get; set; }
        public string Notes { get; set; }

        public override string ToString() => $"Доставка по адресу: {Address} - {Status}";
    }
}
