using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptBuilder
{
    internal class Script
    {
        private string sourceDirectory;
        private string outputDirectory;
        private string scriptName;
        private string output = "";
        private readonly List<string> filesImported = new List<string>();

        public Script(string sourceDirectory, string scriptName)
        {
            this.sourceDirectory = Path.GetFullPath(sourceDirectory);
            this.outputDirectory = Path.Combine(this.sourceDirectory, "Compiled");

            this.scriptName = scriptName;
        }

        public void Compile(bool withUsings)
        {
            // Get the script
            string scriptFile = Path.Combine(this.sourceDirectory, "Scripts", this.scriptName + ".cs");
            if (File.Exists(scriptFile))
            {
                this.output += this.ProcessFile(scriptFile);

                if (withUsings)
                {
                    this.output = string.Format(defaultUsings, this.scriptName, this.output);
                }
            }
        }

        public void Write()
        {
            if (!Directory.Exists(this.outputDirectory))
            {
                Directory.CreateDirectory(this.outputDirectory);
            }
            string filename = Path.Combine(this.outputDirectory, string.Format("{0}.cs", this.scriptName));
            File.WriteAllText(filename, this.output);
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
            var filenames = usings.Where(t => !string.IsNullOrEmpty(t.Trim())).Select(t => Path.Combine(this.sourceDirectory, GetFilenameFromUsing(t)));
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