module Tests

open System
open App
open Xunit

module AddCustomerTests =

    [<Literal>]
    let validFirstName = "Geraldine"
    [<Literal>]
    let validSurname = "Grainger-Tuppence"
    [<Literal>]
    let validEmail = "geraldine@someimaginarycompany.co.uk"
    let validDateOfBirth = DateTime(1975, 1, 13) |> DateOnly.FromDateTime

    let nowProvider = fun () -> DateTime.Now

    let companyRepository id =
        Some { Id = id; Name = $"Some Imaginary Company"; Classification = Gold }

    let customerRepository (input:Customer * Company * CompanyCredit) = ()

    let creditService (company:Company) = 1000

    let services = {
        NowProvider = nowProvider 
        GetCompanyById = companyRepository
        CreateCustomer = customerRepository
        CreditService = creditService
    }

    let rand =
        let rnd = Random()
        fun () -> rnd.Next(100)

    module ``Invalid Customer inputs`` =
        
        // TODO: full set of tests for invalid inputs

        [<Fact>]
        let ``Invalid inputs returns expected errors`` () =
            let firstName = " "
            let surname = "Grainger-Tuppence"
            let email = "" 
            let dateOfBirth = DateTime(1975, 1, 13) |> DateOnly.FromDateTime
            let companyId = 0
            
            let actual =
                CustomerService.addCustomer services firstName surname email dateOfBirth companyId
    
            let expected = Error (FailedCreateValidation [EmptyFirstName " "; InvalidEmail ""])

            Assert.Equal(expected, actual)


    module ``Gold Company classification`` =

        [<Fact>]
        let ``With valid inputs returns Ok`` () =
            let companyId = rand()
            
            let actual =
                CustomerService.addCustomer services validFirstName validSurname validEmail validDateOfBirth companyId
    
            let expected = Ok ()

            Assert.Equal(expected, actual)
    

    module ``Non-Gold Company classification`` =

        [<Fact>]
        let ``Silver company with sufficient credit and valid inputs returns Ok`` () =
            let companyId = rand()
            
            let silverCompanyRepository (id:int) = 
                Some { Id = companyId; Name = $"Some Imaginary Company"; Classification = Silver }
            let services' = { services with GetCompanyById = silverCompanyRepository }

            let actual =
                CustomerService.addCustomer services' validFirstName validSurname validEmail validDateOfBirth companyId
    
            let expected = Ok ()

            Assert.Equal(expected, actual)

        [<Fact>]
        let ``Silver company with insufficient credit and valid inputs returns Ok`` () =
            let companyId = rand()
            
            let silverCompanyRepository (id:int) = 
                Some { Id = companyId; Name = $"Some Imaginary Company"; Classification = Silver }
            let noCreditService (company:Company) = 0
            let services' = { services with GetCompanyById = silverCompanyRepository; CreditService = noCreditService }

            let actual =
                CustomerService.addCustomer services' validFirstName validSurname validEmail validDateOfBirth companyId
    
            let expected = Ok ()

            Assert.Equal(expected, actual)

    module ``Company not found`` =

        [<Fact>]
        let ``returns expected error`` () =
            let companyId = 42
            
            let emptyCompanyRepository (id:int) = None
            let services' = { services with GetCompanyById = emptyCompanyRepository }
            
            let actual =
                CustomerService.addCustomer services' validFirstName validSurname validEmail validDateOfBirth companyId
    
            let expected = Error (CompanyNotFound companyId)

            Assert.Equal(expected, actual)

