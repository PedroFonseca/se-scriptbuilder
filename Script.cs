using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ScriptBuilder
{
    internal class Script
    {
        public string SourcePath { get; set; }
        public string ScriptFilePath { get; set; }
        public string ScriptName { get; private set; }

        public string Output { get; private set; }

        public string OutputWithUsings
        {
            get
            {
                return string.Format(defaultUsings, this.ScriptName, this.Output);
            }
        }

        private readonly List<string> filesImported = new List<string>();

        public Script(string sourcePath, string scriptFilePath)
        {
            this.Output = "";
            this.SourcePath = sourcePath;
            this.ScriptFilePath = scriptFilePath;
            this.ScriptName = scriptFilePath.Substring(scriptFilePath.LastIndexOf("\\") + 1).Replace(".cs", "");
            //this.outputDirectory = Path.Combine(this.sourceDirectory, "Compiled");
        }

        public Script Compile()
        {
            var sw = Stopwatch.StartNew();
            Console.Write("Compiling script: " + ScriptName + "...");

            this.Output = this.ProcessFile(this.ScriptFilePath);

            sw.Stop();
            Console.WriteLine(" Done in " + sw.ElapsedMilliseconds + " ms");

            return this;
        }

        private string ProcessFile(string filename)
        {
            if (!File.Exists(filename) || filesImported.IndexOf(filename) >= 0)
            {
                return "";
            }

            filesImported.Add(filename);

            string output = Environment.NewLine + "\t// File: " + Path.GetFileName(filename) + Environment.NewLine;
            List<string> lines = File.ReadAllLines(filename).ToList<string>();

            var scriptLines = GetRegion(lines, "SpaceEngineers");
            var usingsLines = GetRegion(lines, "Usings");

            if (scriptLines != null)
                output += String.Join(Environment.NewLine, scriptLines);

            if (usingsLines != null)
                output += ProcessUsings(usingsLines);

            output += Environment.NewLine;
            return output;
        }

        private List<string> GetRegion(List<string> lines, string region)
        {
            int startIndex = lines.FindIndex(x => x.Contains("#region " + region));
            if (startIndex < 0)
            {
                return null;
            }

            int endIndex = lines.FindIndex(startIndex, x => x.Contains("#endregion"));
            if (endIndex < 0)
            {
                return null;
            }

            int range = endIndex - startIndex - 1;
            if (range < 0)
            {
                range = 0;
            }

            return lines.GetRange(startIndex + 1, range);
        }

        private string ProcessUsings(List<string> usings)
        {
            var filenames = usings.Where(t => !string.IsNullOrEmpty(t.Trim())).Select(t => Path.Combine(this.SourcePath, GetFilenameFromUsing(t)));
            return String.Join(Environment.NewLine, filenames.Select(ProcessFile));
        }

        private string GetFilenameFromUsing(string usingLine)
        {
            return usingLine.Substring(usingLine.IndexOf("=") + 2).Replace(".", "\\").Replace(";", "") + ".cs";
        }

        private string defaultUsings = @"
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Ingame;
using VRageMath;

namespace SEScripts.Grids
{{
    public class {0}a : Skeleton
    {{
        {1}
    }}
}}";
    }
}