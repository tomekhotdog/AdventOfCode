from main import read_input, read_input_string
import re

def extract(input: str, pattern: str) -> [str]:
    return re.findall(pattern, input)

def evaluate_mul(expression: str) -> int:
    items = expression.split('mul(')[1].split(')')[0].split(',')
    assert(len(items) == 2, f"Expression should be of form mul(X,Y) - was: {expression}")
    return int(items[0]) * int(items[1])

def part1() -> int:
    input = read_input('q3.txt')
    pattern = r'mul\(\d{1,3},\d{1,3}\)'
    matches = [extract(line, pattern) for line in input]
    matches = [item for sublist in matches for item in sublist]
    return sum(list(map(lambda x: evaluate_mul(x), matches)))

def part2() -> int:
    input = read_input('q3.txt')
    pattern = r'mul\(\d{1,3},\d{1,3}\)|do\(\)|don\'t\(\)'
    matches = [extract(line, pattern) for line in input]
    matches = [item for sublist in matches for item in sublist]

    total = 0
    enabled = True
    for match in matches:
        if match == 'do()':
            enabled = True
        elif match == 'don\'t()':
            enabled = False
        elif enabled:
            total += evaluate_mul(match)

    return total