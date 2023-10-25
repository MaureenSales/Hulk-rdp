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
                    Parser parser = new Parser(lexer);
                    List<Stmt> stmts = parser.Parse();
                    // AstPrinter p = new AstPrinter();
                    // foreach(Stmt s in stmts)p.Print(s);
                    Interprete interprete = new Interprete();
                    foreach(Stmt stmt in stmts)interprete.Execute(stmt);
                }
                catch (System.Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    System.Console.WriteLine(e.Message);
                    System.Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

    }
}