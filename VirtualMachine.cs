using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    public class VirtualMachine : IVirtualMachine
    {
        public string Instructions { get; }
        public int InstructionPointer { get; set; }
        public byte[] Memory { get; }
        public int MemoryPointer { get; set; }
        private Dictionary<char, Action<IVirtualMachine>> _commands;

        public VirtualMachine(string program, int memorySize)
        {
            Instructions = program;
            InstructionPointer = 0;
            Memory = new byte[memorySize];
            MemoryPointer = 0;
            _commands = new Dictionary<char, Action<IVirtualMachine>>();
        }

        public void RegisterCommand(char symbol, Action<IVirtualMachine> execute)
        {
            _commands.Add(symbol, execute);
        }

        public void Run()
        {
            while (true)
            {
                if (_commands.ContainsKey(Instructions[InstructionPointer]))
                    _commands[Instructions[InstructionPointer]](this);
                InstructionPointer++;
                if (InstructionPointer >= Instructions.Length)
                    break;
            }
        }
    }
}