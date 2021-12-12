use std::collections::HashMap;

#[allow(dead_code)]

pub fn solution() {
    println!("Q6 part 1: {:?}", part_1());
    println!("Q6 part 2: {:?}", part_2());
}

fn make_fish() -> Vec<u64> {
    let input = include_str!("../inputs/q6.txt");
    input
        .trim()
        .split(',')
        .map(|l| str::parse::<u64>(l).unwrap() )
        .collect::<Vec<u64>>()
}

fn count_additional_fish(initial: u64, days: u64, memo: &mut HashMap<(u64,u64), u64>) -> u64 {
	let key = (initial, days);
	// Selected memoized result if one exists.
	if memo.contains_key(&key) { return *memo.get(&key).unwrap(); }
	// No more fish will be spawned in the days remaining.
	if initial >= days { return 0; 	}
	// Recursive call that considers the fish spawned from the current one, and the immediate offspring.
	let days_remaining = days - initial - 1;
	let ans = count_additional_fish(6, days_remaining, memo) + 1 + count_additional_fish(8, days_remaining, memo);
	// Memoise result.
	memo.insert(key, ans);
	return ans
}

fn count_fish_with_initial_state(initial_fish: Vec<u64>, days: u64) -> u64 {
    let mut memo: HashMap<(u64,u64),u64> = HashMap::new();
	return initial_fish.into_iter().map(|initial_timer| 1 + count_additional_fish(initial_timer, days, &mut memo)).sum::<u64>() as u64;
}

fn part_1() -> u64 {
	return count_fish_with_initial_state(make_fish().clone(), 80);
}

fn part_2() -> u64 {
	return count_fish_with_initial_state(make_fish().clone(), 256);
}