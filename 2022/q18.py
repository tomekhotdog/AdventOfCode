from __future__ import annotations
from main import read_input
from typing import List
from collections import deque


# Represents a 1x1x1 3-dimensional cube at a given location (x,y,z).
class Cube:
    # Comma separated string description (e.g. 3,4,8).
    def __init__(self, x=None, y=None, z=None, raw_string = None):
        if raw_string is not None:
            elems = raw_string.split(',')
            if len(elems) != 3:
                raise Exception("Failed to parse cube: " + str(raw_string) + ".")
            self.x, self.y, self.z = int(elems[0]), int(elems[1]), int(elems[2])
        else:
            if x is None or y is None or z is None:
                raise Exception("Failed to parse cube. One of supplied (x,y,z) was None.")
            self.x, self.y, self.z = x, y, z

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y and self.z == other.z

    def __hash__(self):
        return hash(self.x) + hash(self.y) + hash(self.z)

    def __str__(self):
        return 'Cube: (' + str(self.x) + ', ' + str(self.y) + ', ' + str(self.z) + ')'


def get_adjacent_cubes(cube: Cube) -> List[Cube]:
    return [
        Cube(x=cube.x - 1, y=cube.y, z=cube.z),
        Cube(x=cube.x + 1, y=cube.y, z=cube.z),
        Cube(x=cube.x, y=cube.y - 1, z=cube.z),
        Cube(x=cube.x, y=cube.y + 1, z=cube.z),
        Cube(x=cube.x, y=cube.y, z=cube.z - 1),
        Cube(x=cube.x, y=cube.y, z=cube.z + 1),
    ]


# Returns true if there exists a route from the given cube's location to the origin without passing through the cubes
# that should be avoided. Perform a bread-first-search to the (0,0,0).
def exists_path_to_origin(starting_cube: Cube, cubes_to_avoid: set[Cube]):
    if starting_cube in cubes_to_avoid:
        return False
    return find_path_to_cube([starting_cube], Cube(0, 0, 0), set(), cubes_to_avoid)


def find_path_to_cube(cubes_to_visit: List[Cube], target: Cube, cubes_visited: set[Cube], cubes_to_avoid: set[Cube]) -> bool:
    while len(cubes_to_visit) > 0:
        current = cubes_to_visit.pop(0)
        cubes_visited.add(current)
        if current == target:
            return True
        for neighbour in get_adjacent_cubes(current):
            if neighbour not in cubes_to_avoid and neighbour not in cubes_visited and neighbour not in cubes_to_visit:
                if neighbour == target:
                    return True
                cubes_to_visit.append(neighbour)
    # We have exhausted the possible cubes to visit and not found the target.
    return False


# From the given starting cube position and a set of cubes positions that cannot be moved into, traverse through all
# the possible x,y,z dimensions one cube at a time.
def explore(starting_cube: Cube, cubes_to_avoid: set[Cube]) -> set[Cube]:
    visited = set()
    cubes_to_visit = deque([starting_cube])
    while len(cubes_to_visit) > 0:
        current = cubes_to_visit.popleft()
        if current in cubes_to_avoid:
            continue
        visited.add(current)
        for neighbour in get_adjacent_cubes(current):
            if neighbour not in visited and neighbour not in cubes_to_visit:
                cubes_to_visit.append(neighbour)
    return visited


# Generate a list of cubes that forms a cuboidal perimeter around the given set of cubes that represents some
# contiguous blob.
def generate_cuboid(cubes: List[Cube]) -> List[Cube]:
    xs = [cube.x for cube in cubes]
    ys = [cube.y for cube in cubes]
    zs = [cube.z for cube in cubes]

    min_x, max_x = min(xs), max(xs)
    min_y, max_y = min(ys), max(ys)
    min_z, max_z = min(zs), max(zs)
    cuboid = []

    for x in range(min_x-2, max_x+3, 1):
        for y in range(min_y-2, max_y+3, 1):
            cuboid.append(Cube(x, y, min_z-2))
            cuboid.append(Cube(x, y, max_z+3))
    for z in range(min_z-2, max_z+3, 1):
        for y in range(min_y-2, max_y+3, 1):
            cuboid.append(Cube(min_x-2, y, z))
            cuboid.append(Cube(max_x+3, y, z))
    for x in range(min_x-2, max_x+3, 1):
        for z in range(min_z-2, max_z+3, 1):
            cuboid.append(Cube(x, min_y-2, z))
            cuboid.append(Cube(x, max_y+3, z))

    return cuboid


def parse_input() -> List[Cube]:
    return [Cube(raw_string=line) for line in read_input('q18.txt')]


# Given all the cubes, count the number of sides (represented as an adjacent cube) that appear once.
def part1() -> int:
    cubes = parse_input()
    adjacents = [adjacent for cube in cubes for adjacent in get_adjacent_cubes(cube)]
    return len(list(filter(lambda adjacent: adjacent not in cubes, adjacents)))


def part2() -> int:
    cubes = parse_input()
    cuboidal_structure = generate_cuboid(cubes)
    # Construct the complete set of cubes that represents the perimeter that will be explored.
    constraints = set(cubes + cuboidal_structure)
    # Pick some initial starting cube guaranteed to be within the perimeter.
    initial = Cube(cuboidal_structure[0].x+1, cuboidal_structure[0].y+1, cuboidal_structure[0].z+1)
    # Explore all the cubes given the constraints. This will be used to determine the exterior surface area.
    explored = explore(initial, constraints)
    # Count for each cube the number of exposed surfaces.
    return sum([len(list(filter(lambda neighbour: neighbour in explored, get_adjacent_cubes(cube)))) for cube in cubes])
