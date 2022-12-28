from __future__ import annotations
from main import read_input_string
from typing import List
from enum import Enum, IntEnum
from functools import cmp_to_key


class PacketType(Enum):
    LIST = 1
    INTEGER = 2


class Packet:
    def __init__(self, packet_type: PacketType, elems: List[Packet] = None, elem: int = None):
        self.packet_type = packet_type
        self.elems = elems
        self.elem = elem

    def __eq__(self, other):
        return str(self) == str(other)

    def __hash__(self):
        return hash(str(self))

    def __str__(self):
        match self.packet_type:
            case PacketType.LIST:
                return '[' + ','.join([str(elem) for elem in self.elems]) + ']'
            case PacketType.INTEGER:
                return str(self.elem)
            case packet_type:
                raise Exception("Unexpected PacketType! " + str(packet_type))


class Pair:
    def __init__(self, left: Packet, right: Packet, index: int):
        self.left = left
        self.right = right
        self.index = index

    def __str__(self):
        return str(self.left) + '\n' + str(self.right)


# Given the string representation of a Packet (e.g. "[1,2,[3,4,5],6]"), return the indices of the
# element separators (commas) of the top level list (ignoring separators within nested lists).
def find_separator_indices(raw_input: str) -> List[int]:
    indices = []
    level = 0
    for index, elem in enumerate(raw_input):
        if elem == '[':
            level += 1
        elif elem == ']':
            level -= 1
        elif elem == ',' and level == 1:
            indices.append(index)
    return indices


def parse_packet(raw_input: str) -> Packet:
    if raw_input == '' or raw_input == '[]':
        return Packet(PacketType.LIST, elems=[])
    elif raw_input.startswith('['):
        separator_indices = find_separator_indices(raw_input)
        separator_indices.append(len(raw_input)-1)
        prev_index = 0
        elems = []
        for index in separator_indices:
            elems.append(parse_packet(raw_input[prev_index+1:index]))
            prev_index = index
        return Packet(PacketType.LIST, elems=elems)
    else:
        return Packet(PacketType.INTEGER, elem=int(raw_input))


def parse_pairs(raw_input: str) -> List[Pair]:
    pairs = []
    elems = raw_input.split('\n\n')
    for index, elem in enumerate(elems):
        parts = elem.split('\n')
        if len(parts) != 2:
            raise Exception("Failed to parse pair: " + str(elem))
        left = parse_packet(parts[0])
        right = parse_packet(parts[1])
        pairs.append(Pair(left, right, index+1))
    return pairs


class PacketOrdering(IntEnum):
    LESS_THAN = -1
    EQUAL = 0
    GREATER_THAN = 1


def compare_packets(left: Packet, right: Packet) -> PacketOrdering:
    if left.packet_type == PacketType.INTEGER and right.packet_type == PacketType.INTEGER:
        if left.elem < right.elem:
            return PacketOrdering.LESS_THAN
        elif left.elem > right.elem:
            return PacketOrdering.GREATER_THAN
        return PacketOrdering.EQUAL

    if left.packet_type == PacketType.LIST and right.packet_type == PacketType.LIST:
        for i in range(max(len(left.elems), len(right.elems))):
            # Exhausted elements in left list.
            if i >= len(left.elems):
                return PacketOrdering.LESS_THAN
            # Exhausted elements in right list.
            if i >= len(right.elems):
                return PacketOrdering.GREATER_THAN
            # Determine ordering for ith elem.
            elem_comparison = compare_packets(left.elems[i], right.elems[i])
            if elem_comparison != PacketOrdering.EQUAL:
                return elem_comparison
        return PacketOrdering.EQUAL

    if left.packet_type == PacketType.INTEGER and right.packet_type == PacketType.LIST:
        return compare_packets(Packet(PacketType.LIST, elems=[left]), right)
    if left.packet_type == PacketType.LIST and right.packet_type == PacketType.INTEGER:
        return compare_packets(left, Packet(PacketType.LIST, elems=[right]))
    else:
        raise Exception("Impossible case!")


def compare_packets_function(p1: Packet, p2: Packet) -> int:
    return int(compare_packets(p1, p2))


def part1():
    pairs = parse_pairs(read_input_string("q13.txt"))
    in_order_pairs = list(filter(lambda p: compare_packets(p.left, p.right) != PacketOrdering.GREATER_THAN, pairs))
    return sum([pair.index for pair in in_order_pairs])


def part2():
    pairs = parse_pairs(read_input_string("q13.txt"))
    packets = [p for pair in pairs for p in [pair.left, pair.right]]

    divider_packet_1 = parse_packet("[[2]]")
    divider_packet_2 = parse_packet("[[6]]")
    packets += [divider_packet_1, divider_packet_2]

    packets = sorted(packets, key=cmp_to_key(compare_packets_function))

    divider_packet_1_index = packets.index(divider_packet_1) + 1
    divider_packet_2_index = packets.index(divider_packet_2) + 1

    return divider_packet_1_index * divider_packet_2_index
