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

    let getCredit (creditService:Company -> int) (company:Company) =
        if company |> requiresCreditCheck then
            company
            |> creditService
            |> fun limit -> HasCreditLimit limit 
        else DoesNotHaveCreditLimit

    let hasSufficientCredit (creditLimit:CompanyCredit) =
        match creditLimit with
        | HasCreditLimit limit -> limit >= 500
        | DoesNotHaveCreditLimit -> true

    let bind (f:int -> CompanyCredit) (limit:CompanyCredit) =
        match limit with
        | HasCreditLimit value -> f value 
        | DoesNotHaveCreditLimit -> DoesNotHaveCreditLimit

