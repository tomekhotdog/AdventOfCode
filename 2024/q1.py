from main import read_input

def get_input(filename: str) -> ([], []):
    lines = read_input(filename)
    left, right = [], []
    for line in lines:
        elems = line.split()
        left.append(int(elems[0]))
        right.append(int(elems[1]))

    return left, right

def part1() -> int:
    left, right = get_input('q1.txt')
    left.sort()
    right.sort()
    total_distance = 0

    for i in range(len(left)):
        total_distance += abs(left[i] - right[i])

    return total_distance

def part2() -> int:
    left, right = get_input('q1.txt')
    counts_right = {}
    for item in right:
        if item in counts_right:
            counts_right[item] += 1
        else:
            counts_right[item] = 1

    similarity = 0
    for elem in left:
        if elem in counts_right:
            similarity += elem * counts_right[elem]
    return similarity

print(part2())