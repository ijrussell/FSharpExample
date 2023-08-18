namespace App

type CompanyCredit =
    | HasCreditLimit of int
    | DoesNotHaveCreditLimit

[<RequireQualifiedAccess>]
module CompanyCredit =
    
    let private requiresCreditCheck (company:Company) =
        match company.Classification with
        | Gold -> false
        | _ -> true

    let calculateCreditLimit (company:Company) (limit:int) =
        match company.Classification with
        | Gold -> DoesNotHaveCreditLimit
        | Silver -> 2 * limit |> HasCreditLimit
        | Bronze -> limit |> HasCreditLimit

    let getCreditLimit (creditCheckService:Company -> int) (company:Company) =
        if company |> requiresCreditCheck then
            company
            |> creditCheckService
            |> fun limit -> calculateCreditLimit company limit 
        else DoesNotHaveCreditLimit
