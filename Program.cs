using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScriptBuilder
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if(args.Length == 0)
                {
                    Console.WriteLine("Argument needed with path for runnning scripts!");
                    Console.WriteLine("Failed!");
                    return 1;
                }
                string sourceDir = args[0];
                new ScriptSolution(sourceDir).CompileScripts();
                Console.WriteLine("Scripts compiled successfully!");
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}