using System;
using System.Collections.Generic;

namespace func.brainfuck;

public interface IVmBuilder
{
    public IVmBuilder AddBasicCommands(Func<int> read, Action<char> write);
    public IVmBuilder AddLoopCommands();
    public VirtualMachine Build(string program);
    public IVmBuilder RegisterCommand(char symbol, Action<IVirtualMachine> execute);
}

public class VmBuilder : IVmBuilder
{
    private Action<IVirtualMachine> _addBasicCommands;
    private Action<IVirtualMachine> _addLoopCommands;
    private Dictionary<char, Action<IVirtualMachine>> _commands = new ();
    private int _memorySize;

    public VmBuilder(int memorySize)
    {
        _memorySize = memorySize;
    }
    
    public IVmBuilder AddBasicCommands(Func<int> read, Action<char> write)
    {
        _addBasicCommands = vm => BrainfuckBasicCommands.RegisterTo(vm, read, write);
        return this;
    }

    public IVmBuilder AddLoopCommands()
    {
        _addLoopCommands = vm => BrainfuckLoopCommands.RegisterTo(vm);
        return this;
    }

    public IVmBuilder RegisterCommand(char symbol, Action<IVirtualMachine> execute)
    {
        _commands.Add(symbol, execute);
        return this;
    }

    public VirtualMachine Build(string program)
    {
        var vm = new VirtualMachine(program, _memorySize);
        _addBasicCommands?.Invoke(vm);
        _addLoopCommands?.Invoke(vm);
        foreach (var charCommandPair in _commands)
            vm.RegisterCommand(charCommandPair.Key, charCommandPair.Value);
        return vm;
    }
}