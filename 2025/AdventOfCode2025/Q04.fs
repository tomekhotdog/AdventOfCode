module AdventOfCode2025.Q04

open System
open System.IO

let inputPath = "inputs/q04.txt"

type Grid = char[][]

let parse (lines: string list) : Grid =
    lines
    |> List.map (fun line -> line.ToCharArray())
    |> List.toArray

let adjacentPositions (grid : Grid) (width : int) (height : int) (x : int) (y : int) : char list =
    [ for xDelta in -1 .. 1 do
        for yDelta in -1 .. 1 do
            let newX = x + xDelta
            let newY = y + yDelta
            if not (newX = x && newY = y)
               && 0 <= newX && newX < width
               && 0 <= newY && newY < height then
                yield grid[newY][newX] ]

let removableRolls (grid : Grid) : (int * int) list =
    let height = grid.Length
    let width = grid[0].Length
    [ for y in 0 .. height - 1 do
        for x in 0 .. width - 1 do
            if grid[y][x] = '@' then
                let adjacentRolls =
                    adjacentPositions grid width height x y
                    |> List.filter ((=) '@')
                    |> List.length
                if adjacentRolls < 4 then
                    yield (x, y) ]

let removeRolls (grid : Grid) (removable : (int * int) list) : unit =
    for x, y in removable do
        grid[y][x] <- '.'

let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let grid = parse lines
    removableRolls grid |> List.length

let part2 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let grid = parse lines
    let mutable totalRemoved = 0
    let mutable removable = removableRolls grid
    while not removable.IsEmpty do
        removeRolls grid removable
        totalRemoved <- totalRemoved + removable.Length
        removable <- removableRolls grid
    totalRemoved

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q4, part 1: %A"
    | 2 -> part2 () |> printfn "Q4, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
