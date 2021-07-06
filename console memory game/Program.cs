using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_memory_game
{
    class Program
    {
        private static char[,] CreateTable(int table_size)
        {
            char[,] table = new char[table_size, table_size];

            char[] cards = new char[table.Length];
            char card = 'A';
            for (int i = 0; i < cards.Length; i += 2)
            {
                cards[i] = card;
                cards[i + 1] = card++;
            }
            char[] shuffled_cards = cards.OrderBy(n => Guid.NewGuid()).ToArray();

            int index = 0;
            for (int y = 0; y < table_size; y++)
            {
                for (int x = 0; x < table_size; x++)
                {
                    table[x, y] = shuffled_cards[index++];
                }
            }

            return table;
        }


        private static void PrintTable(char[,] table, int column_1 = -1, int row_1 = -1, int column_2 = -1, int row_2 = -1)
        {
            string[] empty = { " ˍˍˍˍˍ ", "|\\   /|", "| \\ / |", "|  ×  |", "| / \\ |", "|/   \\|", " ˉˉˉˉˉ " };
            string[] full = { " ˍˍˍˍˍ ", "|0    |", "|  &  |", "| & & |", "|  &  |", "|    0|", " ˉˉˉˉˉ " };

            for (int y = 0; y < table.GetLength(0); y++)
            {
                for (int i = 0; i < empty.Length; i++)
                {
                    for (int x = 0; x < table.GetLength(1); x++)
                    {
                        if ((y == row_1 && x == column_1) || (y == row_2 && x == column_2))
                        {
                            if (column_2 > -1 && table[column_1, row_1] == table[column_2, row_2])
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            else if (column_2 > -1)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            Console.Write(full[i].Replace("0", table[x, y].ToString()) + " ");
                            Console.ResetColor();
                        }
                        else if (table[x, y] == '0')
                        {
                            Console.Write("        ");
                        }
                        else
                        {
                            Console.Write(empty[i] + " ");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }


        private static (int, int) GetUserChoice(char[,] table, int round, bool player_1_turn, int max, int column_1 = -1, int row_1 = -1)
        {
            int x, y;
            while (true)
            {
                Console.Write($"Player {(player_1_turn ? "1" : "2")}, enter {round}{(round == 1 ? "st" : "nd")} row number (1-{max}): ");
                char input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                while (true)
                {
                    if (Int32.TryParse(input.ToString(), out y) && y >= 1 && y <= max)
                    {
                        break;
                    }

                    Console.Write($"Wrong input, enter a number between 1 to {max}: ");
                    input = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                }

                Console.Write($"Player {(player_1_turn ? "1" : "2")}, enter {round}{(round == 1 ? "st" : "nd")} column number (1-{max}): ");
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                while (true)
                {
                    if (Int32.TryParse(input.ToString(), out x) && x >= 1 && x <= max)
                    {
                        break;
                    }
                    else
                    {
                        Console.Write($"Wrong input, enter a number between 1 to {max}: ");
                        input = Console.ReadKey().KeyChar;
                        Console.WriteLine();
                    }
                }

                if (x - 1 == column_1 && y - 1 == row_1)
                {
                    Console.WriteLine($"You have selected {y}, {x} for 1st choice, try again :(.");
                }
                else if (table[x - 1, y - 1] == '0')
                {
                    Console.WriteLine($"{y}, {x} is empty, try again.");
                }
                else
                {
                    break;
                }
            }

            (int x, int y) user_input = (x - 1, y - 1);
            return user_input;
        }


        private static void PrintStatus(int player_1_score, int player_2_score)
        {
            Console.Clear();

            if (player_1_score > player_2_score)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\nPlayer 1: {player_1_score}.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\tPlayer 2: {player_2_score}.");
            }
            else if (player_2_score > player_1_score)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\nPlayer 1: {player_1_score}.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\tPlayer 2: {player_2_score}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\nPlayer 1: {player_1_score}.\tPlayer 2: {player_2_score}.");
            }

            Console.ResetColor();
            Console.WriteLine();
        }


        private static bool GameOver(char[,] table)
        {
            for (int y = 0; y < table.GetLength(0); y++)
            {
                for (int x = 0; x < table.GetLength(1); x++)
                {
                    if (table[x, y] != '0')
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        static void Main(string[] args)
        {
            char[] options = { '1', '2', '3' };
            char size_input;
            do
            {
                Console.Write("choose table size(1-3): (1: 2*2, 2: 4*4, 3: 6*6) ");
                size_input = Console.ReadKey().KeyChar;
                Console.WriteLine();
            } while (Array.IndexOf(options, size_input) == -1);

            int table_size = 0;
            switch (size_input)
            {
                case '1':
                    table_size = 2;
                    break;
                case '2':
                    table_size = 4;
                    break;
                case '3':
                    table_size = 6;
                    break;
            }

            char[,] table = CreateTable(table_size);

            bool player_1_turn = true;
            int player_1_score = 0;
            int player_2_score = 0;
            while (!GameOver(table))
            {
                Console.WriteLine($"\nPlayer {(player_1_turn ? "1" : "2")}, Press any key to play.");
                Console.ReadKey();

                PrintStatus(player_1_score, player_2_score);
                PrintTable(table);

                (int x, int y) user_input = GetUserChoice(table, 1, player_1_turn, table_size);
                int column_1 = user_input.x;
                int row_1 = user_input.y;

                PrintStatus(player_1_score, player_2_score);
                PrintTable(table, column_1, row_1);

                user_input = GetUserChoice(table, 2, player_1_turn, table_size, column_1, row_1);
                int column_2 = user_input.x;
                int row_2 = user_input.y;

                PrintStatus(player_1_score, player_2_score);
                PrintTable(table, column_1, row_1, column_2, row_2);

                if (table[column_1, row_1] == table[column_2, row_2])
                {
                    table[column_1, row_1] = '0';
                    table[column_2, row_2] = '0';

                    if (player_1_turn)
                    {
                        player_1_score++;
                    }
                    else
                    {
                        player_2_score++;
                    }
                }
                else
                {
                    player_1_turn = !player_1_turn;
                }
            }

            if (player_1_score == player_2_score)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("------ TIE! ------");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--- Player {(player_1_score > player_2_score ? "1" : "2")} Won!!! ---");
            }
            Console.ResetColor();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();

            //by: t.me/yehuda100
        }
    }
}
