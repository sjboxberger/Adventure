using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Engine
{
    public class Monster : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaxDamage { get; set; }
        public int RewardExp { get; set; }
        public int RewardGold { get; set; }
        public List<LootItem> LootTable { get; set; }

        public Monster(int iD, string name, int maxDamage, int rewardExp, int rewardGold, int currentHP, int maxHP) : base(currentHP, maxHP)
        {
            ID = iD;
            Name = name;
            MaxDamage = maxDamage;
            RewardExp = rewardExp;
            RewardGold = rewardGold;
            LootTable = new List<LootItem>();
        }
    }
}
