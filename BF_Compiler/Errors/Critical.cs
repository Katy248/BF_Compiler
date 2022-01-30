using System;
using System.Linq;

namespace BF_Compiler
{
    internal class Critical : Error
    {
        public Critical(string text) => (this.Text, this.IsFounded, this.TypeName) = (text, false, ErrorTypes[0]);
    }
}
