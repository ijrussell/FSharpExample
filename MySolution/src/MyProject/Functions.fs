namespace App

open System

[<RequireQualifiedAccess>]
module DateOnly =
    let getAge (now:DateOnly) (dateOfBirth:DateOnly) =
        let age = now.Year - dateOfBirth.Year
        if now.Month < dateOfBirth.Month || now.Month = dateOfBirth.Month && now.Day < dateOfBirth.Day then age - 1
        else age
