using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Engine
{
    public class LootItem
    {
        public Item Details { get; set; }
        public int DropChance { get; set; }
        public bool IsDefault { get; set; }
        public LootItem(Item details, int dropChance, bool isDefault=true)
        {
            Details = details;
            DropChance = dropChance;
            IsDefault = isDefault;
        }
    }
}
