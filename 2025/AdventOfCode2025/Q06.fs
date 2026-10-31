module AdventOfCode2025.Q06

open System
open System.IO

let inputPath = "inputs/q06.txt"

type Problem = { Numbers : string list; Operation : char }

let convertRightToLeft (numbers : string list) : uint64 list =
    // Each character column (read top to bottom) is one number; spaces are padding
    let columns = numbers[0].Length
    [ for j in columns - 1 .. -1 .. 0 do
        let digits = numbers |> List.map (fun number -> number[j]) |> List.filter ((<>) ' ')
        if not digits.IsEmpty then
            yield uint64 (String(Array.ofList digits)) ]

let solve (rightToLeft : bool) (problem : Problem) : uint64 =
    let numbers =
        match rightToLeft with
        | true -> convertRightToLeft problem.Numbers
        | false -> problem.Numbers |> List.map (fun x -> uint64 (x.Trim()))
    match problem.Operation with
    | '+' -> numbers |> List.reduce (+)
    | '*' -> numbers |> List.reduce (*)
    | op -> failwithf $"Unexpected operator: {op}"

let parse (lines : string list) : Problem list =
    let width = lines |> List.map _.Length |> List.max
    let lines = lines |> List.map (fun line -> line.PadRight(width))
    let operators = List.last lines
    let numberLines = lines |> List.take (lines.Length - 1)

    // Each operator sits in the first column of its problem, so it marks where the problem starts
    let starts = [ for i in 0 .. width - 1 do if operators[i] <> ' ' then yield i ]
    let ends = (starts |> List.tail |> List.map (fun start -> start - 1)) @ [ width ]

    List.zip starts ends
    |> List.map (fun (start, finish) ->
        { Numbers = numberLines |> List.map (fun line -> line.Substring(start, finish - start))
          Operation = operators[start] })

let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let problems = parse lines
    problems |> List.sumBy (solve false)

let part2 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let problems = parse lines
    problems |> List.sumBy (solve true)

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q6, part 1: %A"
    | 2 -> part2 () |> printfn "Q6, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
