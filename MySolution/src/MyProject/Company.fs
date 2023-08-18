namespace App

type Classification = Gold | Silver | Bronze

type Company = {
    Id: int
    Name: string
    Classification: Classification
}

