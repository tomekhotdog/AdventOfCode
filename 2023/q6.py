from typing import List
from main import read_input
from functools import reduce
import math


class Race:
    def __init__(self, time: int, record_distance):
        self.race_time = time
        self.record_distance = record_distance

    def __str__(self):
        return f"T={self.race_time}, D={self.record_distance}"


def parse_input(filename: str) -> List[Race]:
    lines = read_input(filename)
    times = [int(x) for x in lines[0].split(':')[1].strip().split()]
    distances = [int(x) for x in lines[1].split(':')[1].strip().split()]
    return [Race(t, d) for t, d in zip(times, distances)]


def ways_to_beat_record(rr: Race) -> int:
    # We have a quadratic to solve: distance = (time - release_time) * t.
    # e.g. find t such that: t^2 - Tt + d_r > 0 (t = release time, T = race time, d_r = distance record).
    determinant = math.pow(rr.race_time, 2) - (4 * rr.record_distance)
    sqrt_det = math.pow(determinant, 0.5)
    small_increment = 0.000000000001  # To capture strict inequality.
    upper_solution = ((rr.race_time + sqrt_det) / 2) - small_increment
    lower_solution = ((rr.race_time - sqrt_det) / 2) + small_increment
    return math.ceil(upper_solution) - math.ceil(lower_solution)


def part1() -> int:
    return reduce(lambda x, y: x * y, [ways_to_beat_record(x) for x in parse_input('q6.txt')])


def part2() -> int:
    return ways_to_beat_record(Race(58996469, 478223210191071))
