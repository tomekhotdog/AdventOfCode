use std::collections::HashSet;

#[allow(dead_code)]

pub fn solution() {
    println!("Q10 part 1: {:?}", part_1());
    println!("Q10 part 2: {:?}", part_2());
}

fn parse_input() -> Vec<Vec<u32>> {
    let input = include_str!("../inputs/q10.txt");
    input
        .trim()
        .split('\n')
        .map(|l| l.chars().map(|x| x.to_digit(10).unwrap()).collect::<Vec<u32>>())
        .collect::<Vec<Vec<u32>>>()
}

fn part_1() -> u32 {
	return 0;
}

fn part_2() -> u32 {
	return 0;
}