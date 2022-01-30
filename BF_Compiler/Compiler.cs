using System;
using System.Linq;

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
        public static int[] array;
        public static string output;

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
        const int maxElementSize = int.MaxValue;
        const int minElementSize = 0;

        public static string Compilate(string Text, string Input)
        {
            /*Initialize Array with zero*/
            array = new int[3000];
            for (int i = 0; i < array.Length; i++) array[i] = minElementSize;
            
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
                            if (currentElement < array.Length - 1) currentElement++;
                            break;
                        case Commands.Previous:
                            if (currentElement > 0) currentElement--;
                            break;
                        case Commands.Plus:
                            if (array[currentElement] < maxElementSize) array[currentElement]++;
                            break;
                        case Commands.Minus:
                            if (array[currentElement] > minElementSize) array[currentElement]--;
                            break;
                        case Commands.Out:
                            output += (char)array[currentElement];
                            break;
                        case Commands.In:
                            if (currentInputSymbol < Input.Length)
                            {
                                array[currentElement] = Input[currentInputSymbol];
                                currentInputSymbol++;
                            }
                            break;
                        case Commands.WhileStart:
                            if (array[currentElement] == 0)
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
                            if (array[currentElement] > 0)
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
            array = null;
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