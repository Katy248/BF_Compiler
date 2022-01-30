using System;
using System.Linq;
/// Important!
/// 
/// Do not close cycle before open it, it can't be checked // no longer relevant

/// Using examples
/// 
/// To get formatted output (with messages) use:
/// <code>
///     FormattedOutput = Compilate(Text, Input);
/// </code>
/// 
/// If you want to get only output without formatting use:
/// <code>
///     Compilate(Text, Input);
///     Output = BF.Output;
/// </code>
/// 
/// If you want to get only messages use:
/// <code>
///     Compilate(Text, Input);
///     Messages = Error.Output;
/// </code>
/// 
namespace BF_Compiler
{
    public static class BF
    {
        /*public*/
        public static int[] Array;
        public static string output;

        /*private*/
        static int currentElement;
        static int currentInputSymbol;
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
        static string[] Status = new string[] { "Done with result:\n", "Stopped because of:\n" };
        const int MaxElementSize = int.MaxValue;
        const int MinElementSize = 0;

        public static string Compilate(string Text, string Input)
        {
            /*Initialize Array with zero*/
            Array = new int[3000];
            for (int i = 0; i < Array.Length; i++) Array[i] = MinElementSize;
            
            /*Reseting*/
            output = "";
            currentElement = 0;
            currentInputSymbol = 0;
            foreach (Error Er in Error.Errors) Er.Reset();
            
            /*Errors checking*/
            TextIdentificate(Text, Input);
            
            /*Interpritate*/
            if (!Error.CriticalFounded())
                for (int SymNum = 0; SymNum < Text.Length; SymNum++)
                {
                    switch ((Commands)Text[SymNum])
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
                            output += (char)Array[currentElement];
                            break;
                        case Commands.In:
                            if (currentInputSymbol < Input.Length)
                            {
                                Array[currentElement] = Input[currentInputSymbol];
                                currentInputSymbol++;
                            }
                            break;
                        case Commands.WhileStart:
                            if (Array[currentElement] == 0)
                            {
                                int CycleEnds = 1;
                                while (CycleEnds > 0 && SymNum < Text.Length)
                                {
                                    SymNum++;
                                    if (Text[SymNum] == ']') CycleEnds--;
                                    if (Text[SymNum] == '[') CycleEnds++;
                                }
                            }
                            break;
                        case Commands.WhileEnd:
                            if (Array[currentElement] > 0)
                            {
                                int CycleStarts = 1;
                                while (CycleStarts > 0 && SymNum >= 0)
                                {
                                    SymNum--;
                                    if (Text[SymNum] == '[') CycleStarts--;
                                    if (Text[SymNum] == ']') CycleStarts++;
                                }
                            }
                            break;
                    }
                }
            Array = null;
            GC.Collect();
            return Status[Error.CriticalFounded() ? (1) : (0)] + (Error.CriticalFounded() ? (output + "\n") : "") + Error.Output();
        }
        public static void TextIdentificate(string Text, string Input)
        {
            if (Text.Count(f => f == ',') > Input.Length)
                ErShortInput.Found();

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
        /*errors*/
        static Error ErEarlyCycleCloseing = new Critical("Cycle was not opened before closeing");
        static Error ErCycleWithNoCloseing = new Warning("Cycle was not closed");
        static Error ErShortInput = new Warning("Input text is too short");
    }
}