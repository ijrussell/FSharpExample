open System
open App

let nowProvider = fun () -> DateTime.Now

let companyRepository id =
    Some { Id = id; Name = $"Some Imaginary Company"; Classification = Gold }

let customerRepository (input:Customer * CustomerCredit) = ()

let creditService customer = 1000

let services = {
    NowProvider = nowProvider 
    GetCompanyById = companyRepository
    CreateCustomer = customerRepository
    CreditService = creditService
}

let firstName = "Geraldine"
let surname = "Grainger-Tuppence"
let email = "geraldine@someimaginarycompany.co.uk" 
let dateOfBirth = DateTime(1975, 1, 13) |> DateOnly.FromDateTime
let companyId = 42

CustomerService.addCustomer services firstName surname email dateOfBirth companyId
|> printfn "%A"
