using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptBuilder
{
    public class ScriptSolution
    {
        private readonly string sourceDirectory;
        private readonly string outputDirectory;

        public ScriptSolution(string sourceDirectory)
        {
            this.sourceDirectory = Path.GetFullPath(sourceDirectory);
            this.outputDirectory = Path.Combine(this.sourceDirectory, "Compiled");
        }

        public void CompileScripts()
        {
            if (!Directory.Exists(this.outputDirectory))
            {
                Directory.CreateDirectory(this.outputDirectory);
            }
            GetScriptFiles().Select(t => new Script(this.sourceDirectory, t).Compile()).ToList().ForEach(script =>
            {
                string filename = Path.Combine(this.outputDirectory, string.Format("{0}", script.ScriptName));
                string filenameWithUsings = Path.Combine(this.outputDirectory, string.Format("{0}.cs", script.ScriptName));
                File.WriteAllText(filename, script.Output);
                File.WriteAllText(filenameWithUsings, script.OutputWithUsings);
            });
        }

        private IEnumerable<string> GetScriptFiles()
        {
            Console.WriteLine("Compiling scripts on directory: " + Path.Combine(this.sourceDirectory, "Scripts"));
            return Directory.GetFiles(Path.Combine(this.sourceDirectory, "Scripts"), "*.cs");
        }
    }
}