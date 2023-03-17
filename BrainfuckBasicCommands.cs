using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace func.brainfuck
{
    public class BrainfuckBasicCommands
    {
        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            vm.RegisterCommand('.', b => write((char)b.Memory[b.MemoryPointer]));
            vm.RegisterCommand('+',
                b => b.Memory[b.MemoryPointer] = (byte)(b.Memory[b.MemoryPointer] == byte.MaxValue
                    ? byte.MinValue
                    : b.Memory[b.MemoryPointer] + 1));
            vm.RegisterCommand('-',
                b => b.Memory[b.MemoryPointer] = (byte)(b.Memory[b.MemoryPointer] == byte.MinValue
                    ? byte.MaxValue
                    : b.Memory[b.MemoryPointer] - 1));
            vm.RegisterCommand(',', b => b.Memory[b.MemoryPointer] = (byte)read());
            vm.RegisterCommand('>',
                b => b.MemoryPointer = b.MemoryPointer == b.Memory.Length - 1 ? 0 : b.MemoryPointer + 1);
            vm.RegisterCommand('<',
                b => b.MemoryPointer = b.MemoryPointer == 0 ? b.Memory.Length - 1 : b.MemoryPointer - 1);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            foreach (var ch in chars)
                vm.RegisterCommand(ch, b => b.Memory[b.MemoryPointer] = (byte)ch);
        }
    }
}