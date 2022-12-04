from main import read_input
import string


class Range:
    def __init__(self, description: string):
        elems = description.split('-')
        if len(elems) != 2:
            raise Exception("Unexpected format: " + description)
        self.Lower = int(elems[0])
        self.Upper = int(elems[1])


class Pair:
    def __init__(self, description: string):
        elems = description.split(',')
        if len(elems) != 2:
            raise Exception("Unexpected format: " + description)
        self.Item1 = Range(elems[0])
        self.Item2 = Range(elems[1])


def is_completely_overlapping(pair: Pair) -> bool:
    return (pair.Item1.Lower >= pair.Item2.Lower and pair.Item1.Upper <= pair.Item2.Upper) or \
           (pair.Item2.Lower >= pair.Item1.Lower and pair.Item2.Upper <= pair.Item1.Upper)


def is_partially_overlapping(pair: Pair) -> bool:
    return (pair.Item1.Lower <= pair.Item2.Lower <= pair.Item1.Upper) or \
           (pair.Item2.Lower <= pair.Item1.Lower <= pair.Item2.Upper)


def part1():
    return sum([is_completely_overlapping(Pair(pair_desc)) for pair_desc in read_input("q4.txt")])


def part2():
    return sum([is_partially_overlapping(Pair(pair_desc)) for pair_desc in read_input("q4.txt")])