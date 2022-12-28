from __future__ import annotations
from main import read_input, read_input_string
from typing import List
from enum import Enum, IntEnum


class Direction(Enum):
    N = 1
    NE = 2
    E = 3
    SE = 4
    S = 5
    SW = 6
    W = 7
    NW = 8


class Position:
    def __init__(self, x: int, y: int):
        self.x = x
        self.y = y

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y

    def __hash__(self):
        return hash(str(self))

    def __str__(self):
        return '(' + str(self.x) + ',' + str(self.y) + ')'


class Elf:
    def __init__(self, identifier: int, initial_position: Position):
        self.identifier = identifier
        self.position = initial_position
        self.considered_direction = [Direction.N, Direction.S, Direction.W, Direction.E]

    def __str__(self):
        return 'Elf#' + str(self.identifier) + ' ' + str(self.position)

    def next_direction(self):
        next_direction = self.considered_direction.pop(0)
        self.considered_direction.append(next_direction)
        return next_direction

    def apply_move(self, move: ElfMove):
        self.position = move.destination


class ElfMove:
    def __init__(self, elf: Elf, destination: Position):
        self.elf = elf
        self.destination = destination

    def __str__(self):
        return 'Move: ' + str(self.elf) + ' -> ' + str(self.destination)


def make_destination(start: Position, direction: Direction) -> Position:
    match direction:
        case Direction.N:
            return Position(start.x, start.y - 1)
        case Direction.NE:
            return Position(start.x + 1, start.y - 1)
        case Direction.E:
            return Position(start.x + 1, start.y)
        case Direction.SE:
            return Position(start.x + 1, start.y + 1)
        case Direction.S:
            return Position(start.x, start.y + 1)
        case Direction.SW:
            return Position(start.x - 1, start.y + 1)
        case Direction.W:
            return Position(start.x - 1, start.y)
        case Direction.NW:
            return Position(start.x - 1, start.y - 1)
        case _:
            raise Exception("Unexpected Direction: " + str(direction))


# Returns if a move is possible for an elf in all the given directions.
def elf_move_possible(elf: Elf, elf_positions: set[Position], to_check: List[Direction]) -> bool:
    return all([make_destination(elf.position, direction) not in elf_positions for direction in to_check])


# Return if an elf is in space - no other elf one move away in any direction.
def elf_in_space(elf: Elf, elf_positions: set[Position]) -> bool:
    for direction in Direction:
        if make_destination(elf.position, direction) in elf_positions:
            return False
    return True


# Returns the directions that must be free to move an elf in the given direction.
def required_to_check(d: Direction) -> List[Direction]:
    match d:
        case Direction.N:
            return [Direction.NW, Direction.N, Direction.NE]
        case Direction.S:
            return [Direction.SW, Direction.S, Direction.SE]
        case Direction.W:
            return [Direction.SW, Direction.W, Direction.NW]
        case Direction.E:
            return [Direction.NE, Direction.E, Direction.SE]
        case _:
            raise Exception("Unexpected direction! " + str(d))


def order_of_consideration(initial: Direction) -> List[Direction]:
    in_order = [Direction.N, Direction.S, Direction.W, Direction.E]
    initial_index = in_order.index(initial)
    to_consider = []
    for i in range(4):
        to_consider.append(in_order[(initial_index + i) % 4])
    return to_consider


# Generate Elf move proposals and filter down to unique ones.
def generate_elf_moves(elves: List[Elf]) -> List[ElfMove]:
    existing = set([elf.position for elf in elves])
    proposed = []
    # Generate proposed moves.
    for elf in elves:
        proposals_to_consider = order_of_consideration(elf.next_direction())
        if elf_in_space(elf, existing):
            continue
        for direction in proposals_to_consider:
            positions_to_check = required_to_check(direction)
            if elf_move_possible(elf, existing, positions_to_check):
                proposed.append(ElfMove(elf, make_destination(elf.position, direction)))
                break
    # Filter down to allowed moves (only one elf allowed in each position).
    proposed_destinations = [m.destination for m in proposed]
    destination_counts = {}
    for dest in proposed_destinations:
        destination_counts[dest] = 1 if dest not in destination_counts else destination_counts[dest]+1
    confirmed = list(filter(lambda p: destination_counts[p.destination] == 1, proposed))
    return confirmed


class SimulationResult:
    def __init__(self, elves: List[Elf], num_rounds: int):
        self.elves = elves
        self.num_rounds = num_rounds


def perform_rounds(elves: List[Elf], num_rounds: int,
                   terminate_when_done: bool, config: DiagnosticConfig) -> SimulationResult:
    if config.visualise:
        print(visualisation(elves, config))
    num_round = 0
    for _ in range(num_rounds):
        num_round += 1
        moves = generate_elf_moves(elves)
        for move in moves:
            move.elf.apply_move(move)
        # If desired terminate when none of the elves moved during a round.
        if terminate_when_done and len(moves) == 0:
            break
        if config.visualise  and num_round % config.visualise_every_nth == 0:
            print("Round " + str(num_round))
            print(visualisation(elves, config))

    return SimulationResult(elves, num_round)


def parse_input(filename: str) -> List[Elf]:
    input_string = read_input(filename)
    elf_id = 0
    elves = []
    for y, row in enumerate(input_string):
        for x, elem in enumerate(row):
            if elem == '#':
                elves.append(Elf(elf_id, Position(x, y)))
                elf_id += 1
            elif elem != '.':
                raise Exception("Found unexpected element in terrain! " + str(elem))
    return elves


class DiagnosticConfig:
    def __init__(self, visualise: bool, visualise_every_nth: int, min_coordinate: int, max_coordinate: int):
        self.visualise = visualise
        self.visualise_every_nth = visualise_every_nth
        self.min_coordinate = min_coordinate
        self.max_coordinate = max_coordinate


def visualisation(elves: List[Elf], config: DiagnosticConfig) -> str:
    elf_positions = set([elf.position for elf in elves])
    representation = ""
    grid_size = abs(config.max_coordinate - config.min_coordinate)
    for y in range(grid_size):
        for x in range(grid_size):
            if Position(config.min_coordinate + x, config.min_coordinate + y) in elf_positions:
                representation += '#'
            else:
                representation += '.'
        representation += '\n'
    return representation


def empty_tiles_in_minimal_rectangle(elves: List[Elf]) -> int:
    # Rectangle is aligned to grid with boundaries identified by outer-most elves.
    min_x = min([elf.position.x for elf in elves])
    max_x = max([elf.position.x for elf in elves])
    min_y = min([elf.position.y for elf in elves])
    max_y = max([elf.position.y for elf in elves])
    area = (max_x - min_x + 1) * (max_y - min_y + 1)
    # Each elf occupies a single position.
    return area - len(elves)


def part1() -> int:
    elves = parse_input('q23.txt')
    config = DiagnosticConfig(False, 1, -4, 12)
    res = perform_rounds(elves, 10, False, config)
    return empty_tiles_in_minimal_rectangle(res.elves)


def part2() -> int:
    elves = parse_input('q23.txt')
    config = DiagnosticConfig(False, 25, -100, 100)
    res = perform_rounds(elves, 10000, True, config)
    return res.num_rounds
