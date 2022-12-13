from __future__ import annotations
from main import read_input
from typing import List


class SquarePosition:
    def __init__(self, row: int, col: int):
        self.row = row
        self.col = col

    def __str__(self):
        return "(" + str(self.row) + ', ' + str(self.col) + ')'


def get_elevation_char(elevation_char: chr):
    match elevation_char:
        case 'S':
            return 'a'
        case 'E':
            return 'z'
        case _:
            return elevation_char


class Square:

    def __init__(self, elevation_char):
        self.elevation = elevation_char
        self.distance = 999

    def can_step_onto(self, other: Square):
        source_elevation = get_elevation_char(self.elevation)
        target_elevation = get_elevation_char(other.elevation)
        return ord(target_elevation) - ord(source_elevation) <= 1

    def can_step_from(self, other: Square):
        return other.can_step_onto(self)


class Terrain:

    def __init__(self, raw_input: List[str]):
        self.map = [list(map(lambda x: Square(x), list(row))) for row in raw_input]
        self.get_square(self.find_squares('E')[0]).distance = 0

    def __str__(self):
        representation = ''
        for row in self.map:
            for elem in row:
                representation += elem.elevation
            representation += '\n'
        return representation

    def path_length_representation(self) -> str:
        representation = ''
        for row in self.map:
            for elem in row:
                representation += str(elem.distance).zfill(3) + ' '
            representation += '\n'
        return representation

    def find_squares(self, character_to_find: chr) -> List[SquarePosition]:
        candidates = []
        for row_index, row in enumerate(self.map):
            for col_index, elem in enumerate(row):
                if elem.elevation == character_to_find:
                    candidates.append(SquarePosition(row_index, col_index))
        return candidates

    def get_square(self, pos: SquarePosition) -> Square:
        return self.map[pos.row][pos.col]

    # Returns the coordinates of neighbouring squares.
    def neighbours(self, square: SquarePosition) -> List[SquarePosition]:
        squares = []
        if square.row > 0:
            squares.append(SquarePosition(square.row-1, square.col))
        if square.row < len(self.map)-1:
            squares.append(SquarePosition(square.row+1, square.col))
        if square.col > 0:
            squares.append(SquarePosition(square.row, square.col-1))
        if square.col < len(self.map[0])-1:
            squares.append(SquarePosition(square.row, square.col+1))
        return squares


def traverse(terrain: Terrain, square_pos: SquarePosition) -> None:
    squares_to_visit = [square_pos]
    while len(squares_to_visit) > 0:
        squares_to_visit += traverse_neighbours(terrain, squares_to_visit.pop(0))


def traverse_neighbours(terrain: Terrain, square_pos: SquarePosition) -> List[SquarePosition]:
    to_traverse = []
    square = terrain.map[square_pos.row][square_pos.col]
    neighbours = terrain.neighbours(square_pos)
    for neighbour_pos in neighbours:
        neighbour = terrain.map[neighbour_pos.row][neighbour_pos.col]
        if square.can_step_from(neighbour) and square.distance + 1 < neighbour.distance:
            neighbour.distance = square.distance + 1
            to_traverse.append(neighbour_pos)
    return to_traverse


def pre_compute_terrain(visualise: bool) -> Terrain:
    terrain = Terrain(read_input('q12.txt'))
    if visualise:
        print(terrain)
    traverse(terrain, terrain.find_squares('E')[0])
    if visualise:
        print(terrain.path_length_representation())
    return terrain


# Return length of shortest path from Start (S) -> End (E)
def part1() -> int:
    terrain = pre_compute_terrain(True)
    return terrain.get_square(terrain.find_squares('S')[0]).distance


# Return length of shortest path from any square with elevation 'a'-> End (E)
def part2() -> int:
    terrain = pre_compute_terrain(True)
    candidates = terrain.find_squares('S') + terrain.find_squares('a')
    return min([terrain.get_square(pos).distance for pos in candidates])
