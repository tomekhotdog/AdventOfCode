module AdventOfCode2025.Common

open System.IO

let readLines path = File.ReadLines(path) |> Seq.toList

let gridOfLines (lines: string list) =
    lines
    |> List.mapi (fun y line ->
        line |> Seq.mapi (fun x c -> (x,y), c) |> Seq.toList)
    |> List.collect id
    |> Map.ofList
