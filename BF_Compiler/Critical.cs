using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF_Compiler
{
    internal class Critical : Error
    {
        public Critical(string text) => (this.Text, this.IsFounded, this.TypeName) = (text, false, ErrorTypes[0]);
    }
}
