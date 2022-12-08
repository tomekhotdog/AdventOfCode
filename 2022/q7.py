from main import read_input
from enum import Enum
from typing import List

TOTAL_DISK_SPACE = 70_000_000
REQUIRED_SPACE_FOR_UPDATE = 30_000_000


class FileSystemNodeType(Enum):
    DIRECTORY = 'DIR'
    FILE = 'FILE'


class FileSystemNode:
    def __init__(self, parent, node_type: FileSystemNodeType, name: str, file_size: int):
        # Parent node.
        self.parent = parent
        # Identify whether the node is a directory or a file.
        self.node_type = node_type
        # Name of this node (directory or file name).
        self.name = name
        # The size of this file (or zero if this is a directory).
        self.file_size = file_size
        # Children nodes keyed by their names.
        self.children = {}
        # Total size of all files within directory and subdirectories. Value is -1 if not calculated.
        self.total_size = -1

    def __str__(self):
        match self.node_type:
            case FileSystemNodeType.FILE:
                return f"File. Name={self.name}. FileSize={self.file_size}. Parent={self.parent.name}."
            case FileSystemNodeType.DIRECTORY:
                return f"Directory. Name={self.name}. TotalSize={self.total_size}. Parent={self.parent.name}."

    def add_child(self, node):
        self.children[node.name] = node


def parse_filesystem(inputs: List[str]) -> FileSystemNode:
    root = FileSystemNode(None, FileSystemNodeType.DIRECTORY, "/", 0)
    current_node = root
    for input in inputs:
        if input.startswith("$ cd"):
            move_to_directory = input.split("cd")[1].strip()
            if move_to_directory == "/":
                current_node = root
            elif move_to_directory == "..":
                current_node = current_node.parent
            elif move_to_directory not in current_node.children:
                raise Exception("Could not cd to " + move_to_directory + " from " + current_node.name)
            else:
                current_node = current_node.children[move_to_directory]
            continue
        if input.startswith("$ ls"):
            continue
        else:
            if input.startswith("dir"):
                child_dir = input.split("dir")[1].strip()
                current_node.add_child(FileSystemNode(current_node, FileSystemNodeType.DIRECTORY, child_dir, 0))
            else:
                file_description_elems = input.split(" ")
                if len(file_description_elems) != 2:
                    raise Exception("Unexpected file description: " + input)
                filesize = int(file_description_elems[0])
                filename = file_description_elems[1]
                current_node.add_child(FileSystemNode(current_node, FileSystemNodeType.FILE, filename, filesize))
    return root


# Traverse filesystem from given node to calculate file sizes.
def calculate_total_sizes(node: FileSystemNode) -> int:
    match node.node_type:
        case FileSystemNodeType.FILE:
            node.total_size = node.file_size
            return node.total_size
        case FileSystemNodeType.DIRECTORY:
            if node.total_size == -1:
                current_total = 0
                for child in node.children.values():
                    current_total += calculate_total_sizes(child)
                node.total_size = current_total
            return node.total_size


def sum_directory_sizes(node: FileSystemNode, maximum_size: int) -> int:
    cumulative_dir_sizes = 0
    if node.node_type == FileSystemNodeType.FILE:
        return 0
    if node.total_size <= maximum_size:
        cumulative_dir_sizes += node.total_size
    for child in node.children.values():
        cumulative_dir_sizes += sum_directory_sizes(child, maximum_size)
    return cumulative_dir_sizes


# Parse filesystem and traverse to calculate cumulative sizes.
def analyse_filesystem() -> FileSystemNode:
    inputs = read_input("q7.txt")
    root = parse_filesystem(inputs)
    calculate_total_sizes(root)
    return root


# Traverse the filesystem and keep note of the directory sizes.
def calculate_directory_sizes(node: FileSystemNode, dir_sizes: dict):
    if node.node_type == FileSystemNodeType.FILE:
        return
    dir_sizes[node.name] = node.total_size
    for child in node.children.values():
        calculate_directory_sizes(child, dir_sizes)


def part1():
    return sum_directory_sizes(analyse_filesystem(), 100000)


def part2():
    root = analyse_filesystem()
    dir_sizes = {}
    calculate_directory_sizes(root, dir_sizes)
    required_to_delete = root.total_size - (TOTAL_DISK_SPACE - REQUIRED_SPACE_FOR_UPDATE)
    return min(list(filter(lambda x: x > required_to_delete, dir_sizes.values())))
