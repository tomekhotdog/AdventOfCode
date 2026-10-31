module AdventOfCode2025.Q05

open System
open System.IO

let inputPath = "inputs/q05.txt"

type Range = { Lower : int64; Upper : int64 }

let size (r : Range) : int64 = r.Upper - r.Lower + 1L

let isInRange (x : int64) (r : Range) : bool = x >= r.Lower && x <= r.Upper

let parse (text : string) : Range list * int64 list =
    let sections = text.Replace("\r", "").Trim().Split("\n\n")
    let ranges =
        sections[0].Split("\n")
        |> Array.map (fun line ->
            let parts = line.Split("-")
            { Lower = int64 parts[0]; Upper = int64 parts[1] })
        |> Array.toList
    let ingredients =
        sections[1].Split("\n")
        |> Array.map int64
        |> Array.toList
    ranges, ingredients

let removeOverlapping (ranges : Range list) : Range list =
    match ranges |> List.sortBy (fun r -> r.Lower, r.Upper) with
    | [] -> []
    | first :: rest ->
        rest
        |> List.fold (fun (transformed : Range list) (right : Range) ->
            // transformed is built in reverse, so its head is the last range kept
            let left = List.head transformed
            if right.Lower > left.Upper then
                right :: transformed
            elif right.Upper <= left.Upper then
                transformed
            else
                { Lower = left.Upper + 1L; Upper = right.Upper } :: transformed
        ) [ first ]
        |> List.rev

let part1 () =
    let ranges, ingredients = parse (File.ReadAllText(inputPath))
    ingredients
    |> List.filter (fun x -> ranges |> List.exists (isInRange x))
    |> List.length

let part2 () =
    let ranges, _ = parse (File.ReadAllText(inputPath))
    removeOverlapping ranges |> List.sumBy size

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q5, part 1: %A"
    | 2 -> part2 () |> printfn "Q5, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
