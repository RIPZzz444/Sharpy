using System;
using System.IO;
using System.Text;

namespace CatAndMouseGame
{
    public class Game
    {
        public static string InputFile = "ChaseData.txt";
        public static string OutFile = "PursuitLog.txt";

        public int size;
        public Player cat;
        public Player mouse;
        public GameState state;

        private StringBuilder logBuilder = new StringBuilder();

        public Game(int size)
        {
            this.size = size;
            cat = new Player("Cat");
            mouse = new Player("Mouse");
            state = GameState.Start;
        }

        public void Run()
        {
            if (!File.Exists(InputFile))
            {
                Console.WriteLine($"Ошибка: Входной файл {InputFile} не найден.");
                return;
            }

            string[] lines = File.ReadAllLines(InputFile);
            if (lines.Length == 0) return;

            if (int.TryParse(lines[0].Trim(), out int parsedSize))
            {
                this.size = parsedSize;
            }

            logBuilder.AppendLine("Cat and Mouse");
            logBuilder.AppendLine();
            logBuilder.AppendLine("Cat Mouse  Distance");
            logBuilder.AppendLine("-------------------");

            int lineIndex = 1;
            while (state != GameState.End && lineIndex < lines.Length)
            {
                string line = lines[lineIndex].Trim();
                lineIndex++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                char command = parts[0][0];

                if (command == 'M' || command == 'C')
                {
                    int steps = int.Parse(parts[1]);
                    DoMoveCommand(command, steps);
                }
                else if (command == 'P')
                {
                    DoPrintCommand();
                }

                if (cat.state == State.Playing && mouse.state == State.Playing && cat.location == mouse.location)
                {
                    cat.state = State.Winner;
                    mouse.state = State.Loser;
                    state = GameState.End;
                }
            }

            if (state != GameState.End)
            {
                if (mouse.state == State.Playing) mouse.state = State.Winner;
                if (cat.state == State.Playing) cat.state = State.Loser;
                state = GameState.End;
            }

            logBuilder.AppendLine("-------------------");
            logBuilder.AppendLine();
            logBuilder.AppendLine();
            logBuilder.AppendLine($"Distance traveled:   Mouse    Cat");
            logBuilder.AppendLine($"                      {mouse.distanceTraveled,4}   {cat.distanceTraveled,4}");
            logBuilder.AppendLine();

            if (mouse.state == State.Loser)
            {
                logBuilder.AppendLine($"Mouse caught at: {mouse.location}");
            }
            else
            {
                logBuilder.AppendLine("Mouse evaded Cat");
            }

            File.WriteAllText(OutFile, logBuilder.ToString());
            Console.WriteLine(logBuilder.ToString());
        }

        private void DoMoveCommand(char command, int steps)
        {
            switch (command)
            {
                case 'M':
                    mouse.Move(steps, size);
                    break;
                case 'C':
                    cat.Move(steps, size);
                    break;
            }
        }

        private void DoPrintCommand()
        {
            string catLocStr = (cat.state != State.NotInGame) ? cat.location.ToString().PadLeft(3) : " ??";
            string mouseLocStr = (mouse.state != State.NotInGame) ? mouse.location.ToString().PadLeft(5) : "   ??";

            if (cat.state != State.NotInGame && mouse.state != State.NotInGame)
            {
                int distance = GetDistance();
                logBuilder.AppendLine($"{catLocStr}{mouseLocStr}      {distance,4}");
            }
            else
            {
                logBuilder.AppendLine($"{catLocStr}{mouseLocStr}");
            }
        }

        private int GetDistance()
        {
            int diff = Math.Abs(cat.location - mouse.location);
            return Math.Min(diff, size - diff);
        }
    }
}