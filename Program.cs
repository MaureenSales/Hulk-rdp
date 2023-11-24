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
            //iniciar el interprete tal que las funciones sigan en contexto hasta que lo cierre con el comando exit
            Interprete interprete = new Interprete();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write(">>> ");
                Console.ForegroundColor = ConsoleColor.White;
                //leer de la  entrada el string (linea de codigo)
                string input = Console.ReadLine();
                if (input == "exit") break;

                try
                {
                    //iniciar el lexer pasando al constructor la linea de codigo
                    Lexer lexer = new Lexer(input);
                    //iniciar el parser pasando al constructor la instancia del lexer
                    Parser parser = new Parser(lexer);
                    //el parser devuelve una lista de nodos que son evaluados llamando a evaluate del interprete
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