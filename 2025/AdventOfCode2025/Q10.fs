module AdventOfCode2025.Q10

open System
open System.IO

let inputPath = "inputs/q10_example.txt"

type Machine = { IndicatorLights : uint64; Buttons : uint64 List; Joltages : uint64 List }

let parse (lines: string list) : Machine List =
    lines
    |> List.map(
        fun line ->
            let indicatorBits = line.Split(']').[0].Split('[').[1]
            let indicatorInt =
                indicatorBits.ToCharArray()
                |> Array.mapi(
                    fun i x ->
                        match x with
                        | '#' -> 2.0 ** float (indicatorBits.Length - 1 - i)
                        | '.' -> 0
                        | _ -> failwithf $"Unexpected character! {x}")
                |> Array.sum |> round |> uint64
            let buttons =
                line.Split(']').[1].Split('{').[0].Trim().Split(' ')
                |> Array.map(
                    // of form (2,4)
                    fun elem ->                        
                        elem.Split('(').[1].Split(')').[0].Split(',')
                        |> Array.map (fun x -> 2.0 ** float x)
                        |> Array.sum |> round |> uint64)
                |> Array.toList
            let joltageBits =
                line.Split('{').[1].Split('}').[0].Split(',')
                |> Array.map uint64
                |> Array.toList
            
            { IndicatorLights = uint64 indicatorInt; Buttons = buttons; Joltages = joltageBits}                               
        )    
let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let machines = parse lines
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
