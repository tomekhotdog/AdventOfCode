// use std::fs::File;
// use std::io::{self, BufRead};
// use std::path::Path;

// // The output is wrapped in a Result to allow matching on errors
// // Returns an Iterator to the Reader of the lines of the file.
// pub fn read_lines<P>(file_path: P) -> io::Result<io::Lines<io::BufReader<File>>>
// where P: AsRef<Path>, {
//     let file = File::open(file_path)?;
//     Ok(io::BufReader::new(file).lines())
// }


// pub fn read_numbers(file_path:String) -> Vec<u64> {
// 	println!("Hello {:?}", file_path);
//     let mut inputs: Vec<u64> = Vec::new();
//     if let Ok(lines) = read_lines(file_path) {
//     	println!("good");
//         for line in lines {
//         	if let Ok(value) = line {
//         		inputs.push(value.parse().unwrap())
//         	}    		
//         }
//     } else {
//     	println!("bad");
//     }
//     return inputs;
// }