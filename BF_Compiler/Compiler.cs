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
        static readonly string[] status = new string[] { "Done with result:\n", "Stopped because of:\n" };
        const int maxElementSize = int.MaxValue;
        const int minElementSize = 0;

        public static string Compilate(string text, string input)
        {
            array = new int[3000];
            for (int i = 0; i < array.Length; i++) array[i] = minElementSize;
            
            output = "";
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
                            if (array[currentElement] < maxElementSize) array[currentElement]++;
                            break;
                        case Commands.Minus:
                            if (array[currentElement] > minElementSize) array[currentElement]--;
                            break;
                        case Commands.Out:
                            output += (char)array[currentElement];
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
            return status[Error.CriticalFounded() ? (1) : (0)] + (Error.CriticalFounded() ? (output + "\n") : "") + Error.Output();
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