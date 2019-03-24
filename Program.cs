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
                string sourceDir = args[0];
                new ScriptSolution(sourceDir).CompileScripts();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}