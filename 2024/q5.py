from main import read_input_string
from typing import List

class Rule:
    def __init__(self, raw_value: str):
        elems = raw_value.split('|')
        self.left = int(elems[0])
        self.right = int(elems[1])

    def __str__(self):
        return f"{self.left}|{self.right}"

def relevant_rules(update: List[int], rules: List[Rule]) -> List[Rule]:
    return list(filter(lambda rule: rule.left in update and rule.right in update, rules))

def fix_ordering(update: List[int], rules: List[Rule]) -> List[int]:
    original = update.copy()
    in_order = []
    while len(original) > 0:
        relevant = relevant_rules(original, rules)
        left_by_count = {}
        for rule in relevant:
            if rule.left in left_by_count:
                left_by_count[rule.left] += 1
            else:
                left_by_count[rule.left] = 0
        min_value = original[0] if len(original) == 1 else max(left_by_count, key=left_by_count.get)
        original.remove(min_value)
        in_order.append(min_value)
    return in_order

def part1() -> int:
    elems = read_input_string('q5.txt').split('\n\n')
    rules = [Rule(x) for x in elems[0].split('\n')]
    updates = list(map(lambda update: [int(x) for x in update.split(',')], elems[1].split('\n')))
    correct_updates = list(filter(lambda update: update == fix_ordering(update, rules), updates))
    middle_numbers = list(map(lambda update: update[int(len(update) / 2)], correct_updates))
    return sum(middle_numbers)


def part2() -> int:
    elems = read_input_string('q5.txt').split('\n\n')
    rules = [Rule(x) for x in elems[0].split('\n')]
    updates = list(map(lambda update: [int(x) for x in update.split(',')], elems[1].split('\n')))
    incorrect_updates = list(filter(lambda update: update != fix_ordering(update, rules), updates))
    corrected = list(map(lambda update: fix_ordering(update, rules), incorrect_updates))
    middle_numbers = list(map(lambda update: update[int(len(update) / 2)], corrected))
    return sum(middle_numbers)

print(part1())
print(part2())