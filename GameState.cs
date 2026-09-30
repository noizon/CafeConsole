// ============================================================
// GameState.cs — игровая логика кафе-симулятора
// ============================================================

using System.Text.Json;

namespace CafeConsole
{
    public class MenuItem
    {
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public int Cost { get; set; }
        public int UnlockPrice { get; set; }
        public int RequiredReputation { get; set; }
        public int RequiredDay { get; set; }
        public bool Unlocked { get; set; }
    }

    public class Upgrade
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Price { get; set; }
        public bool Purchased { get; set; }
        public string EffectType { get; set; } = "";
        public int EffectValue { get; set; }
    }

    public class GameState
    {
        public int Money { get; set; } = 500;
        public int Day { get; set; } = 1;
        public int Reputation { get; set; } = 0;
        public int MaxReputation { get; set; } = 100;
        public int DaysToSurvive { get; set; } = 10;

        public int TotalEarned { get; set; } = 0;
        public int TotalServed { get; set; } = 0;
        public int TotalLost { get; set; } = 0;
        public int TotalUnlocked { get; set; } = 0;
        public int TotalUpgrades { get; set; } = 0;

        public List<MenuItem> MenuItems { get; set; } = new();
        public List<Upgrade> Upgrades { get; set; } = new();

        public GameState()
        {
            MenuItems.Add(new MenuItem { Name = "Эспрессо", Price = 150, Cost = 30, Unlocked = true });
            MenuItems.Add(new MenuItem { Name = "Американо", Price = 180, Cost = 35, Unlocked = true });
            MenuItems.Add(new MenuItem { Name = "Булка с маком", Price = 100, Cost = 20, Unlocked = true });

            MenuItems.Add(new MenuItem { Name = "Латте", Price = 250, Cost = 60, UnlockPrice = 300, RequiredReputation = 20, RequiredDay = 2 });
            MenuItems.Add(new MenuItem { Name = "Капучино", Price = 240, Cost = 55, UnlockPrice = 300, RequiredReputation = 25, RequiredDay = 2 });
            MenuItems.Add(new MenuItem { Name = "Круассан", Price = 150, Cost = 40, UnlockPrice = 500, RequiredReputation = 35, RequiredDay = 3 });
            MenuItems.Add(new MenuItem { Name = "Банановый хлеб", Price = 180, Cost = 50, UnlockPrice = 600, RequiredReputation = 45, RequiredDay = 4 });
            MenuItems.Add(new MenuItem { Name = "Матча Латте", Price = 300, Cost = 90, UnlockPrice = 1000, RequiredReputation = 60, RequiredDay = 5 });
            MenuItems.Add(new MenuItem { Name = "Колд Брю", Price = 280, Cost = 70, UnlockPrice = 900, RequiredReputation = 55, RequiredDay = 5 });

            Upgrades.Add(new Upgrade { Name = "Новая кофемашина", Description = "+20% к цене всех напитков", Price = 800, EffectType = "price", EffectValue = 20 });
            Upgrades.Add(new Upgrade { Name = "Реклама в соцсетях", Description = "+1 клиент каждый день", Price = 600, EffectType = "customers", EffectValue = 1 });
            Upgrades.Add(new Upgrade { Name = "Опытный бариста", Description = "+2 репутации за клиента", Price = 1000, EffectType = "reputation", EffectValue = 2 });
            Upgrades.Add(new Upgrade { Name = "Скидка у поставщика", Description = "-15% к себестоимости", Price = 1200, EffectType = "discount", EffectValue = 15 });
            Upgrades.Add(new Upgrade { Name = "Уютный интерьер", Description = "+3 клиента каждый день", Price = 2500, EffectType = "customers", EffectValue = 3 });
            Upgrades.Add(new Upgrade { Name = "Мишленовский шеф", Description = "+5 репутации за клиента", Price = 4000, EffectType = "reputation", EffectValue = 5 });
        }

        // ============================================================
        // ЭКОНОМИКА
        // ============================================================

        public int GetEffectivePrice(MenuItem item)
        {
            int bonus = Upgrades
                .Where(u => u.Purchased && u.EffectType == "price")
                .Sum(u => u.EffectValue);
            int result = item.Price + (item.Price * bonus / 100);
            return Math.Max(1, result);
        }

        public int GetEffectiveCost(MenuItem item)
        {
            int discount = Upgrades
                .Where(u => u.Purchased && u.EffectType == "discount")
                .Sum(u => u.EffectValue);

            int result = item.Cost - (item.Cost * discount / 100);
            return Math.Max(1, result);
        }

        public int GetCustomersPerDay()
        {
            int baseCount = 3 + Reputation / 15;
            int extra = Upgrades
                .Where(u => u.Purchased && u.EffectType == "customers")
                .Sum(u => u.EffectValue);
            return Math.Max(1, baseCount + extra);
        }

        public int GetReputationGain()
        {
            int bonus = Upgrades
                .Where(u => u.Purchased && u.EffectType == "reputation")
                .Sum(u => u.EffectValue);
            return 2 + bonus;
        }

        public List<MenuItem> GetUnlockedItems() => MenuItems.Where(m => m.Unlocked).ToList();
        public List<MenuItem> GetLockedItems() => MenuItems.Where(m => !m.Unlocked).ToList();

        // ============================================================
        // ОТКРЫТИЕ / АПГРЕЙДЫ
        // ============================================================

        public bool TryUnlockItem(string itemName, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(itemName)) { error = "Пустое название."; return false; }

            var item = MenuItems.FirstOrDefault(m =>
                !m.Unlocked && m.Name.ToLower() == itemName.Trim().ToLower());

            if (item == null) { error = "Такого продукта нет или он уже открыт."; return false; }
            if (Day < item.RequiredDay) { error = $"Нужен день {item.RequiredDay}, сейчас {Day}."; return false; }
            if (Reputation < item.RequiredReputation) { error = $"Нужна репутация {item.RequiredReputation}, сейчас {Reputation}."; return false; }
            if (Money < item.UnlockPrice) { error = $"Нужно {item.UnlockPrice} руб., у тебя {Money}."; return false; }

            Money -= item.UnlockPrice;
            item.Unlocked = true;
            TotalUnlocked++;
            return true;
        }

        public bool TryBuyUpgrade(string upgradeName, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(upgradeName)) { error = "Пустое название."; return false; }

            var up = Upgrades.FirstOrDefault(u => u.Name.ToLower() == upgradeName.Trim().ToLower());
            if (up == null) { error = "Такого апгрейда нет."; return false; }
            if (up.Purchased) { error = "Уже куплено."; return false; }
            if (Money < up.Price) { error = $"Нужно {up.Price} руб., у тебя {Money}."; return false; }

            Money -= up.Price;
            up.Purchased = true;
            TotalUpgrades++;
            return true;
        }

        // ============================================================
        // СОХРАНЕНИЕ
        // ============================================================

        public void Save(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(this, options));
        }

        public static GameState Load(string path)
        {
            if (!File.Exists(path)) return new GameState();
            try
            {
                string json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<GameState>(json);
                return loaded ?? new GameState();
            }
            catch
            {
                return new GameState();
            }
        }

        // ============================================================
        // РАБОЧИЙ ДЕНЬ
        // ============================================================
        public int ProcessDay()
        {
            int customers = GetCustomersPerDay();
            var rng = Random.Shared;
            var unlocked = GetUnlockedItems();
            int earnedThisDay = 0;
            int repGain = GetReputationGain();

            for (int i = 0; i < customers; i++)
            {
                var item = unlocked[rng.Next(unlocked.Count)];

                // Обновляем шапку живьём
                Console.Clear();
                Ui.DrawDayHeader(Day, DaysToSurvive, Money, Reputation, MaxReputation, i + 1, customers);
                Console.WriteLine();

                int price = GetEffectivePrice(item);
                int cost = GetEffectiveCost(item);
                int profit = price - cost;

                Ui.DrawCustomerCard(item.Name, price, cost, profit);

                Console.WriteLine();
                Ui.PrintCentered("[ Y ] Обслужить        [ N ] Отказать", ConsoleColor.DarkGray);
                Console.WriteLine();

                // Читаем клавишу пока не Y или N
                ConsoleKey key;
                do
                {
                    key = Console.ReadKey(true).Key;
                } while (key != ConsoleKey.Y && key != ConsoleKey.N);

                if (key == ConsoleKey.Y)
                {
                    Money += profit;
                    earnedThisDay += profit;

                    Reputation = Math.Min(MaxReputation, Reputation + repGain);
                    TotalServed++;

                    // Живое обновление шапки
                    Console.Clear();
                    Ui.DrawDayHeader(Day, DaysToSurvive, Money, Reputation, MaxReputation, i + 1, customers);
                    Console.WriteLine();
                    Ui.DrawCustomerCard(item.Name, price, cost, profit);
                    Console.WriteLine();
                    Ui.PrintCentered($"✅ +{profit} руб.   ⭐ +{repGain} репутации", ConsoleColor.Green);
                }
                else
                {
                    Reputation = Math.Max(0, Reputation - 4);
                    TotalLost++;

                    Console.Clear();
                    Ui.DrawDayHeader(Day, DaysToSurvive, Money, Reputation, MaxReputation, i + 1, customers);
                    Console.WriteLine();
                    Ui.DrawCustomerCard(item.Name, price, cost, profit);
                    Console.WriteLine();
                    Ui.PrintCentered($"❌ Клиент ушёл. -4 репутации", ConsoleColor.Red);
                }

                Thread.Sleep(700);
            }

            TotalEarned += earnedThisDay;
            return earnedThisDay;
        }
    }
}