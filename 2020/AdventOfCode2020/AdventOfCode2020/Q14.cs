using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q14
    {
        public static void Solve()
        {
            var input = Tools.GetInput(14);
            ParseInstructions(input, true);
            ParseInstructions(input, false);
        }

        private static void ParseInstructions(string[] instructions, bool part1)
        {
            var memory = new Dictionary<long, long>();
            var mask = "";
            foreach (var instructionString in instructions)
            {
                var instruction = new Instruction(instructionString);
                switch (instruction.Type)
                {
                    case InstructionType.Mask:
                        mask = instruction.Mask;
                        break;
                    case InstructionType.MemoryAllocation:
                        if (part1) ApplyMemoryAllocationForPart1(memory, mask, instruction);
                        else       ApplyMemoryAllocationForPart2(memory, mask, instruction);
                        break;
                    default:
                        throw new Exception($"Unknown instruction: {instructionString}");
                }
            }
            var sumOfAllValues = memory.Values.Sum(i => i);
            Console.WriteLine($"Sum of values in memory = {sumOfAllValues}");
        }

        private static void ApplyMemoryAllocationForPart1(
            Dictionary<long, long> memorySpace, string mask, Instruction instruction)
        {
            var binaryRepresentation = Convert.ToString(instruction.MemoryAllocationValue, 2)
                .PadLeft(36, '0').ToCharArray();
            for (var i = 0; i < binaryRepresentation.Length; i++)
            {
                binaryRepresentation[i] = mask[i] switch
                {
                    '0' => '0',
                    '1' => '1',
                    _ => binaryRepresentation[i]
                };
            }
            var newValue = Convert.ToInt64(new string(binaryRepresentation), 2);
            memorySpace[instruction.MemoryAllocationAddress] = newValue;
        }

        private static void ApplyMemoryAllocationForPart2(
            Dictionary<long, long> memorySpace, string mask, Instruction instruction)
        {
            var binaryRepresentation = Convert.ToString(instruction.MemoryAllocationAddress, 2)
                .PadLeft(36, '0').ToCharArray();
            for (var i = 0; i < binaryRepresentation.Length; i++)
            {
                binaryRepresentation[i] = mask[i] switch
                {
                    '0' => binaryRepresentation[i],
                    '1' => '1',
                    'X' => 'X',
                    _ => throw new Exception($"Unexpected bit: {mask[i]}.")
                };
            }
            var newRepresentation = new string(binaryRepresentation);
            
            // TODO: Surely can think of a nicer way to expand these strings?
            var addresses = new List<string>();
            var expandedSet = new List<string>{newRepresentation};
            while (addresses.Count != expandedSet.Count)
            {
                addresses = new List<string>(expandedSet);
                expandedSet.Clear();
                foreach (var addressString in addresses)
                {
                    if (addressString.Contains('X'))
                    {
                        var index = addressString.IndexOf('X');
                        var address = addressString.ToCharArray();
                        address[index] = '0';
                        expandedSet.Add(new string(address));
                        address[index] = '1';
                        expandedSet.Add(new string(address));
                    }
                    else
                    {
                        expandedSet.Add(addressString);
                    }
                }
            }

            foreach (var address in expandedSet)
            {
                memorySpace[Convert.ToInt64(address, 2)] = instruction.MemoryAllocationValue;
            }
        }
        
        enum InstructionType { Mask, MemoryAllocation }

        struct Instruction
        {
            internal InstructionType Type { get; }
            internal string Mask { get; }
            internal long MemoryAllocationAddress { get; }
            internal long MemoryAllocationValue { get; }
            
            internal Instruction(string instruction)
            {
                if (instruction.Substring(0, 4) == "mask")
                {
                    Type = InstructionType.Mask;
                    Mask = instruction.Split("=")[1].Trim();
                    MemoryAllocationAddress = 0;
                    MemoryAllocationValue = 0;
                } 
                else if (instruction.Substring(0, 3) == "mem")
                {
                    Type = InstructionType.MemoryAllocation;
                    Mask = string.Empty;
                    MemoryAllocationAddress = uint.Parse(instruction.Split('[')[1].Split(']')[0]);
                    MemoryAllocationValue = uint.Parse(instruction.Split('=')[1].Trim());
                }
                else
                {
                    throw new Exception("Unknown instruction.");
                }
            }
        }
    }
}