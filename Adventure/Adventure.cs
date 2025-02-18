using Adventure_Engine;
using System.Runtime.CompilerServices;

namespace Adventure
{
    public partial class Adventure : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public Adventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            _player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            //MoveTo(World.LocationByID(World.LOCATION_ID_HOME));

            lblExperience.Text = _player.Exp.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblHitPoints.Text = _player.CurrentHP.ToString();
            //lblLevel.Text = _player.Level.ToString();
            lblLevel.Text = _player.CurrentLocation.Name;
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationNorth);
        }
        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationSouth);
        }
        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationEast);
        }
        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationWest);
        }
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }


        private void MoveTo(Location newLocation)
        {
            // Checking location item requirements
            if (!_player.HasRequiredItemsToEnterLocation(newLocation))
            {
                rtbMessages.Text += "You need a " + newLocation.ItemRequiredToEnter.Name + " to enter." + Environment.NewLine;
                return;
            }

            // If player has item, or no item is required, move to (update) new location.
            _player.CurrentLocation = newLocation;

            // Update movement button visibility
            btnNorth.Visible = (newLocation.LocationNorth != null);
            btnSouth.Visible = (newLocation.LocationSouth != null);
            btnEast.Visible = (newLocation.LocationEast != null);
            btnWest.Visible = (newLocation.LocationWest != null);

            // Display current location name/desc
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // Restore player HP to full health & update UI
            _player.CurrentHP = _player.MaxHP;
            lblHitPoints.Text = _player.CurrentHP.ToString();

            // New Location quest handling
            // If there is a quest in this location
            if(newLocation.QuestAvailableHere != null)
            {
                // boolean hasQuest/questCompleted vars
                bool playerHasQuest = _player.HasQuest(newLocation.QuestAvailableHere);
                bool playerCompletedQuest = _player.CompletedQuest(newLocation.QuestAvailableHere);

                // Logic handling a quest that has already been obtained
                if (playerHasQuest)
                {
                    // If quest is not completed, check for required items
                    if (!playerCompletedQuest)
                    {
                        // Assume required items are in player inventory...
                        // Easier to find one counterexample than to check
                        // if every case is true
                        bool playerHasRequiredQuestItems = _player.HasAllQuestItems(newLocation.QuestAvailableHere);
                                               
                        // If player has all items, handle quest completion.
                        if (playerHasRequiredQuestItems)
                        {
                            // Display completion message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the '" + newLocation.QuestAvailableHere.Name +
                                "' quest." + Environment.NewLine;

                            //Remove required items
                            _player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            // Display reward messages
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExp.ToString() + "exp" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + "gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            // Update UI
                            _player.Exp += newLocation.QuestAvailableHere.RewardExp;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            // Add reward item to player's inventory
                            _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem, 1);

                            // Mark quest as completed
                            _player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                // Logic handling quest that is not yet obtained
                else
                {
                    //Display quest receipt messages & completion info
                    rtbMessages.Text += "You receive the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete the quest, return with: " + Environment.NewLine;
                    foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if(qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    // Add quest to player quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // New Location Monster handling
            if(newLocation.MonsterLivingHere != null)
            {
                //Display monster messages
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                // Create monster using static World class, as base for making a 'current' monster
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                // Make current monster
                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaxDamage,
                    standardMonster.RewardExp, standardMonster.RewardGold, standardMonster.CurrentHP, standardMonster.MaxHP);

                // Add items to loot table
                foreach(LootItem li in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(li);
                }

                // Display combat actions
                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                // Set current monster to null & hide combat actions
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            //Refresh Player inventory list
            UpdateInventoryListUI();

            //Refresh player quest list
            UpdateQuestListUI();

            //Refresh player weapons combo box
            UpdateWeaponListUI();


            // Refresh player healing potions combo box
            UpdatePotionListUI();
        }

        private void UpdateInventoryListUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            // Add inventory items to grid display
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { ii.Details.Name, ii.Quantity.ToString() });
                }
            }
        }
        private void UpdateQuestListUI()
        {
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Quest";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Completed?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest pq in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { pq.Details.Name, pq.IsCompleted.ToString() });
            }
        }
        private void UpdateWeaponListUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            // Find weapons in player inventory
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details is Weapon)
                {
                    if (ii.Quantity > 0)
                    {
                        weapons.Add((Weapon)ii.Details);
                    }
                }
            }

            // Display weapons/funcitonality based on #weapons in inventory
            if (weapons.Count == 0)
            {
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";
                cboWeapons.SelectedIndex = 0;
            }
        }
        private void UpdatePotionListUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details is HealingPotion)
                {
                    if (ii.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)ii.Details);
                    }
                }
            }
            if (healingPotions.Count == 0)
            {
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
        }
    }
}
