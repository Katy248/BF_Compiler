using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF_Compiler
{
    public class Warning : Error
    {
        public Warning(string text) => (this.Text, this.IsFounded, this.TypeName) = (text, false, ErrorTypes[1]);
    }
}
