from main import read_input

class Grid:
    def __init__(self, coordinate_range, corrupted):
        self.range = coordinate_range
        self.corrupted = corrupted

    def neighbouring_locations(self, location, corrupted):
        x, y = location
        neighbouring = [(x-1, y), (x+1, y), (x, y-1), (x, y+1)]
        neighbouring = [(x, y) for (x, y) in neighbouring if 0 <= x <= self.range and 0 <= y <= self.range]
        return [x for x in neighbouring if x not in corrupted]

    # Returns whether the shortest path exists after simulating n falling bytes and its length if so.
    def shortest_path(self, num_bytes):
        corrupted = set(self.corrupted[:num_bytes])

        # Visits the location calculating shortest path by inspecting neighbours. Returns next locations to be visited.
        def visit(location, corrupted_bytes, shortest_paths_map):
            neighbouring = self.neighbouring_locations(location, corrupted_bytes)
            shortest_to_neighbour = min([shortest_paths_map[x] for x in neighbouring if x in shortest_paths_map])
            shortest_to_location = shortest_to_neighbour + 1
            shortest_paths_map[location] = shortest_to_location
            next_to_visit = [x for x in neighbouring if x not in shortest_paths_map or shortest_paths_map[x] > shortest_to_location]
            return next_to_visit

        end_point = (self.range, self.range)
        shortest_paths = { end_point: 0 }
        to_visit = self.neighbouring_locations(end_point, corrupted)
        while len(to_visit) > 0:
            neighbours_to_visit = visit(to_visit.pop(0), corrupted, shortest_paths)
            to_visit += [x for x in neighbours_to_visit if x not in to_visit]

        start_point = (0, 0)
        return (True, shortest_paths[start_point]) if start_point in shortest_paths else (False, 0)

def parse_byte_coordinates(filename):
    return [(int(x.split(',')[0]), int(x.split(',')[1])) for x in read_input(filename)]

def part1() -> int:
    byte_coordinates = parse_byte_coordinates('q18.txt')
    grid = Grid(70, byte_coordinates)
    _, path_length = grid.shortest_path(1024)
    return path_length

def part2() -> str:
    byte_coordinates = parse_byte_coordinates('q18.txt')
    grid = Grid(70, byte_coordinates)
    # TODO: Can perform a binary search over the n_bytes simulation.
    for n in range(1, len(byte_coordinates)):
        path_exists, _ = grid.shortest_path(n)
        if not path_exists:
            return str(byte_coordinates[n-1])
    raise Exception('Path to was never blocked after simulating all falling bytes!')

print(part1())
print(part2())