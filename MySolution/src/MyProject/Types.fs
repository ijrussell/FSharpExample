namespace App

open System

type CustomerValidationErrors =
    | EmptyFirstName of string
    | EmptySurname of string
    | InvalidEmail of string
    | CustomerTooYoung of DateOnly

type FirstName = private FirstName of string

[<RequireQualifiedAccess>]
module FirstName =
    let create (input:string) =
        if String.IsNullOrWhiteSpace(input) |> not then Ok (FirstName input)
        else Error (EmptyFirstName input)

    let value (input:FirstName) = input |> fun (FirstName firstName) -> firstName

type Surname = private Surname of string

[<RequireQualifiedAccess>]
module Surname =
    let create (input:string) =
        if String.IsNullOrWhiteSpace(input) |> not then Ok (Surname input)
        else Error (EmptySurname input)

    let value (input:Surname) = input |> fun (Surname surname) -> surname

type Email = private Email of string

[<RequireQualifiedAccess>]
module Email =
    let create (input:string) =
        if input.Contains("@") && input.Contains(".") then Ok (Email input)
        else Error (InvalidEmail input)

    let value (input:Email) = input |> fun (Email email) -> email

type DateOfBirth = private DateOfBirth of DateOnly

[<RequireQualifiedAccess>]
module DateOfBirth =
    let create (nowProvider:unit -> DateTime) (dateOfBirth:DateOnly) =
        let today = nowProvider () |> DateOnly.FromDateTime
        if DateOnly.getAge today dateOfBirth >= 21 then Ok (DateOfBirth dateOfBirth)
        else Error (CustomerTooYoung dateOfBirth)

    let value (input:DateOfBirth) = input |> fun (DateOfBirth dateOfBirth) -> dateOfBirth

