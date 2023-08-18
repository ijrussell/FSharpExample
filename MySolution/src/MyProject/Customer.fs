namespace App

open System
open FsToolkit.ErrorHandling

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
    let private getAge (now:DateOnly) (dateOfBirth:DateOnly) =
        let age = now.Year - dateOfBirth.Year
        if now.Month < dateOfBirth.Month || now.Month = dateOfBirth.Month && now.Day < dateOfBirth.Day then age - 1
        else age
    
    let create (nowProvider:unit -> DateTime) (dateOfBirth:DateOnly) =
        let today = nowProvider () |> DateOnly.FromDateTime
        if getAge today dateOfBirth >= 21 then Ok (DateOfBirth dateOfBirth)
        else Error (CustomerTooYoung dateOfBirth)

    let value (input:DateOfBirth) = input |> fun (DateOfBirth dateOfBirth) -> dateOfBirth

type Customer = {
    FirstName: FirstName
    Surname: Surname
    Email: Email
    DateOfBirth: DateOfBirth
}

type AddCustomerServices = {
    NowProvider: unit -> DateTime
    GetCompanyById: int -> Company option
    CreateCustomer: Customer * Company * CompanyCredit -> unit
    CreditService: Company -> int
}

type AddCustomerErrors =
    | FailedCreateValidation of CustomerValidationErrors list
    | CompanyNotFound of int
    | CompanyNotSupplied

[<RequireQualifiedAccess>]
module Customer =
    let private create firstName surname email dateOfBirth =
        { 
            FirstName = firstName
            Surname = surname
            Email = email
            DateOfBirth = dateOfBirth
       }

    let tryCreate (nowProvider:unit -> DateTime) firstName surname email dateOfBirth =
        validation {
            let! firstName' = firstName |> FirstName.create
            and! surname' = surname |> Surname.create
            and! email' = email |> Email.create
            and! dateOfBirth' = dateOfBirth |> DateOfBirth.create nowProvider
            return create firstName' surname' email' dateOfBirth'
        }     

[<RequireQualifiedAccess>]
module CustomerService =
    let addCustomer (services:AddCustomerServices) firstName surname email dateOfBirth companyId =
        result {
            let! customer = 
                Customer.tryCreate services.NowProvider firstName surname email dateOfBirth 
                |> Result.mapError FailedCreateValidation
            let! company = 
                services.GetCompanyById companyId 
                |> Result.requireSome (CompanyNotFound companyId)
            let credit = CompanyCredit.getCredit services.CreditService company 
            let creditLimit = 
                credit
                |> CompanyCredit.bind (CompanyCredit.calculateCreditLimit company)
            return services.CreateCustomer (customer, company, creditLimit)
        }



