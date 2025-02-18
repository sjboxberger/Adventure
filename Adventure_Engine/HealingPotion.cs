using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Engine
{
    public class HealingPotion : Item
    {
        public int AmountTotal { get; set; }

        public HealingPotion(int id, string name, string namePlural, int amountTotal) : base(id, name, namePlural)
        {
            AmountTotal = amountTotal;
        }
    }
}
