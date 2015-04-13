using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptBuilder
{
    class Script
    {
        private string sourceDirectory;
        private string outputDirectory;
        private string scriptName;
        private string output = "";

        public Script (string sourceDirectory, string outputDirectory, string scriptName) {
            this.sourceDirectory = Path.GetFullPath(sourceDirectory);
            this.outputDirectory = Path.GetFullPath(outputDirectory);

            this.scriptName = scriptName;
            if (String.IsNullOrEmpty(this.scriptName))
            {
                this.scriptName = Path.GetFileName(this.sourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            }
        }

        public void Compile()
        {
            // Special File - UserConfig.cs
            string userConfig = Path.Combine(this.sourceDirectory, "UserConfig.cs");
            if (File.Exists(userConfig))
            {
                this.output += this.ProcessFile(userConfig);
            }
            this.output += this.ProcessDirectory(this.sourceDirectory, 0);
        }

        public void Write()
        {
            string fullpath = Path.Combine(this.outputDirectory, this.scriptName);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }
            string filename = Path.Combine(fullpath, "Script.cs");
            File.WriteAllText(filename, this.output);
        }

        private string ProcessDirectory(string directory, int level)
        {
            if (level > 128)
            {
                return "";
            }

            string output = "";
            foreach (string dir in Directory.GetDirectories(directory))
            {
                output += this.ProcessDirectory(dir, level + 1);
            }

            string userConfig = Path.Combine(this.sourceDirectory, "UserConfig.cs");
            foreach (string file in Directory.GetFiles(directory, "*.cs"))
            {
                if (file == userConfig)
                {
                    continue;
                }
                output += this.ProcessFile(file);
            }

            return output;
        }

        private string ProcessFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return "";
            }

            string output = Environment.NewLine + "\t// File: " + Path.GetFileName(filename) + Environment.NewLine;
            List<string> lines = File.ReadAllLines(filename).ToList<string>();

            int startIndex = lines.FindIndex(x => x.Contains("#region SpaceEngineers"));
            if (startIndex < 0)
            {
                return "";
            }

            int endIndex = lines.FindIndex(startIndex, x => x.Contains("#endregion"));
            if (endIndex < 0)
            {
                return "";
            }

            int range = endIndex - startIndex - 1;
            if (range < 0)
            {
                range = 0;
            }

            output += String.Join(Environment.NewLine, lines.GetRange(startIndex + 1, range));

            output += Environment.NewLine;
            return output;
        }
    }
}
