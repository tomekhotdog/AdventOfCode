use crate::tools::read_lines as read_lines;

pub fn solution() {

    let inputs = read_numbers(String::from("../inputs/q1.txt"));
    let part_1_answer = part_1(inputs.clone());
    let part_2_answer = part_2(inputs.clone());

    println!("Answer to part 1: {:?}", part_1_answer);
    println!("Answer to part 2: {:?}", part_2_answer);

}

fn part_1(measurements: Vec<u64>) -> usize {
	return measurements.windows(2).filter(|x| x[1] > x[0]).count();
}

fn part_2(measurements: Vec<u64>) -> usize {
	// Comparing the sum of triples: [A B C] [B C D]. 
	// We can inspect just the first element of the first triple and last element of second.
	return measurements.windows(4).filter(|x| (x[3]) > x[0]).count()
}

fn read_numbers(file_path:String) -> Vec<u64> {
    let mut inputs: Vec<u64> = Vec::new();
    if let Ok(lines) = read_lines(file_path) {
        for line in lines {
        	if let Ok(value) = line {
        		inputs.push(value.parse().unwrap())
        	}    		
        }
    }
    return inputs;
}