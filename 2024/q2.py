from main import read_input

def increasing(report: [int]) -> bool:
    for i in range(1, len(report)):
        if not report[i] > report[i-1]:
            return False
    return True

def decreasing(report: [int]) -> bool:
    for i in range(1, len(report)):
        if not report[i] < report[i - 1]:
            return False
    return True

def in_range(report: [int], min_diff: int, max_diff: int) -> bool:
    for i in range(1, len(report)):
        difference = abs(report[i] - report[i-1])
        if difference < min_diff or difference > max_diff:
            return False
    return True

def single_report_safe(levels: [int]) -> bool:
    return (increasing(levels) or decreasing(levels)) and in_range(levels, 1, 3)

def report_safe(report: str, problem_dampening: bool):
    levels = list(map(lambda x: int(x), report.split()))
    if not problem_dampening:
        return single_report_safe(levels)
    else:
        for i in range(len(levels)):
            new_levels = levels[:i] + levels[i+1:]
            if single_report_safe(new_levels):
                return True
        return False

def part1() -> int:
    reports = read_input('q2.txt')
    return len(list(filter(lambda x: report_safe(x, False), reports)))

def part2() -> int:
    reports = read_input('q2.txt')
    return len(list(filter(lambda x: report_safe(x, True), reports)))

print(part2())