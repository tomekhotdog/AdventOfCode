module AdventOfCode2025.Program

open System

[<EntryPoint>]
let main argv =
    // default: latest implemented day, both parts
    let dayOpt =
        argv
        |> Array.tryFindIndex ((=) "--day")
        |> Option.bind (fun i ->
            if i + 1 < argv.Length then
                match Int32.TryParse argv.[i+1] with
                | true, d -> Some d
                | _ -> None
            else None)

    let partOpt =
        argv
        |> Array.tryFindIndex ((=) "--part")
        |> Option.bind (fun i ->
            if i + 1 < argv.Length then
                match Int32.TryParse argv.[i+1] with
                | true, p -> Some p
                | _ -> None
            else None)

    let runDay day part =
        match day with
        | 1 -> Q01.run part
        | 2 -> Q02.run part
        | 3 -> Q03.run part
        | 4 -> Q04.run part
        | 7 -> Q07.run part
        | 8 -> Q08.run part
        // ...
        | _ -> failwithf "Day %d not implemented yet" day

    match dayOpt, partOpt with
    | Some d, Some p ->
        runDay d p
    | Some d, None ->
        runDay d 1
        runDay d 2
    | None, _ ->
        printfn "Usage: dotnet run -- --day <n> [--part <1|2>]"
        printfn "Example: dotnet run -- --day 1 --part 2"

    0
