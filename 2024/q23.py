import itertools
from main import read_input

def parse_connections(filename):
    connections = {}
    for connection in read_input(filename):
        elems = connection.split('-')
        a, b = elems[0], elems[1]
        if a not in connections:
            connections[a] = set()
        connections[a].add(b)
        if b not in connections:
            connections[b] = set()
        connections[b].add(a)
    return connections

def historian_triples(connections):
    triples = set()
    candidates = [x for x in connections.keys() if x.startswith('t')]
    for t in candidates:
        for a in connections[t]:
            for b in connections[a]:
                if b in connections[t]:
                    triples.add(tuple(sorted([t, a, b])))
    return triples

def unique_subsets(computers, n):
    return list(set([tuple(sorted(x)) for x in itertools.combinations(computers, n)]))

def interconnected(connections, computers):
    for a in computers:
        for b in computers:
            if a != b and b not in connections[a]:
                return False
    return True

def largest_interconnected_subset(connections):
    largest = []
    for computer in connections.keys():
        connected = connections[computer]
        for n in range(len(connected), len(largest), -1):
            for subset in unique_subsets(connected, n):
                if len(subset) > len(largest) and interconnected(connections, subset):
                    largest = list(subset) + [computer]
    return largest

def part1() -> int:
    connections = parse_connections('q23.txt')
    return len(historian_triples(connections))

def part2() -> str:
    connections = parse_connections('q23.txt')
    lan_party = largest_interconnected_subset(connections)
    return ','.join(sorted(lan_party))

print(part1())
print(part2())