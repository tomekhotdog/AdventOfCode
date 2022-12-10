from __future__ import annotations
from main import read_input
from typing import List
from enum import Enum
import copy
from math import copysign

GRID_VISUALISATION_HEIGHT = 32
GRID_VISUALISATION_WIDTH = 30


class Direction(Enum):
    UP = "UP",
    DOWN = "DOWN",
    RIGHT = "RIGHT",
    LEFT = "LEFT"


class Motion:

    def __init__(self, direction: Direction, n: int):
        self.direction = direction
        self.n = n

    def __str__(self):
        return str(self.direction) + " " + str(self.n)


class Point:
    def __init__(self, x: int, y: int):
        self.X = x
        self.Y = y

    def __eq__(self, other):
        return self.X == other.X and self.Y == other.Y

    # https://en.wikipedia.org/wiki/Pairing_function#Cantor_pairing_function.
    def __hash__(self) -> int:
        # Cantor pairing function works for x,y >= 0, so adjust since Point may take negative coordinates.
        x_prime = self.X + 10000
        y_prime = self.Y + 10000
        return int((x_prime + y_prime) * (x_prime + y_prime + 1) / 2 + x_prime)

    def __str__(self):
        return "(" + str(self.X) + "," + str(self.Y) + ")"

    def move(self, direction: Direction):
        match direction:
            case Direction.UP:
                self.Y += 1
            case Direction.DOWN:
                self.Y -= 1
            case Direction.RIGHT:
                self.X += 1
            case Direction.LEFT:
                self.X -= 1
            case _:
                raise Exception("Unexpected Direction: " + str(direction))

    def touching(self, other: Point) -> bool:
        return abs(self.X - other.X) <= 1 and abs(self.Y - other.Y) <= 1


class Rope:
    def __init__(self, number_of_knots: int):
        self.knots = [Point(0, 0) for _ in range(number_of_knots)]

    def __str__(self):
        return "H=" + str(self.H) + ", T=" + str(self.T) + "."

    def get_tail(self) -> Point:
        return self.knots[len(self.knots) - 1]

    def get_head(self) -> Point:
        return self.knots[0]

    def move_head(self, direction: Direction):
        for index, knot in enumerate(self.knots):
            if index == 0:
                knot.move(direction)
            else:
                knot_ahead = self.knots[index-1]
                if not knot.touching(knot_ahead):
                    delta_y = knot_ahead.Y - knot.Y
                    delta_x = knot_ahead.X - knot.X
                    # copysign(1, x) is equivalent to sign(x) in most languages.
                    knot.Y += int(copysign(1, delta_y)) if delta_y != 0 else 0
                    knot.X += int(copysign(1, delta_x)) if delta_x != 0 else 0


class Grid:
    def __init__(self, rope: Rope):
        self.rope = rope

    def __str__(self):
        return "Rope: (" + str(self.rope) + ")"

    def apply_single_motion(self, direction: Direction) -> None:
        self.rope.move_head(direction)

    def visualisation(self, grid_height: int, grid_width: int) -> str:
        height_adjust = grid_height // 2
        width_adjust = grid_width // 2
        representation = ""
        for y in range(grid_height-1-height_adjust, -1-height_adjust, -1):
            for x in range(-width_adjust, grid_width-width_adjust, 1):
                icon = '.'
                for idx, knot in reversed(list(enumerate(self.rope.knots))):
                    if knot == Point(x, y):
                        if idx == 0:
                            icon = 'H'
                        elif idx == len(self.rope.knots):
                            icon = 'T'
                        else:
                            icon = str(idx)
                representation += icon
            representation += '\n'
        return representation


def parse_direction(raw_string: str):
    match raw_string:
        case "U":
            return Direction.UP
        case "D":
            return Direction.DOWN
        case "L":
            return Direction.LEFT
        case "R":
            return Direction.RIGHT
    raise Exception("Failed to parse direction: " + raw_string)


def parse_motion(raw_string: str) -> Motion:
    elems = raw_string.split(' ')
    assert(len(elems) == 2, "Could not parse instruction: " + raw_string)
    return Motion(parse_direction(elems[0]), int(elems[1]))


def parse_motions() -> List[Motion]:
    return [parse_motion(line) for line in read_input("q9.txt")]


# Simulate the movement of the rope. Returns the number of positions the tail of the rope visits at least once.
def simulate(num_knots: int, visualise: bool) -> int:
    tail_positions = []
    grid = Grid(Rope(num_knots))
    tail_positions.append(grid.rope.get_tail())
    for motion in parse_motions():
        if visualise:
            print(motion)
        for i in range(motion.n):
            grid.apply_single_motion(motion.direction)
            # Keep track of the positions the tail visits.
            tail_positions.append(copy.copy(grid.rope.get_tail()))
            if visualise:
                print(grid.visualisation(GRID_VISUALISATION_HEIGHT, GRID_VISUALISATION_WIDTH))
    # Return the number of unique positions that the tail of the rope visited.
    return len(set(tail_positions))


def part1() -> int:
    return simulate(2, True)


def part2() -> int:
    return simulate(10, False)
