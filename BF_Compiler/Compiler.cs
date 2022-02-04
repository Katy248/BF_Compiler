using System;
using System.Linq;
using System.Linq.Expressions;

/// Using examples
/// 
/// To get formatted output (with messages) use:
/// <code>
///     string FormattedOutput = Compilate(Text, Input);
/// </code>
/// 
/// If you want to get only output without formatting use:
/// <code>
///     Compilate(Text, Input);
///     string Output = BF.output;
/// </code>
/// 
/// If you want to get only messages use:
/// <code>
///     Compilate(Text, Input);
///     string Messages = Error.Output;
/// </code>
/// 

namespace BF_Compiler
{
    public static class BF
    {
        private static int[] Array { get; set; }
        public static int ArrayLength { get; set; } = 3000;
        public static string Output { get; private set; }
        static int currentElement;
        static int currentInputSymbol;
        
        static readonly string[] Status = new string[] { "Done with result:\n", "Stopped because of:\n" };
        const int MaxElementSize = int.MaxValue;
        const int MinElementSize = 0;

        public static string Compile(string text, string input)
        {
            Array = new int[ArrayLength];
            Array.Initialize();
            
            Output = "";
            currentElement = 0;
            currentInputSymbol = 0;

            Error.ResetAll();
            
            TextIdentificate(text, input);

            if (!Error.FoundedCritical()) 
                InterpretBySym(text, input);
                
            Array = null;
            GC.Collect();
            return Status[Error.FoundedCritical() ? (1) : (0)] 
                + (Error.FoundedCritical() ? (Output + "\n") : "") 
                    + Error.Output();//1-bad, 0-good
        }

        static void InterpretBySym(string text, string input)
	    {
            for (int symNum = 0; symNum < text.Length; symNum++)
                switch ((Commands)text[symNum])
                {
                    case Commands.Next:
                        if (currentElement < Array.Length - 1) currentElement++;
                        break;
                    case Commands.Previous:
                        if (currentElement > 0) currentElement--;
                        break;
                    case Commands.Plus:
                        if (Array[currentElement] < MaxElementSize) Array[currentElement]++;
                        break;
                    case Commands.Minus:
                        if (Array[currentElement] > MinElementSize) Array[currentElement]--;
                        break;
                    case Commands.Out:
                        Output += (char)Array[currentElement];
                        break;
                    case Commands.In:
                        if (currentInputSymbol < input.Length)
                        {
                            Array[currentElement] = input[currentInputSymbol];
                            currentInputSymbol++;
                        }
                        break;
                    case Commands.WhileStart:
                        if (Array[currentElement] == 0)
                        {
                            int cycleEnds = 1;
                            while (cycleEnds > 0 && symNum < text.Length)
                            {
                                symNum++;
                                if (text[symNum] == ']') cycleEnds--;
                                if (text[symNum] == '[') cycleEnds++;
                            }
                        }
                        break;
                    case Commands.WhileEnd:
                        if (Array[currentElement] > 0)
                        {
                            int cycleStarts = 1;
                            while (cycleStarts > 0 && symNum >= 0)
                            {
                                symNum--;
                                if (text[symNum] == '[') cycleStarts--;
                                if (text[symNum] == ']') cycleStarts++;
                            }
                        }
                        break;
                }
        }

        static void TextIdentificate(string Text, string Input)
        {
            //Input length checking
            if (Text.Count(f => f == ',') > Input.Length)
                ErShortInput.Found();

            //Cycle writing correctness checking
            (int cycleStart, int cycleEnd) = (0, 0);
            for (int i = 0; i < Text.Length; i++)
            {
                switch ((Commands)Text[i])
                {
                    case Commands.WhileStart:
                        cycleStart++;
                        break;
                    case Commands.WhileEnd:
                        cycleEnd++;
                        break;
                }
                if (cycleEnd > cycleStart) ErEarlyCycleCloseing.Found();
            }
            if (cycleEnd < cycleStart) ErCycleWithNoCloseing.Found();
        }
        enum Commands
        {
            Next = '>',
            Previous = '<',
            Plus = '+',
            Minus = '-',
            Out = '.',
            In = ',',
            WhileStart = '[',
            WhileEnd = ']',
        }

        /*errors*/
        public static Error ErEarlyCycleCloseing = new Critical("Cycle was not opened before closeing");
        public static Error ErCycleWithNoCloseing = new Warning("Cycle was not closed");
        public static Error ErShortInput = new Warning("Input text is too short");
        public static Error ErElementLessMin = new Warning("Value in element is less than Min");
        public static Error ErElementOverMax = new Warning("Value in element is over than Max");
    }
}