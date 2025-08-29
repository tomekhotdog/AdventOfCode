#[allow(dead_code)]

pub fn solution() {

    let inputs = read_input();
    let part_1_answer = part_1(&inputs);
    let part_2_answer = part_2(&inputs);

    println!("Q1 part 1: {:?}", part_1_answer);
    println!("Q2 part 2: {:?}", part_2_answer);

}

fn read_input() -> Vec<u64> {
    // base path is the crate root; this works from anywhere
    let input = include_str!(concat!(env!("CARGO_MANIFEST_DIR"), "/inputs/q1.txt"));
    input
        .lines()
        .map(|l| l.parse::<u64>().expect("bad number"))
        .collect()
}

fn part_1(measurements: &[u64]) -> usize {
    measurements.windows(2).filter(|w| w[1] > w[0]).count()
}

fn part_2(measurements: &[u64]) -> usize {
    // Compare [A B C] vs [B C D] by A vs D
    measurements.windows(4).filter(|w| w[3] > w[0]).count()
}

