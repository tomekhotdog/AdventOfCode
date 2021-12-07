#[allow(dead_code)]

pub fn solution() {

    let input = get_input();

    println!("Q4 part 1: {:?}", part_1(input.clone()));
    println!("Q4 part 2: {:?}", part_2(input.clone()));

}

fn get_input() -> Vec<String> {
        let input = include_str!("../inputs/q4.txt");
        input
            .trim()
            .split('\n')
            .map(|l| String::from(l))
            .collect::<Vec<String>>()
}

#[derive(Debug)]
struct BingoBoard {
	grid: Vec<Vec<u32>>,
	marked_off: Vec<Vec<bool>>,
	previous_crossed_off: Option<u32>,
}

struct BingoGame {
	boards: Vec<BingoBoard>,
	numbers: Vec<u32>
}

trait BingoBoardBehaviours {
	fn cross_off(&mut self, number: u32);
	fn completed(&self) -> bool;
	fn score(&self, last_number_called: u32) -> u32;
}

trait BingoGameBehaviours {
	fn incomplete_boards(&self) -> u32;
}

impl BingoBoardBehaviours for BingoBoard {
	fn cross_off(&mut self, selected: u32) {
		for (i, grid_row) in self.grid.iter().enumerate() {
			for (j, number) in grid_row.into_iter().enumerate() {
				if *number == selected {
					self.marked_off[i][j] = true;
				}
			}
		}
		self.previous_crossed_off = Some(selected);
	}

	fn completed(&self) -> bool {
		let size = self.grid.len();
		let complete_rows = self.marked_off.iter().any(|x| x.iter().all(|y| *y));
		let complete_columns = (0..size).into_iter().any(|i| self.marked_off.iter().all(|x| x[i]));
		return complete_rows || complete_columns;
	}

	fn score(&self, last_number_called: u32) -> u32 {
		let mut sum_of_unmarked = 0;
		for (i, line) in self.marked_off.iter().enumerate() {
			for (j, elem) in line.iter().enumerate() {
				if !elem {
					sum_of_unmarked += self.grid[i][j];
				}
			}
		}
		return sum_of_unmarked * last_number_called;
	}
}

impl BingoGameBehaviours for BingoGame {
	fn incomplete_boards(&self) -> u32 {
	    return self.boards.iter().filter(|x| !x.completed()).count() as u32;
	}
}

fn create_bingo_board(input: Vec<String>) -> BingoBoard {
	let size = input.len();
	let mut grid = vec![vec![0; size]; size];

	for (i, board_line) in input.into_iter().enumerate() {
		for (j, elem) in board_line.split_whitespace().enumerate() {
			let parsed = str::parse::<u32>(elem).unwrap();
			grid[i][j] = parsed;
		}
	}

	let marked_off = vec![vec![false; size]; size];
	return BingoBoard { grid: grid, marked_off: marked_off, previous_crossed_off: None };
}

fn parse_bingo_game(input: Vec<String>) -> BingoGame {
	let mut bingo_boards: Vec<BingoBoard> = Vec::new();
	let mut lines: Vec<String> = Vec::new();
	let mut bingo_numbers: Vec<u32> = Vec::new();

	for (i, line) in input.into_iter().enumerate() {
		if i == 0 {
			bingo_numbers = line.split(',').map(|x| str::parse::<u32>(x).unwrap()).collect();
		} else if line == "" && lines.len() > 0 {
			bingo_boards.push(create_bingo_board(lines.clone()));
			lines.clear();
		} else if line != "" {
			lines.push(line);
		}
	}
	// Parse final board.
	if lines.len() > 0 { bingo_boards.push(create_bingo_board(lines.clone())); }

	return BingoGame { boards: bingo_boards, numbers: bingo_numbers };

}

fn part_1(input: Vec<String>) -> u32 {

	let mut game = parse_bingo_game(input.clone());
	for next_called in game.numbers.into_iter() {
		for board in &mut game.boards.iter_mut() {
			board.cross_off(next_called);
			if board.completed() { 
				let score = board.score(next_called);
				return score;
			}
		}
	}
	return 0;
}

fn part_2(input: Vec<String>) -> u32 {
	let mut game = parse_bingo_game(input.clone());
	for next_called in game.numbers.clone().into_iter() {
		let one_incomplete_board_remaining = game.incomplete_boards() == 1;
		for board in &mut game.boards.iter_mut() {
			let board_was_incomplete = !board.completed();
			board.cross_off(next_called);
			if one_incomplete_board_remaining && board_was_incomplete && board.completed() { 
				let score = board.score(next_called);
				return score;
			}
		}
	}
	return 0;
}
