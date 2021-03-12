using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using InterfacesImplementation;
using Interfaces;

namespace Roseline
{
    public class GlobalContext 
    {
        public ClassAdresse ClassAdresseGlobal { get; set; }
        public int IntGlobalValue { get; set; }
    }

    public class ScriptExample
    {
        public static async Task Exemple_EvaluateAsync_Simple()
        {
            try
            {
                string Expression = "int a = 10; " +
                               "int b = 2; " +
                               "int c; " +
                               "c = a * b; " +
                               "return c > a;";
                object objResult = await CSharpScript.EvaluateAsync(code: Expression,
                                                            options: null,
                                                            globals: null);

                Console.WriteLine($"Expression = {Expression}");
                Console.WriteLine($"objResult  = {objResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
            }

            Console.WriteLine("appuyez sur une touche");
            Console.ReadKey();
        }

        public static async Task Exemple_Avec_Lecture_Variableg()
        {
            try
            {
                string Input = "int a = 10; " +
                               "int b = 2; " +
                               "int c; " +
                               "c = a * b;";
                ScriptState<object> state = await CSharpScript.RunAsync(code: Input,
                                                            options: null,
                                                            globals: null);

                if (state.ReturnValue != null)
                    Console.WriteLine(state.ReturnValue.ToString());
                else
                    Console.WriteLine("result is null");

                foreach (var v in state.Variables)
                    Console.WriteLine($"{v.Type} {v.Name} = {v.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
            }

            Console.WriteLine("appuyez sur une touche");
            Console.ReadKey();
        }

        public static async Task Exemple_Create_Compile_MultiRun()
        {
            try
            {
                string Input = "int a = 10; " +
                               "int b = 2; " +
                               "int Mul = a * b; "+
                               "return Mul > a;";
                // compilation
                Script script = CSharpScript.Create(code: Input, options: null, globalsType: null);
                var resCompile = script.Compile();
                // Affichage des erreurs de compilation 
                foreach (var diagnostic in resCompile)
                    Console.WriteLine(diagnostic.ToString());

                // 1 er execution
                ScriptState state = await script.RunAsync(globals: null);
                // Affichage du résultat de retour du script 
                if (state.ReturnValue != null)
                    Console.WriteLine(state.ReturnValue.ToString());
                else
                    Console.WriteLine("result is null");
                foreach (var v in state.Variables)
                    Console.WriteLine($"{v.Type} {v.Name} = {v.Value}");

                // 2 eme execution
                state = await script.RunAsync(globals: null);
                ScriptVariable varMul = state.GetVariable("Mul");
                Console.WriteLine($"{varMul.Type} {varMul.Name} = {varMul.Value}");

                // 3 eme Execution avec du code en plus, le context précédant reste valide
                state = await state.ContinueWithAsync("int Add; Add = a + b;");
                ScriptVariable varAdd = state.GetVariable("Add");
                Console.WriteLine($"{varAdd.GetType()} {varAdd.Name} = {varAdd.Value}");
                varMul = state.GetVariable("Mul");
                Console.WriteLine($"{varMul.GetType()} {varMul.Name} = {varMul.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
            }

            Console.WriteLine("appuyez sur une touche");
            Console.ReadKey();
        }

        public static async Task Exemple_Parametre_Reference()
        {
            try
            {
                GlobalContext globalContext = new GlobalContext
                {
                    ClassAdresseGlobal = new ClassAdresse() { Nom = "BERTON", Mail = "frederic.berton@capgemini.com" },
                    IntGlobalValue = 1000
                };
                string Input = //"using Interfaces;" +
                               //"using InterfacesImplementation;" +
                               "int a = 10; " +
                               "int b = 2; " +
                               "int c; " +
                               "c = IntGlobalValue;"+
                               "string snom = ClassAdresseGlobal.Nom;" +
                               "IInterfaceAdresse myInterfaceAdresse = ClassAdresseGlobal;" +
                               "string snom2 = myInterfaceAdresse.Nom;" +
                               "string stypeinterface = myInterfaceAdresse.GetType().ToString();" +
                               "string type = ClassAdresseGlobal.GetType().ToString();" +
                               "ClassAdresse myAdresse = new ClassAdresse() { Nom = \"ZORG\", Mail = \"zorg@capgemini.com\"};";

                Script script = CSharpScript.Create(code: Input,
                                                    options: ScriptOptions.Default
                                                    .WithReferences(typeof(IInterfaceAdresse).Assembly)
                                                    .WithReferences(typeof(ClassAdresse).Assembly)
                                                    .WithImports(new string[] { "Interfaces", "InterfacesImplementation" })
                                                    ,
                                                    globalsType: typeof(GlobalContext)); ;
                var resCompile= script.Compile();
                foreach (var diagnostic in resCompile)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
                ScriptState state = await script.RunAsync(globals: globalContext);

                if (state.ReturnValue != null)
                    Console.WriteLine(state.ReturnValue.ToString());
                else
                    Console.WriteLine("result is null");

                foreach (var v in state.Variables)
                    Console.WriteLine($"{v.Type} {v.Name} = {v.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
            }

            Console.WriteLine("appuyez sur une touche");
            Console.ReadKey();
        }

        public static async Task Exemple_REPL_1()
        {
            ScriptState<object> state = null;
            var evaluate = true;

            Console.WriteLine("REPL C#. La ligne de commande C#");

            while (evaluate)
            {
                Console.WriteLine("Votre code (exit, pour sortir, clear pour effacer l'ecran, dump liste des varaibles) C#>");
                var input = Console.ReadLine();

                // traitement des commandes spéciale
                if (input == "exit")
                {
                    evaluate = false;
                    break;
                }
                else if (input == "clear")
                {
                    Console.Clear();
                    continue;
                }
                else if (input == "dump")
                {
                    if (state != null)
                    {
                        foreach (var v in state.Variables)
                            Console.WriteLine($"{v.Type} {v.Name} = {v.Value}");
                    }
                    continue;
                }

                // Traitement du code C# en boucle REPL
                try
                {
                    if (state == null)
                        state = await CSharpScript.RunAsync(code: input, options: null, globals: null);
                    else
                        state = await state.ContinueWithAsync(code: input);
                    if (state.ReturnValue != null)
                        Console.WriteLine(state.ReturnValue.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception {ex.Message}");
                }
            }
        }

        public static async Task Exemple_REPL_3(string[] args)
        {
            ClassAdresse myClass1 = new ClassAdresse() { Nom = "BERTON", Mail = "frederic.berton@capgemini.com" };
            ScriptState<object> state = null;
            var evaluate = true;

            Console.WriteLine("Ligne de commande C#");

            while (evaluate)
            {
                Console.WriteLine("Write some C#>");
                var input = Console.ReadLine();

                if (input == "exit")
                {
                    evaluate = false;
                    break;
                }

                if (input == "clear")
                {
                    Console.Clear();
                    continue;
                }

                try
                {
                    if (state == null)
                        state = await CSharpScript.RunAsync(code: input,
                                                            options: null,
                                                            globals: myClass1);
                    else
                        state = await state.ContinueWithAsync(code: input);

                    if (state.ReturnValue != null)
                        Console.WriteLine(state.ReturnValue.ToString());
                    else
                        Console.WriteLine("result is null");
                    foreach (var v in state.Variables)
                    {
                        Console.WriteLine($"{v.Type} {v.Name} = {v.Value}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception {ex.Message}");
                }
            }
        }
    }
}
