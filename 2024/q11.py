from main import read_input_string
from typing import List

def blink_stone(stone: int) -> List[int]:
    if stone == 0:
        return [1]
    elif len(str(stone)) % 2 == 0:
        divisor = int(10 ** (len(str(stone)) / 2))
        left = int(stone / divisor)
        right = stone % divisor
        return [left, right]
    else:
        return [stone * 2024]

def blink_n_times(stone: int, times: int, memo) -> int:
    if (stone, times) in memo:
        return memo[(stone, times)]
    elif times == 0:
        return 1
    else:
        result = sum([blink_n_times(s, times-1, memo) for s in blink_stone(stone)])
        memo[(stone, times)] = result
        return result

def read_stones(filename: str) -> List[int]:
    inputs = read_input_string(filename).split(' ')
    return [int(raw_stone) for raw_stone in inputs]

def part1() -> int:
    return sum([blink_n_times(stone, 25, {}) for stone in read_stones('q11.txt')])

def part2() -> int:
    return sum([blink_n_times(stone, 75, {}) for stone in read_stones('q11.txt')])

print(part1())
print(part2())