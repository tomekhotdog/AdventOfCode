#[allow(dead_code)]

use std::cmp;
use std::collections::HashMap;


pub fn solution() {

    let input = get_input();

    println!("Q5 part 1: {:?}", part_1(input.clone()));
    println!("Q5 part 2: {:?}", part_2(input.clone()));

}

fn get_input() -> Vec<String> {
        let input = include_str!("../inputs/q5.txt");
        input
            .trim()
            .split('\n')
            .map(|l| String::from(l))
            .collect::<Vec<String>>()
}

#[derive(Debug,Eq,PartialEq,Hash,Clone,Copy)]
struct Coordinates {
	x: i32,
	y: i32
}

#[derive(Debug)]
struct LineSegment {
	p1: Coordinates,
	p2: Coordinates
}

fn parse_coordinates(coordinates: &str) -> Coordinates {
	let coordinates = coordinates.trim().split(',').collect::<Vec<&str>>();
	let x = str::parse::<i32>(coordinates[0]).unwrap();
	let y = str::parse::<i32>(coordinates[1]).unwrap();
	return Coordinates { x: x, y: y }
}

fn parse(input: Vec<String>) -> Vec<LineSegment> {
	let mut segments = Vec::<LineSegment>::new();
	for line in input.iter() {
		let elems = line.split("->").collect::<Vec<&str>>();
		let segment =  LineSegment { p1: parse_coordinates(elems[0]), p2: parse_coordinates(elems[1]) };
		segments.push(segment)	
	}
	return segments;
}

fn coordinates_covered_by_segment(s: &LineSegment) -> Vec<Coordinates> {
	let length = cmp::max((s.p2.x - s.p1.x).abs(), (s.p2.y - s.p1.y).abs());
	let dx = (s.p2.x - s.p1.x).signum();
	let dy = (s.p2.y - s.p1.y).signum();
	return (0..length+1).into_iter().map(|n| Coordinates {x: s.p1.x + (n * dx ), y: s.p1.y + (n * dy)}).collect::<Vec<Coordinates>>();
}

fn horizontal_or_vertical(s: &LineSegment) -> bool {
	return s.p1.x == s.p2.x || s.p1.y == s.p2.y;
}

fn count_covered_points(points: Vec<Coordinates>) -> i32 {
	let mut points_map: HashMap<Coordinates, i32> = HashMap::new();
	for point in points {
    	points_map.entry(point).or_insert(0);
    	points_map.insert(point, 1 + points_map[&point]);
	}
	return points_map.into_values().filter(|x| *x >= 2).count() as i32;
}

fn part_1(input: Vec<String>) -> i32 {
	let segments: Vec<LineSegment> = parse(input).into_iter().filter(|s| horizontal_or_vertical(s)).collect();
	let points = segments.iter().flat_map(|s| coordinates_covered_by_segment(s)).collect::<Vec<Coordinates>>();
	return count_covered_points(points);
}

fn part_2(input: Vec<String>) -> i32 {
	let segments = parse(input);
	let points = segments.iter().flat_map(|s| coordinates_covered_by_segment(s)).collect::<Vec<Coordinates>>();
	return count_covered_points(points);
}