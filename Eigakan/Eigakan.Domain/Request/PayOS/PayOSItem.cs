using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.PayOS
{
    public class PayOSItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public PayOSItem(string name, int quantity, int price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
        }

    }
}
