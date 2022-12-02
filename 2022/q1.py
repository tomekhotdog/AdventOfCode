from typing import List
from main import read_input


def calculate_group_sums(raw_input: List[str]) -> List[int]:
    # Construct a single string with all the elements then split on
    # each grouping of items (separated by a double underscore).
    grouped = '_'.join(raw_input).split("__")
    # For each grouping, extract the integer values and sum together.
    return [sum(list(map(lambda x: int(x), elem.split('_')))) for elem in grouped]


def part1() -> int:
    return max(calculate_group_sums(read_input("q1.txt")))


def part2() -> int:
    group_sums = calculate_group_sums(read_input("q1.txt"))
    group_sums.sort(reverse=True)
    # Sum the top three values.
    return sum(group_sums[0:3])
