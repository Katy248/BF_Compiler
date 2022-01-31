using System;
using System.Linq;

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
        public static int[] array;
        public static string Output;
        static int currentElement;
        static int currentInputSymbol;
        
        static readonly string[] Status = new string[] { "Done with result:\n", "Stopped because of:\n" };
        const int MaxElementSize = int.MaxValue;
        const int MinElementSize = 0;

        public static string Compilate(string text, string input)
        {
            array = new int[3000];
            for (int i = 0; i < array.Length; i++) array[i] = MinElementSize;
            
            Output = "";
            currentElement = 0;
            currentInputSymbol = 0;
            foreach (Error Er in Error.Errors) Er.Reset();
            
            TextIdentificate(text, input);
            
            if (!Error.CriticalFounded())
                for (int symNum = 0; symNum < text.Length; symNum++)
                {
                    switch ((Commands)text[symNum])
                    {
                        case Commands.Next:
                            if (currentElement < array.Length - 1) currentElement++;
                            break;
                        case Commands.Previous:
                            if (currentElement > 0) currentElement--;
                            break;
                        case Commands.Plus:
                            if (array[currentElement] < MaxElementSize) array[currentElement]++;
                            break;
                        case Commands.Minus:
                            if (array[currentElement] > MinElementSize) array[currentElement]--;
                            break;
                        case Commands.Out:
                            Output += (char)array[currentElement];
                            break;
                        case Commands.In:
                            if (currentInputSymbol < input.Length)
                            {
                                array[currentElement] = input[currentInputSymbol];
                                currentInputSymbol++;
                            }
                            break;
                        case Commands.WhileStart:
                            if (array[currentElement] == 0)
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
                            if (array[currentElement] > 0)
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
            array = null;
            GC.Collect();
            return Status[Error.CriticalFounded() ? (1) : (0)] + (Error.CriticalFounded() ? (Output + "\n") : "") + Error.Output();
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
    }
}