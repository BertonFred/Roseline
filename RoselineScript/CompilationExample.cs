using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

using Interfaces;

namespace Roseline
{
    public class CompilationExample
    {
        static public void Compile_And_Execute()
        {
            // Lecture dans un string du fichier avec le code de la classe
            string sCodeSource = File.ReadAllText(@"..\..\ClassACompiler.cs");
            
            // Parsing du code, creation du syntax tree
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sCodeSource);

            // Creation des references sur les assembly utilisée
            string PathAssembly_System = typeof(object).Assembly.Location;
            string PathAssembly_Interfaces = typeof(IInterfaceAdresse).Assembly.Location;
            var AssemblyRef_System = MetadataReference.CreateFromFile(PathAssembly_System);
            var AssemblyRef_Interfaces = MetadataReference.CreateFromFile(PathAssembly_Interfaces);
            
            // Compilation du code
            var CompilationOption = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create("MyCompilation",
                                                        syntaxTrees: new[] { tree },
                                                        references: new[] { AssemblyRef_System, AssemblyRef_Interfaces },
                                                        options: CompilationOption);

            // Creation de l'assembly
            Console.WriteLine("COMPILATION");
            EmitResult emitResult = compilation.Emit("output.dll");

            // S'il y a des erreurs de compilation le dire
            if (emitResult.Success == false)
                Console.WriteLine("Compilation KO");
            else
                Console.WriteLine("Compilation OK");

            // Afficher les resultats de la compilation.
            // il peut s'agir d'erreur ou de warning
            Console.WriteLine("Résultat de la compilation");
            foreach (Diagnostic diagnostic in emitResult.Diagnostics)
                Console.WriteLine(diagnostic.ToString());

            Console.WriteLine("Appuyez sur une touche pour charger l'assembly");
            Console.ReadKey();

            // Chargement dynamique de l'assembly que l'on viens de compiler
            Console.WriteLine("Chargment de assembly : output.dll");
            var myAssembly = Assembly.LoadFrom("output.dll");

            // CReation d'une instance de la classe : MyCompilation.ClassACompiler
            Console.WriteLine("Creation d'une instance de la classe : MyCompilation.ClassACompiler");
            Type typeMyClass = myAssembly.GetType("MyCompilation.ClassACompiler");
            IInterfaceAdresse myclass = (IInterfaceAdresse)Activator.CreateInstance(typeMyClass);
            
            // invocation des methodes avec l'interface contenu du programme appelant
            myclass.Init("BERTON", "frederic.berton@capgemini.com");
            Console.WriteLine($"Nom {myclass.Nom} Mail {myclass.Mail}");

            Console.WriteLine("Appuyez sur une touche pour quitter");
            Console.ReadKey();
        }
    }
}
