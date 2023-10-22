using System.ComponentModel;
using System.IO;
namespace Hulk
{
    public class Program
    {
        public static void Main()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Clear();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write(">>> ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine();
                if (input == "exit") break;
                try
                {
                    Lexer lexer = new Lexer(input);
                    foreach (var t in lexer.Tokens)
                        System.Console.WriteLine($"{t.Type}  {t.Lexeme}");
                }
                catch (System.Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    System.Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

    }
}