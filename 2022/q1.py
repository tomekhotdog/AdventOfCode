import typing
from main import read_input


def part1():
    lines = read_input("q1_example.txt")
    current_total_max = 0
    current_total = 0
    for line in lines:
        if line == "":
            current_total_max = current_total_max if current_total_max > current_total else current_total
            current_total = 0
        else:
            current_total += int(line)
    return current_total_max


def part2():
    lines = read_input("q1.txt")
    lines.append("")
    max_items = [0, 0, 0]
    current_total = 0
    for line in lines:
        if line == "":
            if current_total > min(max_items):
                max_items.remove(min(max_items))
                max_items.append(current_total)
            current_total = 0
        else:
            current_total += int(line)
    return sum(max_items)