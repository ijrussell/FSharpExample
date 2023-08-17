# F# Example Codebase

This is some code from converting an interview coding test from C# into F#. It makes use of the awesome FsToolkit.ErrorHandling package (<https://github.com/demystifyfp/FsToolkit.ErrorHandling>).

This codebase contains a number of F# features and techniques including:

- Computation Expressions
- Higher-Order Functions
- Validation
- Discriminated Unions
- Single-case Discriminated Unions
- Result and Option
- Records and Tuples
- Modules

## Setting up

This code was written using VS Code, the ionide F# extension, and .Net SDK 7.0.10.

## Running the code

In the terminal, navigate to the project in the src folder and type:

```dotnet run```

In the terminal, you will get an output of ```Ok ()```

If you change some of the data to be invalid, say the FirstName to " " and the Email to "" and run again you will get an output of ```Error (FailedCreateValidation [EmptyFirstName " "; InvalidEmail ""])```

## Running the tests

Open a new terminal, navigate to the project in the tests folder and type:

```dotnet test```

In the terminal, you will get an output showing the result of running the tests.

## Free F# ebook

You can download my free 200-page ebook from <https://leanpub.com/essential-fsharp>
