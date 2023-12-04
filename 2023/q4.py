from typing import List
from main import read_input
import math


class ScratchCard:
    def __init__(self, card_number: int, winners: List[int], selected: List[int]):
        self.number = card_number
        self.winners = set(winners)
        self.selected = set(selected)

    def __str__(self):
        return f"{self.number}: {self.winners} | {self.selected}"

    def matching(self):
        return len(list(filter(lambda x: x in self.winners, self.selected)))

    # Defined as 2^(n-1), where n is the number of matching numbers.
    def score(self):
        return int(round(math.pow(2, self.matching()-1)))


def parse_input(filename: str) -> List[ScratchCard]:
    inputs = read_input(filename)
    cards = []
    for line in inputs:
        elems = line.split(':')
        card_number = int(elems[0].split()[-1])
        numbers = elems[1].split('|')
        winners = [int(x) for x in numbers[0].split()]
        selected = [int(x) for x in numbers[1].split()]
        cards.append(ScratchCard(card_number, winners, selected))
    return cards


def part1() -> int:
    return sum(list(map(lambda x: x.score(), parse_input('q4.txt'))))


def part2() -> int:
    cards_to_traverse = parse_input('q4.txt')
    cards_to_traverse.reverse()
    card_points = {}
    for card in cards_to_traverse:
        points = 1 + sum([card_points[x] for x in range(card.number+1, card.number+card.matching()+1, 1)])
        card_points[card.number] = points
    return sum(list(card_points.values()))
