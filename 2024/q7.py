from main import read_input
from typing import List

def evaluate(operand1: int, operand2: int, operator: chr):
    if operator == '+':
        return operand1 + operand2
    elif operator == '*':
        return operand1 * operand2
    elif operator == '||':
        return (operand1 * (10 ** len(str(operand2)))) + operand2
    else:
        raise Exception(f"Unknown operator: {operator}")

class Equation:
    def __init__(self, result: int, operands: List[int]):
        self.result = result
        self.operands = operands

    def solvable(self, operators: List[chr]) -> bool:
        running_totals = [self.operands[0]]
        for operand in self.operands[1:]:
            new_totals = []
            for operator in operators:
                for running_total in running_totals:
                    new_totals.append(evaluate(running_total, operand, operator))
            running_totals = new_totals
        return any(filter(lambda x: x == self.result, running_totals))

def parse_input(raw_equations: List[str]) -> List[Equation]:
    equations = []
    for raw in raw_equations:
        elems = raw.split(':')
        result = int(elems[0])
        operands = [int(elem) for elem in elems[1].strip().split(' ')]
        equations.append(Equation(result, operands))
    return equations

def part1() -> int:
    equations = parse_input(read_input('q7.txt'))
    operators = ['+', '*']
    return sum([e.result for e in equations if e.solvable(operators)])

def part2() -> int:
    equations = parse_input(read_input('q7.txt'))
    operators = ['+', '*', '||']
    return sum([e.result for e in equations if e.solvable(operators)])

print(part1())
print(part2())