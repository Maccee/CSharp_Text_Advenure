// modules/combat.cs
using Newtonsoft.Json;

namespace ConsoleApp.Modules
{
    public class Combat
    {
        Random random = new Random();
        Player? player = Program.player;
        Enemy? enemy;
        private List<Enemy> _enemies;
        private bool enemyFirstStrike = false;
        private int ambushChance = 10; // 10% chance of enemy ambush


        // Constructor: initializes the _enemies list and loads enemies from JSON file
        public Combat()
        {
            _enemies = new List<Enemy>();
            LoadEnemiesFromJson();
            enemy = _enemies[random.Next(_enemies.Count)];
        }

        // Loads enemy data from the JSON file and assigns it to the _enemies list
        private void LoadEnemiesFromJson()
        {
            string json = File.ReadAllText("enemy.json");
            var enemyData = JsonConvert.DeserializeObject<EnemyData>(json);
            _enemies = enemyData?.Enemies ?? new List<Enemy>();
        }

        // Enemy ambush and first strike
        private void Ambush()
        {
            if (enemyFirstStrike)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine(" AMBUSHED ");
                    Thread.Sleep(500);
                    Console.Clear();
                    Thread.Sleep(500);
                }
                Console.ResetColor();
                PrintCombatScreen();
                Thread.Sleep(500);
                EnemyAttack();
                enemyFirstStrike = false;
            }
        }

        // Handles combat between the player and a randomly-selected enemy
        public void StartCombat()
        {
            if (player == null || enemy == null) { return; }

            ViewPort.statusWindow = false;

            // Check for enemy ambush
            enemyFirstStrike = random.Next(1, 101) <= ambushChance;
            enemy.MaxHp = enemy.MaxHp + random.Next(-10, 10);
            enemy.HP = enemy.MaxHp;

            while (player.IsInCombat)
            {
                Ambush();

                PrintCombatScreen();

                var combatInput = InputHandler.GetCombatInput();

                if (combatInput == 'A') // Attack
                {
                    int hitChance = Math.Max(5, 75 + (player.ATK - enemy.DEF) * 5); // Minimum hit chance is 5%
                    if (random.Next(0, 100) < hitChance)
                    {
                        // Add a damage variability factor (e.g., 0.2 for +/- 20%)
                        float damageVariability = 0.2f;
                        // Calculate the base damage
                        int damage = (int)(player.ATK * (1 - (enemy.DEF / (float)(enemy.DEF + 100))));
                        // Generate a random percentage between -damageVariability and +damageVariability
                        float randomPercentage = (float)random.NextDouble() * 2 * damageVariability - damageVariability;
                        // Apply the random factor to the base damage
                        damage = (int)(damage * (1 + randomPercentage));
                        // Ensure the minimum damage is 1
                        damage = Math.Max(1, damage);
                        // Apply the damage to the enemy's HP
                        enemy.HP -= damage;

                        PrintCombatMessage($"You hit {enemy.Name} for {damage} damage. {hitChance}%", 500);
                    }
                    else
                    {
                        PrintCombatMessage($"You missed! {hitChance}%", 500);
                    }
                    if (enemy.HP > 0)
                    {
                        EnemyAttack();
                    }
                    else
                    {
                        PrintCombatMessage($"{enemy.Name} died!", 1500);
                        PrintCombatMessage($"You got {enemy.MaxHp / 2} experience!", 1500);
                        player.Exp = player.Exp + enemy.MaxHp / 2;
                        player.IsInCombat = false;
                        player.moveFirst = 0;
                        Console.Clear();
                    }
                }
                else if (combatInput == 'R') // Run from combat
                {
                    player.IsInCombat = false;
                    player.moveFirst = 0;
                    Console.Clear();
                }
            }
        }

        // Enemy attack logic
        private void EnemyAttack()
        {
            if (player == null || enemy == null) { return; }

            int hitChance = Math.Max(5, 75 + (enemy.ATK - player.DEF) * 5); // Minimum hit chance is 5%

            if (random.Next(0, 100) < hitChance)
            {
                // Add a damage variability factor (e.g., 0.2 for +/- 20%)
                float damageVariability = 0.2f;
                // Calculate the base damage
                int damage = (int)(enemy.ATK * (1 - (player.DEF / (float)(player.DEF + 100))));
                // Generate a random percentage between -damageVariability and +damageVariability
                float randomPercentage = (float)random.NextDouble() * 2 * damageVariability - damageVariability;
                // Apply the random factor to the base damage
                damage = (int)(damage * (1 + randomPercentage));
                // Ensure the minimum damage is 1
                damage = Math.Max(1, damage);
                // Apply the damage to the player's HP
                player.HP -= damage;

                PrintCombatMessage($"{enemy.Name} hits you for {damage} damage. {hitChance}%", 500);

                if (player.HP <= 0)
                {
                    player.IsInCombat = false;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("You died!");
                    Thread.Sleep(1000);
                    player.playerDead = true;
                    // Add logic for player defeat, e.g., game over or respawn
                }
            }
            else
            {
                PrintCombatMessage($"{enemy.Name} misses completely! {hitChance}%", 500);
            }
        }

        // New method to print combat messages and pause for a given duration
        private void PrintCombatMessage(string message, int pauseDuration)
        {

            foreach (char c in message)
            {
                if (message == "You missed!")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(message);
                    break;
                }
                if (Char.IsDigit(c))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write(c);
                Console.ResetColor();

            }
            Console.WriteLine();
            Thread.Sleep(pauseDuration);
        }
        // Prints the combat screen, displaying player and enemy HP, and available commands
        public void PrintCombatScreen()
        {
            if (player == null || enemy == null) { return; }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(" ▄▄·           ▌ ▄ ·.  ▄▄▄▄·   ▄▄▄·  ▄▄▄▄▄");
            Console.WriteLine("▐█ ▌·  ▄█▀▄  ·██ ▐███· ▐█ ▀█· ▐█ ▀█   ██");
            Console.WriteLine("██ ▄▄ ▐█▌.▐▌ ▐█ ▌▐▌▐█· ▐█▀▀█▄ ▄█▀▀█   ▐█.");
            Console.WriteLine("▐███▌ ▐█▌.▐▌ ██ ██▌▐█▌ ██▄·▐█ ▐█· ▐▌  ▐█▌·");
            Console.WriteLine("·▀▀▀   ▀█▄▀· ▀▀  █·▀▀▀·▀▀▀▀    ▀   ▀  ▀▀▀");
            Console.WriteLine();

            // PLAYER DATA
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {player.Name}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" {player.HP}/{player.MaxHp}");
            Console.ForegroundColor = ConsoleColor.Green;
            int playerHealthPercentage = (int)(((double)player.HP / player.MaxHp) * 100);
            int playerHealthBars = (playerHealthPercentage * 40) / 100;
            Console.Write(" ");
            for (int i = 0; i < playerHealthBars; i++)
            {
                Console.Write("▌");
            }
            Console.Write("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine("");

            // ENEMY DATA
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {enemy.Name}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" {enemy.HP}/{enemy.MaxHp}");
            Console.ForegroundColor = ConsoleColor.Red;
            int enemyHealthPercentage = (int)(((double)enemy.HP / enemy.MaxHp) * 100);
            int enemyHealthBars = (enemyHealthPercentage * 40) / 100;
            Console.Write(" ");
            for (int i = 0; i < enemyHealthBars; i++)
            {
                Console.Write("▌");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ResetColor();

            // COMMANDS
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" (A)ttack, (R)un");
            Console.WriteLine("");
            Console.ResetColor();
        }


    }

    // Class to store a list of enemies deserialized from the JSON file
    public class EnemyData
    {
        public List<Enemy>? Enemies { get; set; }
    }

    // Class to represent an enemy with its name and stats
    public class Enemy
    {
        public string? Name { get; set; }
        public int MaxHp = 0;
        public int HP = 0;
        public int ATK = 0;
        public int DEF = 0;

    }


}
