using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces;

namespace InterfacesImplementation
{
    public class ClassAdresse : IInterfaceAdresse
    {
        public ClassAdresse()
        {
        }

        public void Init(string _Nom, string _Mail)
        {
            Nom = _Nom;
            Mail = _Mail;
        }

        public string Nom { get; set; }
        public string Mail { get; set; }
    }
}
