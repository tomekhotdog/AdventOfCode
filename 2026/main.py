from typing import List


def read_input(filename: str) -> List[str]:
    with open('inputs/' + filename, 'r') as reader:
        return reader.read().splitlines()


def read_input_string(filename: str) -> str:
    with open('inputs/' + filename, 'r') as reader:
        return reader.read()


if __name__ == '__main__':
    print('AdventOfCode2026!')
