using NUnit.Framework;
// ReSharper disable IsExpressionAlwaysTrue

namespace func.brainfuck;

[TestFixture]
public class VirtualMachineTests
{
	[Test]
	public void ImplementIVirtualMachine()
	{
		var vmBuilder = new VmBuilder(1);
		Assert.IsTrue(vmBuilder.Build("") is IVirtualMachine);
	}
	[Test]
	public void Initialize()
	{
		var vmBuilder = new VmBuilder(12345)
			.RegisterCommand('x', b => { });
		var vm = vmBuilder.Build("xxx");
		Assert.AreEqual(12345, vm.Memory.Length);
		Assert.AreEqual(0, vm.MemoryPointer);
		Assert.AreEqual("xxx", vm.Instructions);
		Assert.AreEqual(0, vm.InstructionPointer);
	}

	[Test]
	public void SetMemorySize()
	{
		var vmBuilder = new VmBuilder(42);
		var vm = vmBuilder.Build("");
		Assert.AreEqual(42, vm.Memory.Length);
	}

	[Test]
	public void IncrementInstructionPointer()
	{
		var res = "";
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('x', b => { res += "x->" + b.InstructionPointer + ", "; })
			.RegisterCommand('y', b => { res += "y->" + b.InstructionPointer; });
		vmBuilder
			.Build("xy")
			.Run();
		Assert.AreEqual("x->0, y->1", res);
	}

	[Test]
	public void MoveInstructionPointerForward()
	{
		var res = "";
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('x', b => { b.InstructionPointer++; })
			.RegisterCommand('y', b => { Assert.Fail(); })
			.RegisterCommand('z', b => { res += "z"; });
		vmBuilder
			.Build("xyz")
			.Run();
		Assert.AreEqual("z", res);
	}
	    
	[Test]
	public void InstructionPointerCanMovedOutside()
	{
		var res = "";
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('x', b => { res += "x"; })
			.RegisterCommand('y', b => { Assert.Fail(); })
			.RegisterCommand('z', b => { res += "z"; });
		var vm = vmBuilder.Build("yxz");
		vm.InstructionPointer++;
		vm.Run();
		Assert.AreEqual("xz", res);
	}

	[Test]
	public void MoveInstructionPointerBackward()
	{
		var res = "";
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('>', b =>
			{
				b.InstructionPointer++;
				res += ">";
			})
			.RegisterCommand('<', b =>
			{
				b.InstructionPointer -= 2;
				res += "<";
			});
		vmBuilder
			.Build(">><")
			.Run();
		Assert.AreEqual("><>", res);
	}
	
	[Test]
	public void ChangeMemoryPointer()
	{
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('x', b => { b.MemoryPointer = b.MemoryPointer + 42; })
			.RegisterCommand('y', b => { Assert.AreEqual(42, b.MemoryPointer); });
		vmBuilder
			.Build("xy")
			.Run();
	}

	[Test]
	public void SkipUnknownCommands()
	{
		var vmBuilder = new VmBuilder(10);
		vmBuilder
			.Build("xyz")
			.Run();
	}

	[Test]
	public void ReadWriteMemory()
	{
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('w', b => { b.Memory[3] = 42; })
			.RegisterCommand('r', b => { Assert.AreEqual(42, b.Memory[3]); });
		vmBuilder
			.Build("wr")
			.Run();
	}

	[Test]
	public void RunInstructionsInRightOrder()
	{
		var res = "";
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('a', b => { res += "a"; })
			.RegisterCommand('b', b => { res += "b"; });
		if (res != "")
			Assert.Fail("Instructions should not be executed before Run() call");
		vmBuilder
			.Build("abbaaa")
			.Run();
		if (res != "")
			Assert.AreEqual("abbaaa", res, "Execution order of program 'abbaaa'");
	}

	[Test]
	public void RunManyInstructions()
	{
		var count = 0;
		var vmBuilder = new VmBuilder(10)
			.RegisterCommand('a', b => { count++; });
		vmBuilder
			.Build(new string('a', 10000))
			.Run();
		Assert.AreEqual(10000, count, "Number of executed instructions");
	}
}