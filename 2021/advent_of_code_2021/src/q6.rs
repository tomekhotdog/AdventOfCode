#[allow(dead_code)]

pub fn solution() {

    let input = get_input();

    println!("Q6 part 1: {:?}", part_1(input.clone()));
    println!("Q6 part 2: {:?}", part_2(input.clone()));

}

fn get_input() -> Vec<String> {
        let input = include_str!("../inputs/q6_a.txt");
        input
            .trim()
            .split('\n')
            .map(|l| String::from(l))
            .collect::<Vec<String>>()
}

fn part_1(input: Vec<String>) -> i32 {
	return 0;
}

fn part_2(input: Vec<String>) -> i32 {
	return 0;
}