from main import read_input_string

def parse_input(filename):
    keys, locks = [], []
    schematics = read_input_string(filename).split('\n\n')
    for s in schematics:
        rows = s.split('\n')
        is_a_key = '#' not in rows[0]
        counts = [0, 0, 0, 0, 0]
        for row in rows[1:-1]:
            for i in range(len(row)):
                if row[i] == '#':
                    counts[i] += 1
        relevant_list = keys if is_a_key else locks
        relevant_list.append(counts)
    return keys, locks

def fit_together(key: [int], lock: [int]):
    assert len(key) == len(lock), f'Unexpected key/lock lengths!: Key={key} / Lock={lock}'
    return all([(key[i] + lock[i]) <= 5 for i in range(len(key))])

def part1() -> int:
    keys, locks = parse_input('q25.txt')
    return sum([fit_together(key, lock) for key in keys for lock in locks])


print(part1())