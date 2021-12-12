// use std::collections::HashMap;

#[allow(dead_code)]

pub fn solution() {
    println!("Q7 part 1: {:?}", part_1());
    println!("Q7 part 2: {:?}", part_2());
}

fn parse() -> Vec<u64> {
    let input = include_str!("../inputs/q7_a.txt");
    input
        .trim()
        .split(',')
        .map(|l| str::parse::<u64>(l).unwrap() )
        .collect::<Vec<u64>>()
}

fn part_1() -> u64 {
	return 0;
}

fn part_2() -> u64 {
	return 0;
}