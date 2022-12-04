from typing import List
from main import read_input
import string


# Given a list of strings return the single character that appears in all strings.
def find_duplicate_char(compartments: List[str]) -> chr:
    duplicates = set(string.ascii_letters)
    [duplicates := duplicates.intersection(set(compartment)) for compartment in compartments]
    if len(duplicates) != 1:
        raise Exception("Multiple duplicates found! " + str(duplicates))
    return list(duplicates)[0]


# Given a string representing the items in a rucksack return the item that appears in both compartments.
def find_rucksack_duplicate(rucksack: str) -> chr:
    if len(rucksack) % 2 != 0:
        raise Exception("Rucksack had odd number of items: " + rucksack + ". (Length=" + str(len(rucksack)) + ").")
    middle_idx = int(len(rucksack) / 2)
    return find_duplicate_char([rucksack[:middle_idx], rucksack[middle_idx:]])


# Calculate item priority.
def item_priority(item: chr) -> int:
    if item.islower():
        return ord(item) - ord('a') + 1
    elif item.isupper():
        return ord(item) - ord('A') + 27
    else:
        raise Exception("Unexpected character: " + item)


def part1():
    rucksacks = read_input("q3.txt")
    return sum(list(map(lambda rucksack: item_priority(find_rucksack_duplicate(rucksack)), rucksacks)))


def part2():
    rucksacks = read_input("q3.txt")
    grouped = [rucksacks[i:i + 3] for i in range(0, len(rucksacks), 3)]
    return sum(list(map(lambda group: item_priority(find_duplicate_char(group)), grouped)))
