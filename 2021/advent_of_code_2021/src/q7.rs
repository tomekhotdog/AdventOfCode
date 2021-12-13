#[allow(dead_code)]

pub fn solution() {
    println!("Q7 part 1: {:?}", part_1());
    println!("Q7 part 2: {:?}", part_2());
}

fn parse_input() -> Vec<i64> {
    let input = include_str!("../inputs/q7.txt");
    input
        .trim()
        .split(',')
        .map(|l| str::parse::<i64>(l).unwrap() )
        .collect::<Vec<i64>>()
}

fn fuel_cost_part_1(positions: Vec<i64>, position: i64) -> i64 {
	return positions.iter().map(|x| i64::abs(position - x) as i64).sum();
}

fn sum_1_to_n(n: i64) -> i64 {
	return (n * (n + 1)) / 2;
}

fn fuel_cost_part_2(positions: Vec<i64>, position: i64) -> i64 {
	return positions.into_iter().map(|x| sum_1_to_n(i64::abs(position - x))).sum();
}

fn part_1() -> i64 {
	let mut positions = parse_input();
	// The median minimises the L1 norm.
	// (see: https://math.stackexchange.com/questions/113270/the-median-minimizes-the-sum-of-absolute-deviations-the-ell-1-norm)
	positions.sort();
	let median = positions[(positions.len() as f64 / 2.0).round() as usize];
	return fuel_cost_part_1(positions.clone(), median);
}

fn part_2() -> i64 {
	let positions = parse_input();
	// The mean minimises the L2 norm.
	// (see: https://math.stackexchange.com/questions/696622/intuition-on-why-the-average-minimizes-the-euclidean-distance)
	let mean = (positions.iter().sum::<i64>() as f64 / (positions.len() as f64)).round() as i64;	
	// Not sure why it's not the rounded mean! But pick the smallest either side.
	return fuel_cost_part_2(positions.clone(), (vec![mean-1, mean, mean+1]).into_iter().min().unwrap());
}