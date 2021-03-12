using System; 
using Interfaces;

namespace MyCompilation
{
    public class ClassACompiler : IInterfaceAdresse
    {
        public ClassACompiler()
        {
            Console.WriteLine($"Execute : MyCompilation.ClassACompiler()");
        }

        public void Init(string _Nom, string _Mail)
        {
            Console.WriteLine($"Execute : MyCompilation.Init({_Nom},{_Mail})");
            Nom = _Nom;
            Mail = _Mail;
        }

        public string Nom { get; set; }
        public string Mail { get; set; }
    }
}


