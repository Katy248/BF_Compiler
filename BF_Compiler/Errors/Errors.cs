using System;
using System.Collections.Generic;

namespace BF_Compiler
{
    public abstract class Error
    {
        public static List<Error> Errors = new List<Error>();
        protected readonly static string[] ErrorTypes = new string[] { "Critical", "Warning" };

        public string Text { get; protected set; }
        public bool IsFounded;
        protected string TypeName;

        public static void ResetAll()
        {
            foreach (Error Er in Errors) Er.Reset();
        }

        public static string Output()
        {
            string Text = "";
            foreach (Error Er in Errors)
            {
                Text += Er.IsFounded ? Er.TypeName + ": " + Er.Text + "\n" : "";
            }
            return Text;
        }
        public static bool FoundedCritical()
        {
            bool CrFound = false;
            foreach (Error Er in Errors)
            {
                CrFound &= (Er.IsFounded && Er.TypeName == ErrorTypes[0]);
            }
            return CrFound;
        }

        public Error() { Errors.Add(this); }

        public void Reset() => (this.IsFounded) = (false);

        public void Found() => (this.IsFounded) = (true);
    }
}
