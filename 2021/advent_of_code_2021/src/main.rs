mod tools;
mod q1;
mod q2;
mod q3;
mod q4;
mod q5;
mod q6;
mod q7;
mod q8;
mod q9;
mod q10;

use std::env;

fn main() {
    let day = env::args().nth(1).unwrap_or_else(|| "10".into());
    println!("AdventOfCode2021 — day {day}");

    match day.as_str() {
        "1"  => q1::solution(),
        "2"  => q2::solution(),
        "3"  => q3::solution(),
        "4"  => q4::solution(),
        "5"  => q5::solution(),
        "6"  => q6::solution(),
        "7"  => q7::solution(),
        "8"  => q8::solution(),
        "9"  => q9::solution(),
        "10" => q10::solution(),
        _ => eprintln!("Unknown day: {day}"),
    }
}