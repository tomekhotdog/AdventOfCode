from main import read_input

def parse_input(filename):
    lines = read_input(filename)
    towel_patterns = [x.strip() for x in lines[0].split(',')]
    desired = [x.split('\n')[0].strip() for x in lines[2:]]
    return towel_patterns, desired

def possible_ways(patterns, desired, memo):
    if len(desired) == 0:
        return 1
    if desired in memo:
        return memo[desired]

    reduced = [desired[len(p):] for p in patterns if desired.startswith(p)]
    memo[desired] = sum([possible_ways(patterns, x, memo) for x in reduced])

    return memo[desired]

def part1() -> int:
    patterns, desired = parse_input('q19.txt')
    known_outcomes = {}
    return len(list(filter(lambda x: possible_ways(patterns, x, known_outcomes) > 0, desired)))

def part2() -> int:
    patterns, desired = parse_input('q19.txt')
    known_outcomes = {}
    return sum([possible_ways(patterns, x, known_outcomes) for x in desired])


print(part1())
print(part2())