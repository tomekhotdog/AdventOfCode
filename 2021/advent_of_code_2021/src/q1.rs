use crate::tools::read_lines as read_lines;

pub fn solution() {

    let inputs = read_numbers(String::from("../inputs/q1.txt"));
    let part_1_answer = part_1(inputs.clone());
    let part_2_answer = part_2(inputs.clone());

    println!("Answer to part 1: {:?}", part_1_answer);
    println!("Answer to part 2: {:?}", part_2_answer);

}

fn part_1(measurements: Vec<i32>) -> i32 {
	let mut maybe_previous : Option<i32> = None;
    let mut increased_measurements = 0;

	for measurement in measurements {
	    match maybe_previous {
	        Some(previous) => if measurement > previous { increased_measurements += 1; },
	        None => {},
	    }
	    maybe_previous = Some(measurement);
	}
	return increased_measurements;
}

fn part_2(measurements: Vec<i32>) -> i32 {
	let mut maybe_previous_window : Option<i32> = None;
	let mut increased_measurements = 0;

	for i in 2..measurements.len() {
		let window_sum = measurements[i] + measurements[i-1] + measurements[i-2];
		match maybe_previous_window {
			Some(previous) => if window_sum > previous { increased_measurements += 1; },
			None => {},
		}
		maybe_previous_window = Some(window_sum);
	}
	return increased_measurements;
}

fn read_numbers(file_path:String) -> Vec<i32> {
    let mut inputs: Vec<i32> = Vec::new();
    if let Ok(lines) = read_lines(file_path) {
        for line in lines {
        	if let Ok(value) = line {
        		inputs.push(value.parse().unwrap())
        	}    		
        }
    }
    return inputs;
}