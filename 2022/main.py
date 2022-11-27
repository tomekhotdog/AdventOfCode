# This is a sample Python script.

# Press ⌃R to execute it or replace it with your code.
# Press Double ⇧ to search everywhere for classes, files, tool windows, actions, and settings.

from typing import List


def read_input(filename: str) -> List[str]:
    with open('inputs/' + filename, 'r') as reader:
        return reader.read().splitlines()


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    print("AdventOfCode2022!")



# See PyCharm help at https://www.jetbrains.com/help/pycharm/