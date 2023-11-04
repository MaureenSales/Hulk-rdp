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
            Interprete interprete = new Interprete();
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
                    List<ASTnode> stmts = parser.Parse();
                    // AstPrinter p = new AstPrinter();
                    // foreach(ASTnode s in stmts)p.Print_(s);
                    
                    foreach(ASTnode stmt in stmts)interprete.evaluate(stmt);
                }
                catch (System.Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    System.Console.WriteLine(e.Message);
                    //System.Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

    }
}