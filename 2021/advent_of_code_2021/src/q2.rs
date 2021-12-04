#[allow(dead_code)]

pub fn solution() {

    let instructions = parse_instructions();

    println!("Q2 part 1: {:?}", part_1(&instructions));
    println!("Q2 part 2: {:?}", part_2(&instructions));

}

#[derive(Debug)]
enum Instruction {
    Forward(u64),
    Down(u64),
    Up(u64)
}

struct Position {
    horizontal: u64,
    depth: u64,
    aim: u64
}

fn parse_instruction(instruction: &str) -> Instruction {
    let elems = instruction.split(' ').collect::<Vec<&str>>();
    assert!(elems.len() == 2);
    let parsed_value = str::parse::<u64>(elems[1]).unwrap();
    let parsed_instruction = match elems[0] {
        "forward" => Instruction::Forward(parsed_value),
        "down"    => Instruction::Down(parsed_value),
        "up"      => Instruction::Up(parsed_value),
        _         => panic!("Unexpected instruction: {:?}", instruction)
    };
    return parsed_instruction
}

fn parse_instructions() -> Vec<Instruction> {
        let input = include_str!("../inputs/q2.txt");
        input
            .trim()
            .split('\n')
            .map(|l| parse_instruction(l))
            .collect::<Vec<Instruction>>()
}


fn part_1(instructions: &Vec<Instruction>) -> u64 {
    let mut submarine_position = Position { horizontal: 0, depth: 0, aim: 0 };
    for instruction in instructions {
        match instruction {
            Instruction::Forward(x) => submarine_position.horizontal += x,
            Instruction::Down(x)    => submarine_position.depth += x,
            Instruction::Up(x)      => submarine_position.depth -= x
        }
    }
    return submarine_position.horizontal * submarine_position.depth;
}

fn part_2(instructions: &Vec<Instruction>) -> u64 {
    let mut submarine_position = Position { horizontal: 0, depth: 0, aim: 0 };
    for instruction in instructions {
        match instruction {
            Instruction::Forward(x) => 
            {
                submarine_position.horizontal += x;
                submarine_position.depth += submarine_position.aim * x;
            },
            Instruction::Down(x)    => submarine_position.aim += x,
            Instruction::Up(x)      => submarine_position.aim -= x
        }
    }
    return submarine_position.horizontal * submarine_position.depth;
}


