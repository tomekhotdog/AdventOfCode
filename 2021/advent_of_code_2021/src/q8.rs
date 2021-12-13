#[allow(dead_code)]

pub fn solution() {
    println!("Q8 part 1: {:?}", part_1());
    println!("Q8 part 2: {:?}", part_2());
}

fn parse_input() -> Vec<i64> {
    let input = include_str!("../inputs/q8_a.txt");
    input
        .trim()
        .split(',')
        .map(|l| str::parse::<i64>(l).unwrap() )
        .collect::<Vec<i64>>()
}

fn part_1() -> i64 {
	return 0;
}

fn part_2() -> i64 {
	return 0;
}