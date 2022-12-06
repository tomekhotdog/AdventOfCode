from collections import deque
from typing import List
from main import read_input_string
from enum import Enum


class CrateMoverVersion(Enum):
    # CrateMover used in part 1 - moves crates one at a time.
    VERSION_9000 = 1
    # CrateMover used in part 2 - moves multiple crates at a time.
    VERSION_9001 = 2


class MoveInstruction:
    def __init__(self, raw: str):
        elems = raw.split()
        assert(elems[0] == "move", "Unexpected instruction: " + elems[0])
        assert(len(elems) == 6, "Unexpected instruction (should be six elements): " + (''.join(elems)))
        self.move_n = int(elems[1])
        self.from_index = int(elems[3]) - 1
        self.to_index = int(elems[5]) - 1

    def __str__(self):
        return f"Move {self.move_n} from index={self.from_index} to index={self.to_index}"


# Represent each stack as a deque of chars.
def parse_stacks(stack_lines: List[str]) -> List[deque]:
    stacks = []
    for line in stack_lines:
        # elems = line.split()
        # TODO: Only works if crate label is one character long.
        for index in range(1, len(line), 4):
            # item = elem.split('[')[1].split(']')[0]
            item = line[index]
            stack_index = (index - 1) // 4
            if item != ' ':
                while len(stacks) <= stack_index:
                    stacks.append(deque())
                stacks[stack_index].append(item)
    return stacks


def apply_instruction(stacks: List[deque], instruction: MoveInstruction, version: CrateMoverVersion) -> None:
    match version:
        case CrateMoverVersion.VERSION_9000:
            for _ in range(instruction.move_n):
                stacks[instruction.to_index].appendleft(stacks[instruction.from_index].popleft())
            return
        case CrateMoverVersion.VERSION_9001:
            crates = deque()
            for _ in range(instruction.move_n):
                crates.appendleft(stacks[instruction.from_index].popleft())
            for crate in crates:
                stacks[instruction.to_index].appendleft(crate)
            return


def apply_instructions(stacks: List[deque], instructions: List[str], version: CrateMoverVersion) -> None:
    for instruction in instructions:
        apply_instruction(stacks, MoveInstruction(instruction), version)


def read_top_of_stacks(stacks: List[deque]) -> str:
    return ''.join([stack[0] if len(stack) > 0 else '' for stack in stacks])


def simulate_crate(version: CrateMoverVersion) -> str:
    input_elems = read_input_string("q5.txt").split("\n\n")
    stack_lines = input_elems[0].split("\n")[:-1]
    instructions = input_elems[1].split("\n")

    stacks = parse_stacks(stack_lines)
    apply_instructions(stacks, instructions, version)
    return read_top_of_stacks(stacks)


def part1() -> str:
    return simulate_crate(CrateMoverVersion.VERSION_9000)


def part2() -> str:
    return simulate_crate(CrateMoverVersion.VERSION_9001)


print(part1())
print(part2())
