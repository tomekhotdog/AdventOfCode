#[allow(dead_code)]

pub fn solution() {

    let instructions = parse_instructions();

    println!("Q3 part 1: {:?}", part_1(instructions.clone()));
    println!("Q3 part 2: {:?}", part_2(instructions.clone()));

}

fn parse_instructions() -> Vec<String> {
        let input = include_str!("../inputs/q3.txt");
        input
            .trim()
            .split('\n')
            .map(|l| String::from(l))
            .collect::<Vec<String>>()
}

fn column_bit_counts(rows: &Vec<String>) -> Vec<u32> {
	let elems_in_row = rows[0].len() as u32;
	let mut col_bit_counts = vec![0; elems_in_row as usize];
	for line in rows {
		for (index, elem) in line.chars().into_iter().enumerate() {
			col_bit_counts[index] += char::to_digit(elem, 2).unwrap();
		}
	}
	return col_bit_counts;
}

fn most_common_bit(rows: &Vec<String>, index: u32) -> u32 {
	let count: u32 = rows.iter().map(|row| char::to_digit(row.chars().nth(index as usize).unwrap(), 2).unwrap()).sum();
	return if (count as f32) >= (rows.len() as f32 / 2.0) { 1 } else { 0 }
}

fn part_1(rows: Vec<String>) -> usize {
	let elems_in_row = rows[0].len() as u32;
	let num_rows = rows.len();
	let col_bit_counts = column_bit_counts(&rows);

	let gamma_rate : usize = col_bit_counts
		.iter()
		.enumerate()
		.map(|(index, val)| if *val > (num_rows as u32 / 2) { usize::pow(2, elems_in_row - (index as u32) - 1) } else {0} )
		.sum();
	let epsilon_rate = gamma_rate ^ (usize::pow(2, elems_in_row ) - 1);

	return gamma_rate * epsilon_rate;
}

fn filter_with_bit_at_index(mut rows: Vec<String>, index: u32, bit: u32) -> Vec<String> {
	return rows
		.drain(..)
		.filter(|row| char::to_digit(row.chars().nth(index as usize).unwrap(), 2).unwrap() == bit)
		.collect();
}

fn binary_string_to_decimal(binary_string: &String) -> usize {
	return binary_string
		.chars()
		.rev()
		.enumerate()
		.map(|(i, x)| if x == '1' { usize::pow(2, i as u32) } else { 0 } )
		.sum();
}

fn part_2(rows: Vec<String>) -> u32 {
	let mut oxygen_rows = rows.clone();
	let mut current_index = 0;
	while oxygen_rows.len() > 1 {
		let most_common_bit = most_common_bit(&oxygen_rows, current_index);
		oxygen_rows = filter_with_bit_at_index(oxygen_rows, current_index, most_common_bit);
		current_index += 1;
	}

	let mut co2_rows = rows.clone();
	current_index = 0;
	while co2_rows.len() > 1 {
		let least_common_bit = most_common_bit(&co2_rows, current_index) ^ 1;
		co2_rows = filter_with_bit_at_index(co2_rows, current_index, least_common_bit);
		current_index += 1;
	}

	let oxygen_generator_rating = binary_string_to_decimal(&oxygen_rows[0]);
	let co2_scrubber_rating = binary_string_to_decimal(&co2_rows[0]);
	return oxygen_generator_rating as u32 * co2_scrubber_rating as u32;
}

