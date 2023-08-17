namespace App

open System
open FsToolkit.ErrorHandling

type Classification = Gold | Silver | Bronze

type Company = {
    Id: int
    Name: string
    Classification: Classification
}

type Customer = {
    FirstName: FirstName
    Surname: Surname
    Email: Email
    DateOfBirth: DateOfBirth
    Company: Company option
}

type CustomerCredit =
    | HasCreditLimit of int
    | DoesNotHaveCreditLimit

type AddCustomerServices = {
    NowProvider: unit -> DateTime
    GetCompanyById: int -> Company option
    CreateCustomer: Customer * CustomerCredit -> unit
    CreditService: Customer -> int
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
            Company = None 
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
module Company =
    let requiresCreditCheck (company:Company) =
        match company.Classification with
        | Gold -> false
        | _ -> true

    let calculateCreditLimit (company:Company) (limit:int) =
        match company.Classification with
        | Gold -> DoesNotHaveCreditLimit
        | Silver -> 2 * limit |> HasCreditLimit
        | Bronze -> limit |> HasCreditLimit

[<RequireQualifiedAccess>]
module CustomerCredit =
    let get (creditService:Customer -> int) (customer:Customer) =
        customer.Company
        |> Option.map (fun company ->
            if company |> Company.requiresCreditCheck then
                customer
                |> creditService 
                |> Company.calculateCreditLimit company
            else DoesNotHaveCreditLimit)

    let hasSufficientCredit (creditLimit:CustomerCredit) =
        match creditLimit with
        | HasCreditLimit limit -> limit >= 500
        | DoesNotHaveCreditLimit -> true

    let bind (f:int -> CustomerCredit) (limit:CustomerCredit) =
        match limit with
        | HasCreditLimit value -> f value 
        | DoesNotHaveCreditLimit -> DoesNotHaveCreditLimit

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
            let customerWithCompany = { customer with Company = Some company }
            let! credit = 
                CustomerCredit.get services.CreditService customerWithCompany 
                |> Result.requireSome CompanyNotSupplied
            let creditLimit = 
                credit
                |> CustomerCredit.bind (Company.calculateCreditLimit company)
            return services.CreateCustomer (customerWithCompany, creditLimit)
        }



