use std::collections::HashSet;

#[allow(dead_code)]

pub fn solution() {
    println!("Q9 part 1: {:?}", part_1());
    println!("Q9 part 2: {:?}", part_2());
}

fn parse_input() -> Vec<Vec<u32>> {
    let input = include_str!("../inputs/q9.txt");
    input
        .trim()
        .split('\n')
        .map(|l| l.chars().map(|x| x.to_digit(10).unwrap()).collect::<Vec<u32>>())
        .collect::<Vec<Vec<u32>>>()
}

#[allow(dead_code)]
fn print(grid: &Vec<Vec<u32>>) {
	for row in grid.into_iter() {
		for elem in row.into_iter() {
			print!("{:?}", elem);
		}
		println!();
	}
}

fn low_points(grid: &Vec<Vec<u32>>) -> Vec<(usize, usize)> {
	let mut low_points: Vec<(usize, usize)> = Vec::new();
	for (y, row) in grid.into_iter().enumerate() {
		for (x, current) in row.into_iter().enumerate() {
			let north = if y == 0 { None } else { Some(grid[y-1][x]) };
			let south = if y == grid.len() - 1 { None } else { Some(grid[y+1][x]) };
			let west  = if x == 0 { None } else { Some(grid[y][x-1]) };
			let east  = if x == row.len() - 1 { None } else { Some(grid[y][x+1]) };
			let low_point = vec![north, south, west, east].into_iter().all(|adjacent| adjacent.is_none() || adjacent.unwrap() > *current);
			if low_point { low_points.push((x, y)); }
		}
	}
	return low_points;
}

// Calculate the 'risk level' of the low points.
fn part_1() -> u32 {
	let height_map = parse_input();
	return low_points(&height_map).into_iter().map(|(x,y)| height_map[y][x] + 1).sum::<u32>() as u32;
}

// Recursively climb up the basin inspecting neighbouring tiles.
fn find_basin_neighbours(grid: &Vec<Vec<u32>>, x: usize, y: usize, basin_elems: &mut HashSet<(usize, usize)>) {
	if basin_elems.contains(&(x,y)) || grid[y][x] == 9 { return };
	basin_elems.insert((x,y));

	let north = if y == 0 { None } else { Some((y-1,x)) };
	let south = if y == grid.len() - 1 { None } else { Some((y+1,x)) };
	let west  = if x == 0 { None } else { Some((y,x-1)) };
	let east  = if x == grid[0].len() - 1 { None } else { Some((y,x+1)) };

	let coordinates = vec![north, south, west, east];
	for coordinate in coordinates.into_iter() {
		if coordinate.is_none() { continue; }
		let (y1,x1) = coordinate.unwrap();
		if grid[y][x] < grid[y1][x1] { find_basin_neighbours(&grid, x1, y1, basin_elems) }
	}
}

// Find the product of the sizes of the three largest basins.
fn part_2() -> u32 {
	let height_map = parse_input();
	let low_points = low_points(&height_map);
	assert!(low_points.len() > 3, "Need at least 3 low points!");

	let mut basin_sizes = Vec::new();
	for (x,y) in low_points.into_iter() {
		let mut basin: HashSet<(usize, usize)> = HashSet::new();
		find_basin_neighbours(&height_map, x, y, &mut basin);
		basin_sizes.push(basin.len());
	}

	basin_sizes.sort();
	basin_sizes.reverse();
	return (basin_sizes[0] * basin_sizes[1] * basin_sizes[2]) as u32;
}