from __future__ import annotations
from main import read_input
from typing import List


class Position:
    def __init__(self, x, y):
        self.X = x
        self.Y = y

    def __str__(self):
        return '(' + str(self.X).zfill(2) + ', ' + str(self.Y).zfill(2) + ')'

    def __eq__(self, other):
        return self.X == other.X and self.Y == other.Y

    def __hash__(self):
        return hash(str(self.X)) + hash(str(self.Y))


# [Start, End] inclusive boundary for a single row,
class Range:
    def __init__(self, x1: int, x2: int):
        self.x1 = x1
        self.x2 = x2

    def __str__(self):
        return '[' + str(self.x1) + ',' + str(self.x2) + ']'

    def __eq__(self, other):
        return self.x1 == other.x1 and self.x2 == other.x2

    def __lt__(self, other):
        return self.x1 < other.x1 or (self.x1 == other.x1 and self.x2 < other.x2)

    def overlaps(self, other):
        return (other.x1 <= self.x1 <= other.x2) or (other.x1 <= self.x2 <= other.x2)

    def combine(self, other):
        if not (self.overlaps(other) or other.overlaps(self)):
            raise (str(self) + " does not overlap with " + str(other) + "! (Cannot combine.)")
        return Range(min(self.x1, other.x1), max(self.x2, other.x2))


class Sensor:
    def __init__(self, sensor_position: Position, beacon_position: Position):
        self.sensor_pos = sensor_position
        self.beacon_pos = beacon_position
        self.distance = abs(sensor_position.X - beacon_position.X) + abs(sensor_position.Y - beacon_position.Y)

    def __str__(self):
        return 'S = ' + str(self.sensor_pos) + ". B = " + str(self.beacon_pos) +\
               '. Manhattan distance = ' + str(self.distance).zfill(2)


class Map:
    def __init__(self, sensors: List[Sensor]):
        self.sensors = sensors

    def __str__(self):
        return "\n".join([str(sensor) for sensor in self.sensors])


# Returns a list of Ranges representing the area covered by the sensors on the map for the given row.
def calculate_coverage_for_row(m: Map, y: int) -> List[Range]:
    ranges = []
    for sensor in m.sensors:
        y_delta = abs(sensor.sensor_pos.Y - y)
        if sensor.distance < y_delta:
            continue
        x_delta = sensor.distance - y_delta
        ranges.append(Range(sensor.sensor_pos.X - x_delta, sensor.sensor_pos.X + x_delta))
    return ranges


# Returns the total combined coverage for a given list of ranges.
def calculate_combined_ranges(ranges: List[Range]) -> List[Range]:
    if len(ranges) == 0:
        return 0
    ranges.sort()
    non_overlapping = []
    current = ranges.pop(0)
    while len(ranges) > 0:
        next = ranges.pop(0)
        if current.overlaps(next) or next.overlaps(current):
            current = current.combine(next)
        else:
            non_overlapping.append(current)
            current = next
    non_overlapping.append(current)
    return non_overlapping


def count_number_of_beacons_in_ranges(map: Map, ranges: List[Range], y: int):
    beacons = []
    for elem in [sensor.beacon_pos for sensor in map.sensors]:
        for r in ranges:
            if elem.Y == y and r.x1 <= elem.X <= r.x2:
                beacons.append(elem)
    return len(set(beacons))


def trim_ranges(ranges: List[Range], min_x: int, max_x: int) -> List[Range]:
    return [Range(max(r.x1, min_x), min(r.x2, max_x)) for r in ranges]


def parse_input_line_section(section: str) -> Position:
    bits = section.split('=')
    return Position(int(bits[1].split(',')[0]), int(bits[2]))


def parse_input_line(line: str) -> Sensor:
    sections = line.split(':')
    return Sensor(parse_input_line_section(sections[0]), parse_input_line_section(sections[1]))


def parse_input() -> Map:
    return Map([parse_input_line(line) for line in read_input('q15.txt')])


def part1() -> int:
    row_under_inspection = 2000000
    the_map = parse_input()
    print(str(the_map))
    ranges = calculate_coverage_for_row(the_map, row_under_inspection)
    combined_ranges = calculate_combined_ranges(ranges)
    size_of_coverage = sum([nor.x2 - nor.x1 + 1 for nor in combined_ranges])
    beacons_in_ranges = count_number_of_beacons_in_ranges(the_map, combined_ranges, row_under_inspection)
    return size_of_coverage - beacons_in_ranges


# Remark: takes ~30s to run.
def part2() -> int:
    the_map = parse_input()
    max_range = 4000000
    tuning_frequency = 0

    for y in range(0, max_range, 1):
        ranges = calculate_coverage_for_row(the_map, y)
        combined_ranges = calculate_combined_ranges(ranges)
        trimmed_ranges = trim_ranges(combined_ranges, 0, max_range)
        size_of_coverage = sum([nor.x2 - nor.x1 + 1 for nor in trimmed_ranges])
        if size_of_coverage != max_range + 1:
            # There will be two ranges, the x-coordinates is between the two. For example: [[1, 4], [6, 9]] -> 5
            x_coordinate = trimmed_ranges[1].x1 - 1
            tuning_frequency = x_coordinate * 4000000 + y
            print("Row " + str(y) + ": Tuning freq = " + str(tuning_frequency))

    return tuning_frequency
