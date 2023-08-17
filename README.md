# FSharp Example

This is some code from converting an interview coding test from C# into F#. It makes use of the awesome FsToolkit.ErrorHandling package (https://github.com/demystifyfp/FsToolkit.ErrorHandling).

This codebase contains a number of F# features including:

- Computation Expressions
- Higher-Order Functions
- Validation
- Discriminated Unions
- Single-case Discriminated Unions
- Result and Option
- Records and Tuples
- Modules

## Running the code

In the terminal, navigate to the project in the src folder and type:

```dotnet run```

In the terminal, you will get an output of ```Ok ()```

If you change some of the data to be invalid, say the FirstName to " " and the Email to "" and run again you will get an output of ```Error (FailedCreateValidation [EmptyFirstName " "; InvalidEmail ""])```
