from typing import List
from main import read_input_string


# Returns whether the list contains only unique values.
def chars_unique(input: List[chr]) -> bool:
    return len(set(input)) == len(input)


def find_marker_position(input: str, marker_length: int) -> int:
    if marker_length > len(input):
        raise Exception("Window size (" + str(marker_length) + ") greater than input length! ")
    current_position = marker_length
    while current_position < len(input):
        signal_candidate = list(input[current_position-marker_length:current_position])
        if chars_unique(signal_candidate):
            return current_position
        current_position += 1
    raise Exception("Failed to find signal in input.")


def part1():
    return find_marker_position(read_input_string("q6.txt"), 4)


def part2():
    return find_marker_position(read_input_string("q6.txt"), 14)
