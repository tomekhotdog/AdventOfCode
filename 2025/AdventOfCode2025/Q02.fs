module AdventOfCode2025.Q02

open System.IO

let inputPath = "inputs/q02.txt"

type Range = { Low: uint64; High: uint64 }
type Repeated =
    | Twice
    | NTimes

let parse (lines: string list) : Range seq =
    lines[0].Split(',')
    |> Array.map (fun range ->
        let elems = range.Split('-')
        { Low = uint64 elems[0]; High = uint64 elems[1] }
        )
    |> Array.toSeq

let selectInvalid (range : Range) (repeated: Repeated) : uint64 seq =
    seq { range.Low .. range.High }
    |> Seq.choose
        (fun x ->
            let candidate = x.ToString().ToCharArray()
            let l = candidate.Length
            let potentialPatternLengths =
                match repeated with
                | Repeated.Twice -> seq { l / 2 }
                | Repeated.NTimes -> seq { 1 .. (l - 1) }                
            let patternLengthFilter =
                match repeated with
                | Repeated.Twice -> (fun n -> l = 2 * n)
                | Repeated.NTimes -> (fun n -> l % n = 0)                
            
            let filteredPatternLengths = potentialPatternLengths |> Seq.filter patternLengthFilter                
            
            let foundPattern =
                filteredPatternLengths
                |> Seq.exists
                    (fun patternLength ->
                        let repeatedElems =
                            seq { 1 .. (l / patternLength) }
                            |> Seq.map (fun n -> candidate.[((n - 1) * patternLength) .. ((n * patternLength) - 1)])
                        let firstElem = candidate.[0 .. patternLength - 1]
                        let elemsEqual = repeatedElems |> Seq.forall (fun elem -> elem = firstElem)
                        elemsEqual
                    )
                    
            match foundPattern with
            | true -> Some x
            | false -> None
        )

let part1 () =
    File.ReadLines(inputPath)
    |> Seq.toList
    |> parse
    |> Seq.collect (fun range -> selectInvalid range Repeated.Twice)
    |> Seq.sum

let part2 () =
    File.ReadLines(inputPath)
    |> Seq.toList
    |> parse
    |> Seq.collect (fun range -> selectInvalid range Repeated.NTimes)
    |> Seq.sum

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q1, part 1: %A"
    | 2 -> part2 () |> printfn "Q1, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
