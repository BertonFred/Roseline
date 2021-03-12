using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInterfaceAdresse
    {
        void Init(string _Nom, string _Mail);
        string Nom { get; set; }
        string Mail { get; set; }
    }
}
