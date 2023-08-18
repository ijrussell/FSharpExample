namespace App

open System
open System.Text.RegularExpressions

[<AutoOpen>]
module SharedFunctions =
    let (|ParseRegex|_|) regex str =
        let m = Regex(regex).Match(str)
        if m.Success then Some (List.tail [ for x in m.Groups -> x.Value ])
        else None

    let (|IsNullOrWhiteSpace|_|) input =
        if String.IsNullOrWhiteSpace input then Some ()
        else None

    let (|IsValidEmail|_|) input =
        match input with
        | ParseRegex ".*?@(.*)" [ _ ] -> Some input
        | _ -> None

