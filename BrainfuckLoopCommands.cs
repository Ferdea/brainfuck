using System.Collections.Generic;

namespace func.brainfuck
{
    public class BrainfuckLoopCommands
    {
        public static void RegisterTo(IVirtualMachine vm)
        {
            var closedBracketPositions = new Dictionary<int, int>();
            vm.RegisterCommand('[', b =>
            {
                if (b.Memory[b.MemoryPointer] == 0)
                {
                    if (closedBracketPositions.Count == 0)
                        closedBracketPositions = GetClosedBracketPositions(b);
                    b.InstructionPointer = closedBracketPositions[b.InstructionPointer];
                }
            });
            var openBracketPositions = new Dictionary<int, int>();
            vm.RegisterCommand(']', b =>
            {
                if (b.Memory[b.MemoryPointer] != 0)
                {
                    if (openBracketPositions.Count == 0)
                        openBracketPositions = GetOpenBracketPositions(b);
                    b.InstructionPointer = openBracketPositions[b.InstructionPointer];
                }
            });
        }

        private static Dictionary<int, int> GetOpenBracketPositions(IVirtualMachine virtualMachine)
        {
            var openBracketPositions = new Dictionary<int, int>();
            var closedBracketsStack = new Stack<int>();
            for (var i = virtualMachine.Instructions.Length - 1; i >= 0; i--)
            {
                if (virtualMachine.Instructions[i] == ']')
                    closedBracketsStack.Push(i);
                if (virtualMachine.Instructions[i] == '[')
                    openBracketPositions.Add(closedBracketsStack.Pop(), i);
            }

            return openBracketPositions;
        }
		
        private static Dictionary<int, int> GetClosedBracketPositions(IVirtualMachine virtualMachine)
        {
            var closedBracketPositions = new Dictionary<int, int>();
            var openBracketsStack = new Stack<int>();
            for (var i = 0; i < virtualMachine.Instructions.Length; i++)
            {
                if (virtualMachine.Instructions[i] == '[')
                    openBracketsStack.Push(i);
                if (virtualMachine.Instructions[i] == ']')
                    closedBracketPositions.Add(openBracketsStack.Pop(), i);
            }

            return closedBracketPositions;
        }
    }
}