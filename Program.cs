// ============================================================
// Program.cs — точка входа и игровой цикл кафе-симулятора
// ============================================================

using CafeConsole;

class Program
{
    const string SaveFile = "savegame.json";
    static GameState state = new GameState();
    static bool hasActiveGame = false;

    // ============================================================
    // ТОЧКА ВХОДА
    // ============================================================
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        while (true)
        {
            int choice = Ui.ShowMenu("ГЛАВНОЕ МЕНЮ", new[]
            {
                "🎮  Новая игра",
                "💾  Загрузить сохранение",
                "❌  Выход"
            });

            switch (choice)
            {
                case 0: NewGame(); break;
                case 1: LoadGame(); break;
                case 2:
                    Console.Clear();
                    Ui.PrintCentered("Спасибо за игру! 👋", ConsoleColor.Cyan);
                    return;
            }
        }
    }

    // ============================================================
    // НОВАЯ ИГРА / ЗАГРУЗКА
    // ============================================================
    static void NewGame()
    {
        state = new GameState();
        hasActiveGame = true;
        state.Save(SaveFile);
        Ui.ShowMessage("✨ Новая игра создана!", ConsoleColor.Green);
        GameLoop();
    }

    static void LoadGame()
    {
        if (!File.Exists(SaveFile))
        {
            Ui.ShowMessage("❌ Сохранение не найдено.", ConsoleColor.Red);
            return;
        }

        state = GameState.Load(SaveFile);
        hasActiveGame = true;
        Ui.ShowMessage("💾 Сохранение загружено!", ConsoleColor.Green);
        GameLoop();
    }

    // ============================================================
    // ГЛАВНЫЙ ИГРОВОЙ ЦИКЛ
    // ============================================================
    static void GameLoop()
    {
        while (hasActiveGame)
        {
            Console.Clear();
            Ui.DrawGameHeader(state);

            int choice = Ui.ShowMenu("ДЕЙСТВИЯ", new[]
            {
                "☕  Начать рабочий день",
                "📋  Меню кафе (открыть позиции)",
                "🛠️  Магазин / Прокачка",
                "📊  Статистика",
                "💾  Сохранить игру",
                "🚪  Выйти в главное меню"
            });

            switch (choice)
            {
                case 0: StartDay(); break;
                case 1: ShowCafeMenu(); break;
                case 2: ShowShop(); break;
                case 3: ShowStats(); break;
                case 4: state.Save(SaveFile); Ui.ShowMessage("💾 Игра сохранена!", ConsoleColor.Green); break;
                case 5: hasActiveGame = false; break;
            }
        }
    }

    // ============================================================
    // РАБОЧИЙ ДЕНЬ
    // ============================================================
    static void StartDay()
    {
        int earned = state.ProcessDay();

        Console.Clear();
        Ui.DrawGameHeader(state);
        Ui.DrawSection($"ИТОГ ДНЯ {state.Day}", ConsoleColor.Cyan);

        Ui.PrintCentered($"💰 Заработано за день: {earned} руб.", ConsoleColor.Yellow);
        Ui.PrintCentered($"⭐ Репутация: {state.Reputation}/{state.MaxReputation}", ConsoleColor.Green);
        Console.WriteLine();
        Ui.PrintCentered(Ui.ReputationBar(state.Reputation, state.MaxReputation, 40), ConsoleColor.Green);
        Console.WriteLine();
        Ui.PrintCentered($"💵 В кассе: {state.Money} руб.", ConsoleColor.Yellow);

        state.Day++;
        state.Save(SaveFile);

        if (state.Day > state.DaysToSurvive)
        {
            EndGame();
            return;
        }

        Ui.ShowMessage("Нажми любую клавишу для продолжения...", ConsoleColor.Gray);
    }

    // ============================================================
    // ФИНАЛ
    // ============================================================
    static void EndGame()
    {
        Console.Clear();
        Ui.DrawLogo();
        Console.WriteLine();

        Ui.PrintCentered("🏁  И Г Р А   З А В Е Р Ш Е Н А", ConsoleColor.Magenta);
        Console.WriteLine();
        Ui.DrawSeparator();
        Console.WriteLine();

        int score = 0;
        score += state.TotalEarned / 100;
        score += state.Reputation;
        score += state.TotalServed * 2;
        score += state.TotalUnlocked * 30;
        score += state.TotalUpgrades * 40;

        Ui.PrintCentered($"💰 Всего заработано:      {state.TotalEarned,6} руб.", ConsoleColor.Yellow);
        Ui.PrintCentered($"🍽️  Обслужено клиентов:    {state.TotalServed,6}", ConsoleColor.Green);
        Ui.PrintCentered($"💔 Упущено клиентов:      {state.TotalLost,6}", ConsoleColor.Red);
        Ui.PrintCentered($"📋 Открыто позиций:       {state.TotalUnlocked,6}", ConsoleColor.Cyan);
        Ui.PrintCentered($"🛠️  Куплено апгрейдов:     {state.TotalUpgrades,6}", ConsoleColor.Cyan);
        Ui.PrintCentered($"⭐ Финальная репутация:   {state.Reputation,6}/{state.MaxReputation}", ConsoleColor.Green);

        Console.WriteLine();
        Ui.DrawSeparator();
        Console.WriteLine();

        string rank =
            score >= 500 ? "🏆 ЛЕГЕНДА КОФЕЙНИ" :
            score >= 350 ? "⭐ МАСТЕР БАРИСТА" :
            score >= 200 ? "🥈 ОПЫТНЫЙ БАРИСТА" :
            score >= 100 ? "🥉 НАЧИНАЮЩИЙ БАРИСТА" :
                           "☕ СТАЖЁР";

        Ui.PrintCentered($"Ранг: {rank}", ConsoleColor.Yellow);
        Ui.PrintCentered($"Итоговый счёт: {score}", ConsoleColor.Magenta);

        Ui.ShowMessage("Нажми любую клавишу для выхода...", ConsoleColor.Gray);
        hasActiveGame = false;
    }

    // ============================================================
    // МЕНЮ КАФЕ
    // ============================================================
    static void ShowCafeMenu()
    {
        while (true)
        {
            Console.Clear();
            Ui.DrawGameHeader(state);
            Ui.DrawSection("📋 МЕНЮ КАФЕ", ConsoleColor.Cyan);

            Console.ForegroundColor = ConsoleColor.White;
            Ui.PrintCentered("✅  ОТКРЫТЫЕ ПОЗИЦИИ", ConsoleColor.Green);
            Console.WriteLine();

            foreach (var item in state.GetUnlockedItems())
            {
                int price = state.GetEffectivePrice(item);
                int cost = state.GetEffectiveCost(item);
                string line = $"  • {item.Name,-18}  {price,4} руб.  (себест. {cost,3})";
                Ui.PrintCentered(line, ConsoleColor.Yellow);
            }

            var locked = state.GetLockedItems();
            if (locked.Count > 0)
            {
                Console.WriteLine();
                Ui.PrintCentered("🔒  ДОСТУПНО ДЛЯ ОТКРЫТИЯ", ConsoleColor.DarkYellow);
                Console.WriteLine();

                foreach (var item in locked)
                {
                    string status;
                    ConsoleColor statusColor;

                    if (state.Day < item.RequiredDay)
                    {
                        status = $"нужен день {item.RequiredDay}";
                        statusColor = ConsoleColor.DarkGray;
                    }
                    else if (state.Reputation < item.RequiredReputation)
                    {
                        status = $"нужна реп. {item.RequiredReputation}";
                        statusColor = ConsoleColor.DarkGray;
                    }
                    else if (state.Money < item.UnlockPrice)
                    {
                        status = $"нужно {item.UnlockPrice} руб.";
                        statusColor = ConsoleColor.DarkGray;
                    }
                    else
                    {
                        status = "✔ можно открыть!";
                        statusColor = ConsoleColor.Green;
                    }

                    string line = $"  • {item.Name,-18}  {item.UnlockPrice,5} руб.   [{status}]";
                    Ui.PrintCentered(line, statusColor);
                }
            }

            Console.WriteLine();
            Ui.PrintCentered("Введи название позиции для открытия (Enter — назад):", ConsoleColor.Gray);
            Console.WriteLine();
            Console.Write(new string(' ', Math.Max(0, (Console.WindowWidth - 30) / 2)));
            Console.Write("> ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            if (state.TryUnlockItem(input, out string error))
                Ui.ShowMessage($"✅ «{input}» открыто!", ConsoleColor.Green);
            else
                Ui.ShowMessage($"❌ {error}", ConsoleColor.Red);
        }
    }

    // ============================================================
    // МАГАЗИН
    // ============================================================
    static void ShowShop()
    {
        while (true)
        {
            Console.Clear();
            Ui.DrawGameHeader(state);
            Ui.DrawSection("🛠️  МАГАЗИН / ПРОКАЧКА", ConsoleColor.Cyan);

            foreach (var up in state.Upgrades)
            {
                if (up.Purchased)
                {
                    Ui.PrintCentered($"  ✅ {up.Name,-26}  куплено", ConsoleColor.Green);
                }
                else
                {
                    string line = $"  ⬜ {up.Name,-26}  {up.Price,5} руб.   {up.Description}";
                    Ui.PrintCentered(line, ConsoleColor.White);
                }
            }

            Console.WriteLine();
            Ui.PrintCentered("Введи название апгрейда для покупки (Enter — назад):", ConsoleColor.Gray);
            Console.WriteLine();
            Console.Write(new string(' ', Math.Max(0, (Console.WindowWidth - 30) / 2)));
            Console.Write("> ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            if (state.TryBuyUpgrade(input, out string error))
                Ui.ShowMessage($"✅ Куплено: «{input}»", ConsoleColor.Green);
            else
                Ui.ShowMessage($"❌ {error}", ConsoleColor.Red);
        }
    }

    // ============================================================
    // СТАТИСТИКА
    // ============================================================
    static void ShowStats()
    {
        Console.Clear();
        Ui.DrawGameHeader(state);
        Ui.DrawSection("📊 СТАТИСТИКА", ConsoleColor.Cyan);

        Ui.PrintCentered($"💰 Всего заработано:     {state.TotalEarned,6} руб.", ConsoleColor.Yellow);
        Ui.PrintCentered($"🍽️  Обслужено клиентов:   {state.TotalServed,6}", ConsoleColor.Green);
        Ui.PrintCentered($"💔 Упущено клиентов:     {state.TotalLost,6}", ConsoleColor.Red);
        Console.WriteLine();
        Ui.PrintCentered($"⭐ Репутация: {state.Reputation}/{state.MaxReputation}", ConsoleColor.Green);
        Ui.PrintCentered(Ui.ReputationBar(state.Reputation, state.MaxReputation, 40), ConsoleColor.Green);
        Console.WriteLine();
        Ui.PrintCentered($"📅 День: {state.Day - 1}/{state.DaysToSurvive}", ConsoleColor.White);
        Ui.PrintCentered($"📋 Открыто позиций:      {state.TotalUnlocked,6}", ConsoleColor.Cyan);
        Ui.PrintCentered($"🛠️  Куплено апгрейдов:    {state.TotalUpgrades,6}", ConsoleColor.Cyan);

        Ui.ShowMessage("Нажми любую клавишу...", ConsoleColor.Gray);
    }
}