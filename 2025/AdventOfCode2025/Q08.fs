module AdventOfCode2025.Q08

open System.IO

let inputPath = "inputs/q08.txt"

type Box = { X : uint64; Y : uint64; Z : uint64 }
type CircuitId = Id of int
type Part =
    | Part1
    | Part2

let parse (lines: string list) : Box seq =
    lines
    |> Seq.map (
        fun line ->
            let elems = line.Split(',')
            { X = uint64 elems[0]; Y = uint64 elems[1]; Z = uint64 elems[2] }
        )

let boxDistances (boxes : Box List) : Map<float, Box * Box> =
    let mutable distances = Map<float, Box * Box>([])
    for boxA in boxes do
        for boxB in boxes do
            if boxA <> boxB then
                let xDelta = (float(boxA.X) - float(boxB.X)) ** 2.0
                let yDelta = (float(boxA.Y) - float(boxB.Y)) ** 2.0
                let zDelta = (float(boxA.Z) - float(boxB.Z)) ** 2.0
                let distance = sqrt (xDelta + yDelta + zDelta)
                distances <- distances.Add((distance, (boxA, boxB)))                
    distances
                        
let makeCircuits (boxes : Box List) (distances : Map<float, Box * Box>) (nConnections : int32) (part : Part) : uint64 =
    let mutable boxToCircuit =
        seq { 0 .. ((boxes |> Seq.length) - 1) }
        |> Seq.map (fun i -> boxes.[i], Id(i))
        |> Map.ofSeq
        
    let mutable circuitToBoxes = 
        seq { 0 .. ((boxes |> Seq.length) - 1) }
            |> Seq.map (fun i ->
                    Id(i), [boxes.[i]]    
                )
            |> Map.ofSeq
            
    let connectBoxes boxA boxB : unit =
        let boxACircuit = boxToCircuit.[boxA]
        let boxBCircuit = boxToCircuit.[boxB]
        let boxesInBoxACircuit = circuitToBoxes.[boxACircuit]
        let boxesInBoxBCircuit = circuitToBoxes.[boxBCircuit]
        let combinedBoxes = boxesInBoxACircuit @ boxesInBoxBCircuit
        if boxACircuit <> boxBCircuit then
            // Transfer boxes in boxB circuit to boxA circuit.
            for b in boxesInBoxBCircuit do
                boxToCircuit <- boxToCircuit |> Map.add b boxACircuit        
            circuitToBoxes <- circuitToBoxes |> Map.add boxACircuit combinedBoxes
            circuitToBoxes <- circuitToBoxes |> Map.remove boxBCircuit
    
    // Iteratively connect boxes and combine their existing circuits. 
    let sorted = distances |> Map.toSeq |> Seq.sortBy fst
    let mutable connected = 0
    let mutable lastXCoordinatesProduct : uint64 = 0UL
    sorted
    |> Seq.toList
    |> List.takeWhile (
        fun (_, (boxA, boxB)) ->
            connectBoxes boxA boxB
            connected <- connected + 1
            lastXCoordinatesProduct <- boxA.X * boxB.X            
            // Quit early depending on the part of the question.
            match part with
            | Part1 -> connected < nConnections
            | Part2 -> circuitToBoxes |> Map.keys |> Seq.length <> 1            
        )
    |> ignore
    
    match part with
    | Part1 ->           
        circuitToBoxes
        |> Map.toSeq
        |> Seq.sortWith (fun (circuitA, circuitABoxes) (circuitB, circuitBBoxes) ->
            compare circuitBBoxes.Length circuitABoxes.Length)
        |> Seq.take 3
        |> Seq.map (fun (_, boxes) -> uint64 boxes.Length)
        |> Seq.fold (*) 1UL
    | Part2 ->
        lastXCoordinatesProduct
    
let part1 () =
    let boxes = File.ReadLines(inputPath) |> Seq.toList |> parse |> Seq.toList
    let distances = boxDistances boxes
    makeCircuits boxes distances 1000 Part.Part1

let part2 () =
    let boxes = File.ReadLines(inputPath) |> Seq.toList |> parse |> Seq.toList
    let distances = boxDistances boxes
    makeCircuits boxes distances 1000 Part.Part2

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q8, part 1: %A"
    | 2 -> part2 () |> printfn "Q8, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
