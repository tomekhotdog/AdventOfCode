use std::collections::HashSet;
use std::collections::HashMap;
use std::iter::FromIterator;

#[allow(dead_code)]

pub fn solution() {
    println!("Q8 part 1: {:?}", part_1());
    println!("Q8 part 2: {:?}", part_2());
}

struct Input {
	digits: Vec<String>,
	display: Vec<String>
}

impl Input {
    pub fn new(input: String) -> Self {
    	let elems = input.split('|').map(|x| x.to_string()).collect::<Vec<String>>();
	    assert!(elems.len() == 2);
    	let digits = elems[0].trim().split(' ').map(|x| x.to_string()).collect::<Vec<String>>();
    	let display = elems[1].trim().split(' ').map(|x| x.to_string()).collect::<Vec<String>>();

        Self { digits, display }
    }
}

fn parse_input() -> Vec<Input> {
    let input = include_str!("../inputs/q8.txt");
    let elems = input
    		.trim()
    		.split('\n')
    		.map(|line| Input::new(line.to_string()))
    		.collect::<Vec<Input>>();
    return elems;
}

//   0:      1:      2:      3:      4:
//  aaaa    ....    aaaa    aaaa    ....
// b    c  .    c  .    c  .    c  b    c
// b    c  .    c  .    c  .    c  b    c
//  ....    ....    dddd    dddd    dddd
// e    f  .    f  e    .  .    f  .    f
// e    f  .    f  e    .  .    f  .    f
//  gggg    ....    gggg    gggg    ....

//   5:      6:      7:      8:      9:
//  aaaa    aaaa    aaaa    aaaa    aaaa
// b    .  b    .  .    c  b    c  b    c
// b    .  b    .  .    c  b    c  b    c
//  dddd    dddd    ....    dddd    dddd
// .    f  e    f  .    f  e    f  .    f
// .    f  e    f  .    f  e    f  .    f
//  gggg    gggg    ....    gggg    gggg

fn is_1(digit: String) -> bool {
	return digit.len() == 2;
}

fn is_4(digit: String) -> bool {
	return digit.len() == 4;
}

fn is_7(digit: String) -> bool {
	return digit.len() == 3;
}

fn is_8(digit: String) -> bool {
	return digit.len() == 7;
}

fn part_1() -> i64 {
	let inputs: Vec<Input> = parse_input();
	let unique_digits: i64 = 
		inputs
			.iter()
			.map(|input| input.display.iter()
				.filter(|display| is_1(display.to_string()) || is_4(display.to_string()) || 
							      is_7(display.to_string()) || is_8(display.to_string()))
				.count())
			.sum::<usize>() as i64;

	return unique_digits;
}

fn map_display(display: Vec<char>, segment_map: &HashMap<char, char>) -> Vec<char> {
	let mut mapped: Vec<char> = Vec::new();
	for elem in display.into_iter() {
		if !segment_map.contains_key(&elem) {
			panic!("Failed to find elem ({:?}) in segment_map! {:?}", elem, segment_map);
		}
		mapped.push(segment_map[&elem]);
	}
	return mapped;
}

fn calculate_display_value(input: &String, segments: &HashMap<char, char>) -> i64 {
	let mapped: Vec<char> = map_display(input.chars().into_iter().collect::<Vec<char>>(), &segments);
	// println!("mapped: {:?}", mapped);
	if mapped.len() == 6 {
		if !mapped.contains(&'d') {
			return 0;
		}
		if !mapped.contains(&'c') {
			return 6;
		}
		if !mapped.contains(&'e') {
			return 9;
		}
		panic!("Assumption failure for display with length 6!");
	}
	if mapped.len() == 5 {
		if !mapped.contains(&'f') {
			return 2;
		}
		if !mapped.contains(&'b') {
			return 3;
		}
		if !mapped.contains(&'c') {
			return 5;
		}
		panic!("Assumption failure for display with length 5!")
	}
	if mapped.len() == 2 {
		return 1;
	}
	if mapped.len() == 3 {
		return 7;
	}
	if mapped.len() == 4 {
		return 4;
	}
	if mapped.len() == 7 {
		return 8;
	}
	panic!("Assumption failure! {:?}", mapped);
}

fn decipher_input(input: &Input) -> i64 {
	let digit_1: HashSet<char> = input.digits.iter().find(|x| is_1(x.to_string())).unwrap().chars().collect();
	let digit_4: HashSet<char> = input.digits.iter().find(|x| is_4(x.to_string())).unwrap().chars().collect();
	let digit_7: HashSet<char> = input.digits.iter().find(|x| is_7(x.to_string())).unwrap().chars().collect();
	let digits_0_6_9: Vec<HashSet<char>> = input.digits.iter().filter(|x| x.len() == 6).map(|x| x.chars().collect()).collect();

	let set_difference: HashSet<&char> = digit_7.difference(&digit_1).collect();
	let a = *set_difference.into_iter().next().unwrap();
	let set_a: HashSet<char> = vec![a].into_iter().collect();

	// The segment representing 'e' is the set difference between digit 6 and digit 9 (which are the digits with 6 segments).
	let digit_6 = digits_0_6_9.iter().find(|x|(digit_1.difference(&x).copied().collect::<HashSet<char>>()).len() > 0).unwrap();
	let digit_9 = digits_0_6_9.iter().find(|x|(digit_4.difference(&x).copied().collect::<HashSet<char>>()).len() == 0).unwrap();
	let set_difference_6_9: HashSet<&char> = digit_6.difference(&digit_9).collect();
	let e = *set_difference_6_9.into_iter().next().unwrap();
	let set_difference_9_6: HashSet<&char> = digit_9.difference(&digit_6).collect();
	let c = *set_difference_9_6.into_iter().next().unwrap();

	let set_c: HashSet<char> = vec![c].into_iter().collect();
	let set_difference: HashSet<&char> = digit_1.difference(&set_c).collect();
	let f = *set_difference.into_iter().next().unwrap();

	let set_difference_9_4: HashSet<char> = digit_9.difference(&digit_4).copied().collect();
	let set_difference_9_4_a: HashSet<&char> = set_difference_9_4.difference(&set_a).collect();

	let g = *set_difference_9_4_a.into_iter().next().unwrap();

	let digit_0 = digits_0_6_9.iter().find(|x| (x.difference(&digit_6).collect::<HashSet<&char>>()).len() > 0 && (x.difference(&digit_9).collect::<HashSet<&char>>()).len() > 0).unwrap();
	let set_difference_9_0: HashSet<&char> = digit_9.difference(&digit_0).collect();
	let d = *set_difference_9_0.into_iter().next().unwrap();

	let set_c_d_f: HashSet<char> = vec![c,d,f].into_iter().collect();
	let set_difference_4_c_d_f: HashSet<&char> = digit_4.difference(&set_c_d_f).collect();
	let b = *set_difference_4_c_d_f.into_iter().next().unwrap();

    let segment_map: HashMap<_, _> = HashMap::from_iter([(a, 'a'), (b, 'b'), (c, 'c'), (d, 'd'), (e, 'e'), (f, 'f'), (g, 'g')]);

	return input.display
		.iter()
		.rev()
		.enumerate()
		.map(|(i,x)| i64::pow(10, i as u32) * calculate_display_value(x, &segment_map))
		.sum();
}

fn part_2() -> i64 {
	let mut inputs: Vec<Input> = parse_input();
	return inputs.iter_mut().map(|i| decipher_input(i)).sum::<i64>();
}

