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

        private int playerHitChance;
        private int enemyHitChance;
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
        private void HandleLevelup(int expGained)
        {
            if (player == null) { return; }
            player.Exp += expGained;
            if (player.Exp >= player.requiredExp)
            {
                int excessExp = player.Exp - player.requiredExp;
                player.Level++;
                player.requiredExp = (int)(player.requiredExp * 1.2);

                // Call the modified PrintCombatMessage method for the level up message
                PrintCombatMessage($"You got a new level!\n", 1500);

                PrintCombatMessage($"Which stat you want to increase ([A]TK or [D]EF)", 0);
                ConsoleKeyInfo key = Console.ReadKey(true);
                while (key.KeyChar != 'a' && key.KeyChar != 'A' && key.KeyChar != 'd' && key.KeyChar != 'D')
                {
                    PrintCombatMessage($"Invalid input. Choose which stat you want to increase ([A]TK or [D]EF)", 1500);
                    key = Console.ReadKey(true);
                }

                if (key.KeyChar == 'a' || key.KeyChar == 'A')
                {
                    player.ATK++;
                }
                else if (key.KeyChar == 'd' || key.KeyChar == 'D')
                {
                    player.DEF++;
                }

                player.Exp = excessExp;
            }
        }




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
            playerHitChance = Math.Max(5, 75 + (player.ATK - enemy.DEF) * 5); // Minimum hit chance is 5%
            if (playerHitChance > 100) { playerHitChance = 100; }
            enemyHitChance = Math.Max(5, 75 + (enemy.ATK - player.DEF) * 5); // Minimum hit chance is 5%
            if (enemyHitChance > 100) { enemyHitChance = 100; }
            while (player.IsInCombat)
            {

                Ambush();
                PrintCombatScreen();
                var combatInput = InputHandler.GetCombatInput();
                if (combatInput == 'A') // Attack
                {


                    if (random.Next(0, 100) < playerHitChance)
                    {
                        // Add a damage variability factor (e.g., 0.3 for +/- 30%)
                        float damageVariability = 0.3f;
                        // Calculate the base damage
                        int damage = (int)(player.ATK * (1 - (enemy.DEF / (float)(enemy.DEF + 100))));
                        // Generate a random percentage between -damageVariability and +damageVariability for both ATK and DEF
                        float randomATKPercentage = (float)random.NextDouble() * 2 * damageVariability - damageVariability;
                        float randomDEFPercentage = (float)random.NextDouble() * 2 * damageVariability - damageVariability;
                        // Apply the random factor to the base damage
                        damage = (int)(damage * (1 + randomATKPercentage) * (1 - randomDEFPercentage));
                        // Ensure the minimum damage is 1
                        damage = Math.Max(1, damage);
                        // Apply the damage to the enemy's HP
                        enemy.HP -= damage;
                        PrintCombatMessage($"You hit {enemy.Name} for {damage} damage.", 500);
                    }
                    else
                    {
                        PrintCombatMessage($"You missed!", 500);
                    }
                    if (enemy.HP > 0)
                    {
                        EnemyAttack();
                    }
                    else
                    {
                        int expGained = enemy.MaxHp + (enemy.ATK + enemy.DEF) / 3;
                        PrintCombatMessage($"{enemy.Name} died!", 1500);
                        PrintCombatMessage($"You got {expGained} experience!\n", 1500);


                        HandleLevelup(expGained);

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

            if (random.Next(0, 100) < enemyHitChance)
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
                PrintCombatMessage($"{enemy.Name} hits you for {damage} damage.", 500);
                if (player.HP <= 0)
                {
                    if (Program.mapArray == null) { return; }
                    Console.WriteLine("You died!");
                    Thread.Sleep(500);
                    player.playerDead = true;
                    player.IsInCombat = false;
                    // Save the current position to put the original ground tile back after teleporting
                    int oldY = player.Y;
                    int oldX = player.X;
                    // Teleport player to a new random position on the mapArray
                    (player.Y, player.X) = player.GetRandomStartPosition(Program.mapArray);
                    // Put the original ground tile back at the old position
                    Program.mapArray[oldY, oldX] = Program.previousTile;
                    // Update the previousTile with the new position's tile
                    Program.previousTile = Program.mapArray[player.Y, player.X];
                    // Place the player's character at the new position
                    Program.mapArray[player.Y, player.X] = player.Character;
                    player.moveFirst = 0;
                }
            }
            else
            {
                PrintCombatMessage($"{enemy.Name} misses completely!", 500);
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
            Console.WriteLine($" {player.HP}/{player.MaxHp} :: {playerHitChance}%");
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
            Console.WriteLine($" {enemy.HP}/{enemy.MaxHp} :: {enemyHitChance}%");
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
            Console.WriteLine(" [A]ttack, [R]un");
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
