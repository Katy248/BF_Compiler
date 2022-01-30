using System;
using System.Linq;

namespace BF_Compiler
{
    public class Warning : Error
    {
        public Warning(string text) => (this.Text, this.IsFounded, this.TypeName) = (text, false, ErrorTypes[1]);
    }
}
