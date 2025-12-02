module AdventOfCode2025.Q02

open System
open System.IO

let inputPath = "inputs/q02.txt"

let parse (lines: string list) =
    lines
    // your parsing here
    ()

let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let data = parse lines
    // compute result
    0  // replace with real result

let part2 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let data = parse lines
    // compute result
    0

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q1, part 1: %A"
    | 2 -> part2 () |> printfn "Q1, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
