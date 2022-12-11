from main import read_input
from typing import List
from enum import Enum


class InstructionType(Enum):
    ADD = "ADD"
    NOOP = "NOOP"


class Instruction:
    def __init__(self, instruction_type: InstructionType, argument: int):
        self.instruction_type = instruction_type
        self.argument = argument

    def __str__(self):
        match self.instruction_type:
            case InstructionType.ADD:
                return "ADD " + str(self.argument)
            case InstructionType.NOOP:
                return "NOOP"
            case _:
                raise ("Unexpected instruction type: " + str(self.instruction_type))


class CPU:
    def __init__(self, instructions: List[Instruction]):
        self.X = 1
        self.instructions = instructions
        self.cycle_counter = 0
        self.current_instruction_index = 0
        self.current_instruction_cycles_remaining = 0

    def start_next_cycle(self):
        self.cycle_counter += 1
        # If finished processing previous instruction start the next one.
        if self.current_instruction_cycles_remaining == 0:
            instruction = self.instructions[self.current_instruction_index]
            match instruction.instruction_type:
                case InstructionType.ADD:
                    self.current_instruction_cycles_remaining += 2
                case InstructionType.NOOP:
                    self.current_instruction_cycles_remaining += 1
                case _:
                    raise("Unexpected instruction type: " + str(instruction.instruction_type))

    def end_cycle(self):
        if self.current_instruction_cycles_remaining == 0:
            raise Exception("Idle at end of cycle! #cycle=" + str(self.cycle_counter))
        self.current_instruction_cycles_remaining -= 1
        if self.current_instruction_cycles_remaining == 0:
            # Conclude the processing of the given instruction.
            instruction = self.instructions[self.current_instruction_index]
            match instruction.instruction_type:
                case InstructionType.ADD:
                    self.X += instruction.argument
                case InstructionType.NOOP:
                    pass
                case _:
                    raise("Unexpected instruction type: " + str(instruction.instruction_type))
            self.current_instruction_index += 1

    # Defined to be "the cycle number multiplied by the value of the X register".
    def signal_strength(self):
        return self.cycle_counter * self.X

    def terminated(self):
        return self.current_instruction_index >= len(self.instructions)

    def __str__(self):
        inst_string = str(self.instructions[self.current_instruction_index]) if self.current_instruction_index < len(self.instructions) else "NONE"
        return "[cycle=" + str(self.cycle_counter) + ", X=" + str(self.X) + ", instruction=" + inst_string + "]"


# Assumed to be of the form: [noop|addx] x?
# For example: noop, addx 3, addx -5
def parse_instruction(raw_instruction: str) -> Instruction:
    elems = raw_instruction.split(' ')
    match elems[0]:
        case "noop":
            return Instruction(InstructionType.NOOP, 0)
        case "addx":
            return Instruction(InstructionType.ADD, int(elems[1]))


def parse_instructions() -> List[Instruction]:
    return [parse_instruction(raw_instruction) for raw_instruction in read_input("q10.txt")]


def part1() -> int:
    cycles_to_log = set(list(range(20, 221, 40)))
    visualise = False
    cpu = CPU(parse_instructions())
    signal_strengths = []
    while not cpu.terminated():
        cpu.start_next_cycle()
        if visualise:
            print("start: " + str(cpu))
        if cpu.cycle_counter in cycles_to_log:
            ss = cpu.signal_strength()
            if visualise:
                print("signal_strength=" + str(ss))
            signal_strengths.append(ss)
        cpu.end_cycle()
        if visualise:
            print("end: " + str(cpu))
    return sum(signal_strengths)


# Simulate the CRT screen.
def part2() -> str:
    output = ""
    cpu = CPU(parse_instructions())
    while not cpu.terminated():
        cpu.start_next_cycle()
        output += '#' if abs(cpu.X - ((cpu.cycle_counter % 40) - 1)) <= 1 else '.'
        cpu.end_cycle()
        output += '\n' if cpu.cycle_counter % 40 == 0 else ''
    return output
