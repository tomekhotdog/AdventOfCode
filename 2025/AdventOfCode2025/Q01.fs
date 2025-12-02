module AdventOfCode2025.Q01

open System
open System.IO

let inputPath = "inputs/q01.txt"

type Direction =
    | L
    | R
    
type Rotation = { Direction : Direction; Distance : int }

let parse (lines: string list) : Rotation seq =
    lines
    |> Seq.map (fun line ->
        let d =
            match line[0] with
            | 'L' -> Direction.L
            | 'R' -> Direction.R
            | _ -> failwithf $"Unrecognised direction {line[0]}"
        { Direction = d; Distance = int (line.Substring(1, line.Length - 1)) } )

let countDialTransitionsToZero
    (rotations : Rotation seq)
    (initialPosition : int)
    (includeTransitionPastZero : bool) : int
    =
        let _, timesAtZero =
            rotations
            |> Seq.fold (fun (startPosition : int, count : int) (rotation : Rotation) ->
                let sanitised = rotation.Distance % 100
                let revolutions = rotation.Distance / 100
                let rawEndPosition =
                    match rotation.Direction with
                    | Direction.L -> startPosition - sanitised
                    | Direction.R -> startPosition + sanitised
                    
                // Did we cross zero "in flight"?
                let transitionsPastZero =
                    match startPosition <> 0 with
                    | true -> if (rawEndPosition < 0 || rawEndPosition > 100) then 1 else 0
                    | false -> 0
                
                let endPosition = (rawEndPosition + 100) % 100
                
                // Did we land exactly on zero?
                let transitionsToZero = if endPosition = 0 then 1 else 0                
                let transitions =
                    match includeTransitionPastZero with
                    | true -> transitionsPastZero + transitionsToZero + revolutions
                    | false -> transitionsToZero

                endPosition, count + transitions
            ) (initialPosition, 0)
        timesAtZero

let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let rotations = parse lines
    let startPosition = 50
    countDialTransitionsToZero rotations startPosition false

let part2 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let rotations = parse lines
    let startPosition = 50
    countDialTransitionsToZero rotations startPosition true

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q1, part 1: %A"
    | 2 -> part2 () |> printfn "Q1, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
