// ============================================================
// Ui.cs — интерфейс консольного кафе-симулятора
// ============================================================

namespace CafeConsole
{
    public static class Ui
    {
        private const int InnerWidth = 62;

        // ============================================================
        // ЛОГОТИП
        // ============================================================
        public static void DrawLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintCentered("╔══════════════════════════════════════════════════════════════╗");
            PrintCentered("║                                                              ║");
            PrintCentered("║              ☕   C A F E   S I M U L A T O R   ☕            ║");
            PrintCentered("║                                                              ║");
            PrintCentered("║                       версия 1.0                              ║");
            PrintCentered("║                                                              ║");
            PrintCentered("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        // ============================================================
        // ШАПКА ИГРЫ
        // ============================================================
        public static void DrawGameHeader(GameState state)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            PrintCentered("╔══════════════════════════════════════════════════════════════╗");
            Console.ResetColor();

            string dayText = $"📅 День {state.Day}/{state.DaysToSurvive}";
            string moneyText = $"💰 {state.Money} руб.";
            string repText = $"⭐ {state.Reputation}/{state.MaxReputation}";

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("║");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {dayText,-18}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  {moneyText,-20}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"  {repText,-18}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("║");

            Console.Write("║  ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Репутация: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(ReputationBar(state.Reputation, state.MaxReputation, 40));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"  {state.Reputation,3}/{state.MaxReputation,-3}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  ║");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            PrintCentered("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        // ============================================================
        // ШАПКА РАБОЧЕГО ДНЯ
        // ============================================================
        public static void DrawDayHeader(int day, int maxDays, int money, int rep, int maxRep, int current, int total)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            PrintCentered("╔══════════════════════════════════════════════════════════════╗");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("║  ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"📅 День {day}/{maxDays}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("  │  ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"💰 {money} руб.");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("  │  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"⭐ {rep}/{maxRep}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  ║");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("║  ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Клиенты: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(ProgressBar(current, total, 40));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"  {current,2}/{total,-2}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  ║");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            PrintCentered("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        // ============================================================
        // КАРТОЧКА КЛИЕНТА
        // ============================================================
        public static void DrawCustomerCard(string itemName, int price, int cost, int profit)
        {
            int padLeft = Math.Max(0, (Console.WindowWidth - InnerWidth - 2) / 2);
            string pad = new string(' ', padLeft);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(pad + "┌" + new string('─', InnerWidth) + "┐");

            string order = $"👤 Клиент заказывает: {itemName}";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(pad + "│ " + order.PadRight(InnerWidth - 1) + "│");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(pad + "├" + new string('─', InnerWidth) + "┤");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(pad + "│ " + $"💵 Цена:            {price,4} руб.".PadRight(InnerWidth - 1) + "│");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(pad + "│ " + $"📦 Себестоимость:   {cost,4} руб.".PadRight(InnerWidth - 1) + "│");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(pad + "│ " + $"💰 Прибыль:         {profit,4} руб.".PadRight(InnerWidth - 1) + "│");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(pad + "└" + new string('─', InnerWidth) + "┘");
            Console.ResetColor();
        }

        // ============================================================
        // ИНТЕРАКТИВНОЕ МЕНЮ СО СТРЕЛКАМИ (исправлено)
        // ============================================================
        public static int ShowMenu(string title, string[] options)
        {
            int selected = 0;

            while (true)
            {
                // Полная очистка — надёжно и просто
                Console.Clear();

                DrawLogo();
                Console.WriteLine();
                PrintCentered($"─── {title} ───", ConsoleColor.Cyan);
                Console.WriteLine();

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                        PrintCentered($"  ▶  {options[i]}  ◀  ", ConsoleColor.Yellow);
                    else
                        PrintCentered($"     {options[i]}     ", ConsoleColor.Gray);
                }

                Console.WriteLine();
                PrintCentered("↑ ↓ — выбор   Enter — подтвердить", ConsoleColor.DarkGray);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + options.Length) % options.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % options.Length;
                else if (key == ConsoleKey.Enter)
                    return selected;
            }
        }

        // ============================================================
        // ПРОГРЕСС-БАРЫ
        // ============================================================
        public static string ReputationBar(int rep, int max, int width = 20)
        {
            if (max <= 0) max = 1;
            int filled = (int)Math.Round((double)rep / max * width);
            filled = Math.Clamp(filled, 0, width);
            return new string('█', filled) + new string('░', width - filled);
        }

        public static string ProgressBar(int current, int total, int width = 20)
        {
            if (total <= 0) total = 1;
            int filled = (int)Math.Round((double)current / total * width);
            filled = Math.Clamp(filled, 0, width);
            return new string('▓', filled) + new string('░', width - filled);
        }

        // ============================================================
        // УТИЛИТЫ
        // ============================================================
        public static void PrintCentered(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            int pad = (Console.WindowWidth - text.Length) / 2;
            if (pad < 0) pad = 0;

            Console.ForegroundColor = color;
            Console.WriteLine(new string(' ', pad) + text);
            Console.ResetColor();
        }

        public static void ShowMessage(string msg, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.WriteLine();
            PrintCentered(msg, color);
            Console.WriteLine();
            PrintCentered("Нажми любую клавишу...", ConsoleColor.DarkGray);
            Console.ReadKey(true);
        }

        public static void SafeClear()
        {
            try { Console.Clear(); } catch { }
        }

        public static void DrawSeparator()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrintCentered(new string('─', Math.Min(Console.WindowWidth - 4, 60)));
            Console.ResetColor();
        }

        public static void DrawSection(string title, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.WriteLine();
            PrintCentered($"─── {title} ───", color);
            Console.WriteLine();
        }
    }
}
