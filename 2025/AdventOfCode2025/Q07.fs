module AdventOfCode2025.Q07

open System.IO

let inputPath = "inputs/q07.txt"

type Position = { X : int32; Y : int32 }
type Manifold = {
    Beams : Position Set
    Splitters : Position Set
    ActivatedSplitters : Position Set
    Width : int32
    Height : int32
}

let parse (lines: string list) : Position * Manifold =
    let mutable beams = []
    let mutable splitters = []
    lines
    |> List.iteri
        (fun y row ->
            row.ToString().ToCharArray()
            |> Array.iteri
                (fun x elem ->
                    match elem with
                    | '^' -> splitters <- { X = x; Y = y } :: splitters
                    | 'S' -> beams <- { X = x; Y = y } :: beams  
                    | _ -> ()                     
                )            
        )
    beams.[0],
    { Beams = Set beams
      Splitters = Set splitters
      ActivatedSplitters = Set.empty
      Width = lines[0].Length
      Height = lines.Length }

let printManifold (manifold : Manifold) : unit =
    for y in 0 .. manifold.Height-1 do
        for x in 0 .. manifold.Width-1 do
            let pos = { X = x; Y = y }
            if manifold.Beams.Contains(pos) then
                printf "|"
            if manifold.Splitters.Contains(pos) then
                printf "^"
            else
                printf "."
        printfn ""
    ()
    
let next (m : Manifold) : Manifold =
    let mutable activated = m.ActivatedSplitters |> Seq.toList
    let newBeams =
        m.Beams
        |> Set.toSeq
        |> Seq.collect (fun beam ->
            let next = { X = beam.X; Y = beam.Y + 1 }
            let nextBeams =
                if (next.Y > m.Height) then
                    []
                else
                    if m.Splitters.Contains next then
                        activated <- next :: activated
                        let left = { X = beam.X - 1; Y = beam.Y + 1}
                        let right = { X = beam.X + 1; Y = beam.Y + 1 }
                        [left ; right]
                    else
                        [next]
            nextBeams)
        |> Seq.toList
        
    let newActivated = set activated
    { Beams = set newBeams; Splitters = m.Splitters; ActivatedSplitters = newActivated; Height = m.Height; Width = m.Width }
    
let countBeamSplits (manifold : Manifold) : int32 =
    let mutable m = manifold
    while not m.Beams.IsEmpty do
        m <- next m
        
    m.ActivatedSplitters |> Set.toSeq |> Seq.length   

let part1 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let _, manifold = parse lines
    countBeamSplits manifold
    
let countPaths (start : Position) (manifold : Manifold) : uint64 =    
    let rec countPathsInner (pos : Position) (cache : Map<Position, uint64>) : uint64 * Map<Position, uint64> =
        match Map.tryFind pos cache with
        | Some value -> 
            value, cache
        | None ->
            let result, cache' =
                if pos.Y > manifold.Height then
                    1UL, cache
                else
                    let next = { pos with Y = pos.Y + 1 }

                    if not (manifold.Splitters.Contains next) then
                        countPathsInner next cache
                    else
                        let posLeft  = { pos with X = pos.X - 1 }
                        let posRight = { pos with X = pos.X + 1 }
                        let countLeft, cacheLeft = countPathsInner posLeft cache
                        let countRight, cacheRight = countPathsInner posRight cacheLeft
                        countLeft + countRight, cacheRight

            let cacheNext = cache'.Add(pos, result)
            result, cacheNext
            
    countPathsInner start Map.empty |> fst              
        
let part2 () =
    let lines = File.ReadLines(inputPath) |> Seq.toList
    let start, manifold = parse lines
    countPaths start manifold

let run part =
    match part with
    | 1 -> part1 () |> printfn "Q7, part 1: %A"
    | 2 -> part2 () |> printfn "Q7, part 2: %A"
    | _ -> failwith "Part must be 1 or 2"
