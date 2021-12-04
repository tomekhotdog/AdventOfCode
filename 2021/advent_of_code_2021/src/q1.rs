#[allow(dead_code)]

pub fn solution() {

    let inputs = read_input();
    let part_1_answer = part_1(&inputs);
    let part_2_answer = part_2(&inputs);

    println!("Q1 part 1: {:?}", part_1_answer);
    println!("Q2 part 2: {:?}", part_2_answer);

}

fn read_input() -> Vec<u64> {
        let input = include_str!("../inputs/q1.txt");
        input
            .trim()
            .split('\n')
            .map(|l| str::parse::<u64>(l).unwrap())
            .collect::<Vec<u64>>()
}

fn part_1(measurements: &Vec<u64>) -> usize {
	return measurements.windows(2).filter(|x| x[1] > x[0]).count();
}

fn part_2(measurements: &Vec<u64>) -> usize {
	// Comparing the sum of triples: [A B C] [B C D]. 
	// We can inspect just the first element of the first triple and last element of second.
	return measurements.windows(4).filter(|x| (x[3]) > x[0]).count()
}


