using System;
using System.Threading.Tasks;

namespace Roseline
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // await ScriptExample.Exemple_EvaluateAsync_Simple();
            // await ScriptExample.Exemple_Avec_Lecture_Variableg();
            // await ScriptExample.Exemple_Create_Compile_MultiRun();
            // await ScriptExample.Exemple_Parametre_Reference();
            // await ScriptExample.Exemple_REPL_1();
            CompilationExample.Compile_And_Execute();
        }
    }
}
