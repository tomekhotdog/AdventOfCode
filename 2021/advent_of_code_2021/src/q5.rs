#[allow(dead_code)]

pub fn solution() {

    let input = get_input();

    println!("Q5 part 1: {:?}", part_1(input.clone()));
    println!("Q5 part 2: {:?}", part_2(input.clone()));

}

fn get_input() -> Vec<String> {
        let input = include_str!("../inputs/q5_a.txt");
        input
            .trim()
            .split('\n')
            .map(|l| String::from(l))
            .collect::<Vec<String>>()
}

fn part_1(input: Vec<String>) -> u32 {
	return 0;
}

fn part_2(input: Vec<String>) -> u32 {
	return 0;
}