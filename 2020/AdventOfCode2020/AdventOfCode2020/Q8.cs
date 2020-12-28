using System;
using System.Collections.Generic;

namespace AdventOfCode2020
{
    public static class Q8
    {
        // Find value of accumulator before re-entering a loop.
        public static ExecutionResult SolvePar1()
        {
            var instructions = Tools.GetInput(8);
            return TerminateOrFindLoop(instructions);
        }
        
        // Find value of accumulator of fixed instruction set.
        public static ExecutionResult SolvePart2()
        {
            var instructions = Tools.GetInput(8);
            for (var i = 0; i < instructions.Length; i++)
            {
                var instructionToTweakOriginal = instructions[i];
                if (instructions[i].Contains("nop"))
                {
                    instructions[i] = instructions[i].Replace("nop", "jmp");
                    
                } 
                else if (instructions[i].Contains("jmp"))
                {
                    instructions[i] = instructions[i].Replace("jmp", "nop");
                }

                var executionResult = TerminateOrFindLoop(instructions);
                if (executionResult.InstructionIndex == instructions.Length)
                {
                    return executionResult;
                }
                instructions[i] = instructionToTweakOriginal;
            }
            
            throw new Exception();
        }

        private static ExecutionResult TerminateOrFindLoop(string[] instructions)
        {
            var instructionsExecuted = new HashSet<int>();
            var instructionIndex = 0;
            var accumulatorValue = 0;
            while (true)
            {
                if (instructionIndex == instructions.Length)
                {
                    Console.WriteLine($"Terminated with: Accumulator = {accumulatorValue}");
                    return new ExecutionResult(instructionIndex, accumulatorValue);
                }
                
                if (instructionsExecuted.Contains(instructionIndex))
                {
                    Console.WriteLine($"Found loop. Accumulator = {accumulatorValue}");
                    return new ExecutionResult(instructionIndex, accumulatorValue);
                }
                
                // Mark instruction as executed.
                instructionsExecuted.Add(instructionIndex);
                
                var nextInstruction = instructions[instructionIndex];
                if (nextInstruction.Contains("nop"))
                {
                    instructionIndex++;
                } 
                else if (nextInstruction.Contains("acc"))
                {
                    accumulatorValue += ParseNumber(nextInstruction.Split(" ")[1].Trim());
                    instructionIndex++;
                }
                else if (nextInstruction.Contains("jmp"))
                {
                    instructionIndex += ParseNumber(nextInstruction.Split(" ")[1].Trim());
                }
                else
                {
                    throw new Exception($"Unexpected instruction: {nextInstruction}");
                }
            }
        }

        public struct ExecutionResult
        {
            public int InstructionIndex;
            public int AccumulatedValue;

            public ExecutionResult(int index, int value)
            {
                InstructionIndex = index;
                AccumulatedValue = value;
            }

            public override string ToString()
            {
                return $"InstructionIndex={InstructionIndex}. AccumulatedValue={AccumulatedValue}";
            }
        }

        private static int ParseNumber(string numberRepresentation)
        {
            var numberValue = int.Parse(numberRepresentation.Substring(1, numberRepresentation.Length - 1));
            if (numberRepresentation[0] == '-')
            {
                return -numberValue;
            }
            if (numberRepresentation[0] == '+')
            {
                return numberValue;
            }
            
            throw new Exception($"Unexpected representation: {numberRepresentation}");
        }
    }
}