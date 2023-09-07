using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading;
//         DAILY MISSIONS
// Make menu after lose
// Make color on difference score
// Delete magic numbers
//          WEEKLY MISSIONS 
// Make new levels and switching levels
// Ветка луза и победы( луз -> меню ) ( вин -> некст левел )
// Расчет счета в форе с помощью метода(возможно по чарам точкам)
namespace Snake
{
    //this is my ideas
    // функция ескейпа, левела но это не сильно имеет смысл так как я буду менять тут половину изза моргания, просто тренируюсь пока
    // сделать жизни, сделать в бут меню старт игры в хард режиме(без жизней) либо для слабочков в легком с жизнями, не знаю нужно ли это 
    // но можно сделать возврат в бут меню после проигрыша, разбить по ООП
    internal class Program
    {
        static void Main()
        {
            bool startGame = false;
            Console.CursorVisible = false;

            char[,] map = MapUtility.ReadMap("1lvl.txt");
            var pressedKey = new ConsoleKeyInfo('w', ConsoleKey.W, false, false, false);

            Task.Run(() =>
            {
                while (true)
                {
                    pressedKey = Console.ReadKey();
                }
            });

            int snakeX = 1;
            int snakeY = 1;
            int score = 0;

            int intialDelayMilliseconds = 500;
            int delayMilliseconds = intialDelayMilliseconds;

            string snake = "_";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(50, 1);
            Console.WriteLine("Snake");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(50, 5);
            Console.WriteLine("Start");
            Console.SetCursorPosition(50, 6);
            Console.WriteLine("Exit");
            pressedKey = Console.ReadKey();


            string buttonName = "Start";
            Menu BootMenu = new(ref pressedKey, ref startGame, ref buttonName);

            Menu EscapeMenu = new(ref pressedKey, ref startGame, ref buttonName);


            while (startGame)
            {
                if (pressedKey.Key == ConsoleKey.Escape && startGame != false)
                {
                    startGame = false;
                    Console.Clear();
                    buttonName = "Resume";
                    EscapeMenu.Show(ref pressedKey, ref startGame, ref buttonName);
                }
                Console.Clear();

                Controls.HandleInput(pressedKey, ref snakeX, ref snakeY, map, ref score, ref snake, ref delayMilliseconds);

                Console.ForegroundColor = ConsoleColor.Blue;
                MapUtility.DrawMap(map);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(snakeX, snakeY); //x, y
                Console.Write(snake);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(13, 0);
                Console.WriteLine($"Score: {score} ");

                Console.SetCursorPosition(36, 0);
                Console.WriteLine("$ - SpeedBoost");

                Console.SetCursorPosition(36, 1);
                Console.WriteLine("& - SlowBoost");

                Console.SetCursorPosition(36, 2);
                Console.WriteLine(". - Score Points");

                Console.SetCursorPosition(36, 3);
                Console.WriteLine("@ - Finish if u have all Points");

                Console.SetCursorPosition(36, 4);
                Console.WriteLine("Borders will kill you, be a carefull!");

                Console.SetCursorPosition(6, 10);
                Console.WriteLine("Just Beta, now thinking");

                Thread.Sleep(delayMilliseconds);
            }
        }
    }

    class Controls
    {
        public static void HandleInput(ConsoleKeyInfo pressedKey, ref int snakeX, ref int snakeY, char[,] map, ref int score, ref string snake, ref int delayMilliseconds)
        {
            int[] direction = GetDirection(pressedKey);
            int nextSnakePositionX = snakeX + direction[0];
            int nextSnakePositionY = snakeY + direction[1];

            char nextCell = map[nextSnakePositionX, nextSnakePositionY];

            if (nextCell == '$')
            {
                delayMilliseconds -= 100;
                map[nextSnakePositionX, nextSnakePositionY] = ' ';
            }
            if (nextCell == '&')
            {
                delayMilliseconds += 100;
                map[nextSnakePositionX, nextSnakePositionY] = ' ';
            }

            if (nextCell == ' ' || nextCell == '.')
            {
                snakeX = nextSnakePositionX;
                snakeY = nextSnakePositionY;
                ChangeSnakesRoad(ref snake, pressedKey);
                if (nextCell == '.')
                {
                    score++;
                    delayMilliseconds -= 8;
                    if (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.DownArrow)
                    {
                        snake = "|";
                    }

                    else if (pressedKey.Key == ConsoleKey.RightArrow || pressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        snake = "_";
                    }
                    map[nextSnakePositionX, nextSnakePositionY] = ' ';
                }
            }
            else if (nextCell == '|' || nextCell == '_' || nextCell == '#')
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(53, 5);
                Console.WriteLine("You lose");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            if (score < 42 && nextCell == '@')
            {
                int scoreDifference = 42 - score;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(45, 4);
                Console.WriteLine($"You need a {scoreDifference} points for win");
                Thread.Sleep(2000);
                Console.Clear();
            }
        }

        public static void ChangeSnakesRoad(ref string snake, ConsoleKeyInfo pressedKey)
        {
            if (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.DownArrow)
                snake = string.Concat("|", snake.AsSpan(1));

            else if (pressedKey.Key == ConsoleKey.LeftArrow || pressedKey.Key == ConsoleKey.RightArrow)
                snake = string.Concat("_", snake.AsSpan(1));
        }

        public static int[] GetDirection(ConsoleKeyInfo pressedKey)
        {
            int[] direction = { 0, 0 };

            if (pressedKey.Key == ConsoleKey.UpArrow)
                direction[1] -= 1;
            else if (pressedKey.Key == ConsoleKey.DownArrow)
                direction[1] += 1;
            else if (pressedKey.Key == ConsoleKey.LeftArrow)
                direction[0] -= 1;
            else if (pressedKey.Key == ConsoleKey.RightArrow)
                direction[0] += 1;

            return direction;
        }
    }

    class MapUtility
    {
        //Providing file info for the map, can be used in generating a new level map, for example.

        public static string? Map(string fileName)
        {
            try
            {
                return File.ReadAllText(fileName);
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found, is a file valid?");
                return null;
            }

            catch (IOException)
            {
                Console.WriteLine("An I/O (input, output) error occurred while reading the file.");
                return null;
            }
        }

        public static int GetMaxLengthOfLine(string[] lines)
        {
            int maxLength = lines[0].Length;

            foreach (var line in lines)
                if (line.Length > maxLength)
                    maxLength = line.Length;

            return maxLength;
        }

        public static char[,] ReadMap(string fileName)
        {
            string[] file = File.ReadAllLines(fileName);

            char[,] map = new char[GetMaxLengthOfLine(file), file.Length];

            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    map[x, y] = file[y][x];

            return map;
        }

        public static void DrawMap(char[,] map)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                    Console.Write(map[x, y]);

                Console.Write("\n");
            }
        }
    }

    class Menu
    {
        public Menu(ref ConsoleKeyInfo pressedKey, ref bool startGame, ref string buttonName)
        {
            Show(ref pressedKey, ref startGame, ref buttonName);
        }

        public void Show(ref ConsoleKeyInfo pressedKey, ref bool startGame, ref string buttonName)
        {
            while (true)
            {
                if (!startGame)
                {
                    if (pressedKey.Key == ConsoleKey.UpArrow)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(50, 1);
                        Console.WriteLine("Snake");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(50, 5);
                        Console.WriteLine(buttonName);
                        Console.SetCursorPosition(70, 5);
                        Console.WriteLine("Selected*");
                        Console.SetCursorPosition(50, 6);
                        Console.WriteLine("Exit");
                        Console.ReadKey();
                        if (pressedKey.Key == ConsoleKey.Enter)
                        {
                            startGame = true;
                            Console.Clear();
                        }
                    }

                    else if (pressedKey.Key == ConsoleKey.DownArrow)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(50, 1);
                        Console.WriteLine("Snake");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(50, 6);
                        Console.WriteLine("Exit");
                        Console.SetCursorPosition(70, 6);
                        Console.WriteLine("Selected*");

                        Console.SetCursorPosition(50, 5);
                        Console.WriteLine(buttonName);
                        Console.ReadKey();

                        if (pressedKey.Key == ConsoleKey.Enter)
                            Exit();
                    }
                }
                else
                    break;
            }
        }

        public static void Exit()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(50, 6);
            Console.WriteLine("Bye");
            Environment.Exit(0);
        }
    }
}