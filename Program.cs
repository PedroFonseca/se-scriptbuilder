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
        private static void Main(string[] args)
        {
            Program.DisplayBanner();

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "--help":
                    case "-h":
                    case "/?":
                        Program.Help();
                        return;

                    default:
                        break;
                }
            }

            Program.ProcessScript(args);
        }

        private static void ProcessScript(string[] args)
        {
            string sourceDir = Program.GetParam(args, 0);
            string scriptName = Program.GetParam(args, 1);
            bool withUsings = Program.GetParam(args, 2) == "withUsings";

            Console.WriteLine(" Source: " + sourceDir);
            Console.WriteLine(" Script Name: " + scriptName);

            Script script = new Script(sourceDir, scriptName);
            script.Compile(withUsings);
            script.Write();
        }

        private static void Help()
        {
            Console.WriteLine("Usage: ScriptBuilder.exe <input> [scriptname]");
        }

        private static void DisplayBanner()
        {
            Console.WriteLine(Program.GetVersion() + Environment.NewLine);
            Console.WriteLine("Michael Smith <me@murray-mint.co.uk>");
            Console.WriteLine("https://github.com/murray-mint/se-scriptbuilder");
            Console.WriteLine("This software is released under the MIT License" + Environment.NewLine);
        }

        private static string GetVersion()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            AssemblyTitleAttribute title = (AssemblyTitleAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute));
            return String.Format("{0} {1}.{2}.{3}", title.Title, version.Major.ToString(), version.Minor.ToString(), version.Revision.ToString());
        }

        private static string GetParam(string[] args, int index)
        {
            string defaultValue = null;
            if (args.Length > index)
            {
                return args[index];
            }
            return defaultValue;
        }
    }
}