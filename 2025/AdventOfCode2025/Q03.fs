module AdventOfCode2025.Q03

open System
open System.IO

let inputPath = "inputs/q03.txt"

type Bank = int list

let parse (lines: string list) : Bank list =
    lines
    |> List.map (fun line -> line |> Seq.map (fun c -> int c - int '0') |> Seq.toList)

let bankJoltage (bank : Bank) (nBatteries : int) : uint64 =
    let batteries = bank.Length
    let joltage, _ =
        [ 0 .. nBatteries - 1 ]
        |> List.fold (fun (joltage : uint64, startIndex : int) (i : int) ->
            // Leave enough batteries after this pick to fill the remaining slots
            let endIndex = batteries - (nBatteries - i - 1)
            let eligible = bank[startIndex .. endIndex - 1]
            let batteryDigit = List.max eligible
            let nextStartIndex = startIndex + (List.findIndex ((=) batteryDigit) eligible) + 1
            joltage * 10UL + uint64 batteryDigit, nextStartIndex
        ) (0UL, 0)
    joltage

let totalJoltage (banks : Bank list) (nBatteries : int) : uint64 =
    banks |> List.sumBy (fun bank -> bankJoltage bank nBatteries)

let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let banks = parse lines
    totalJoltage banks 2

let part2 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let banks = parse lines
    totalJoltage banks 12

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q3, part 1: %A"
    | 2 -> part2 () |> printfn "Q3, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
