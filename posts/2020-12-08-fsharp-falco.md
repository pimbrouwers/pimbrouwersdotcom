[« Go home](/)

# F# on the Web: A guide to building websites with Falco, .NET 5.x and ASP.NET Core

F# has a rich and mature development scene for both client- and server-side web programming. [Fable][2] revolutionized the durability with which you can craft client-side code, providing F#'s steadfast type system and an MVU programming model. On there server, [Giraffe][3], [Saturn][4] and [Falco][1] each offer their own unique way of providing similar durability on the server.

* * *

<small class="muted monospace">DECEMBER 8, 2020</small>

The focus of this post is going to be the [Falco Framework][1], where will aim to build a small, but complete-_ish_ bullet journal using .NET 5.x and ASP.NET Core. We'll also be bringing in a couple of other packages to help us get the job done:

- [System.Data.SQLite][5]
- [Donald][6]
- [Validus][7]

> This post assumes you have SQLite 3 installed and available in your path. Command-line snippets will be shown using PowerShell.

The final app will look something like:
!["FalcoJournal screenshot"](/img/2020-12-08-fsharp-falco/app.png)

## The Setup

We'll start by getting our project laid out, and a valid server running that responds to `GET /` with *"hello world"*. This lets us get the server setup cruft out of the way early, so we can focus on the actual functionality later on.

The first thing we'll do is install the Falco templates from NuGet:

```powershell
PS C:\> dotnet new -i "Falco.Template::*"
```

Next, we'll get our project setup:

```powershell
PS C:\Users\pim\code> New-Item -Path FalcoJournal\src -ItemType Directory
PS C:\Users\pim\code> cd FalcoJournal
PS C:\Users\pim\code\FalcoJournal> dotnet new sln
PS C:\Users\pim\code\FalcoJournal> dotnet new falco -o src -n FalcoJournal
PS C:\Users\pim\code\FalcoJournal> dotnet sln add .\src\FalcoJournal
```

And finally, install our dependencies:

```poweshell
PS C:\Users\pim\code\FalcoJournal> dotnet add .\src\FalcoJournal package System.Data.SQLite
PS C:\Users\pim\code\FalcoJournal> dotnet add .\src\FalcoJournal package Donald
PS C:\Users\pim\code\FalcoJournal> dotnet add .\src\FalcoJournal package Validus
```

At this point, we should have ourselves a working solution with a runnable Falco app. If you execute the commands below, a hello world app should be available at [https://localhost:5001](https://localhost:5001).

```powershell
PS C:\Users\pim\code\FalcoJournal> dotnet restore
PS C:\Users\pim\code\FalcoJournal> dotnet build
PS C:\Users\pim\code\FalcoJournal> dotnet run -p .\src\FalcoJournal
```

## The Domain

Our bullet journal has a dead simple domain, since we effectively only have one entity to concern ourselves with, a journal entry. But this is great because it will let us focus most of our energy on the web side of things.

To encapsulate our journal entry we'll need to know a few things, 1) the content, 2) when it was written and 3) a way to look it up in the future. 

In F# it will look something like:

```fsharp
type Entry = 
    { EntryId      : int
      HtmlContent  : string 
      TextContent  : string
      EntryDate    : DateTime }
```

With the definition taking a similar shape in SQL:

```sql
CREATE TABLE entry (
    entry_id       INTEGER  NOT NULL  PRIMARY KEY
  , html_content   TEXT     NOT NULL  
  , text_content   TEXT     NOT NULL
  , entry_date     TEXT     NOT NULL
  , modified_date  TEXT     NOT NULL);
```

> Notice the addition of `modified_date` here. We've purposefully omitted this from the F# model since we don't want the app to exude any control over this. But it will come in useful later for intraday sorting.

Another thing we'll want to model is the state changes of our entry, which will take the form of either 1) creating a new entry, or 2) updating existing entry. I like this approach, despite it involving some duplicate code, becuase it uses the type system to make our intent clear and more importantly prevents us from filling meaningful fields with placeholder data (i.e. junk).

```fsharp
type NewEntry = 
    { HtmlContent : string 
      TextContent : string }

type UpdateEntry =
    { EntryId     : int
      HtmlContent : string 
      TextContent : string }
```

It's important to stop here for a minute and appreciate that these two entities represent points of input from the user. This means it would likely be wise of us to put some guards in place to ensure that data follows the shape we expect it to. To do this we'll run input validation against the raw input values, using [Validus][7].

Our goal will be to:

- Ensure the user can't submit an empty entry.
- Ensure the user can't submit an entry with an invalid identifier.

```fsharp
type NewEntry = 
    { HtmlContent : string 
      TextContent : string }

    // equally valid to make this a module function
    static member Create html text : ValidationResult<NewEntry> =
        let htmlValidator : Validator<string> = 
            // ensure the HTML is not empty, and check for empty <li></li>
            // which is the default value
            Validators.String.notEmpty None
            <+> Validators.String.notEquals "<li></li>" (Some (sprintf "%s must not be empty"))

        let create html text = 
            { HtmlContent = html
              TextContent = text }            

        create
        <!> htmlValidator "HTML" html
        <*> Validators.String.notEmpty None "Text" text

type UpdateEntry =
    { EntryId     : int
      HtmlContent : string 
      TextContent : string }

    // equally valid to make this a module function
    static member Create entryId html text : ValidationResult<UpdateEntry> =        
        let create (entryId : int) (entry : NewEntry) = 
            { EntryId = entryId 
              HtmlContent = entry.HtmlContent 
              TextContent = entry.TextContent }

        // repurpose the validation from NewEntry, since it's shape
        // resembles this and also check that the EntryId is gt 0
        create 
        <!> Validators.Int.greaterThan 0 (Some (sprintf "Invalid %s")) "Entry ID" entryId 
        <*> NewEntry.Create html text
```

Now, this adds a fair chunk of code to our previous slim domain. But I'll argue it's worthwhile it for it to live here on the basis that this code is highly cohesive with the type and may end up being used in more than one place.

[Validus][7] comes with validators for most primitives types, which reside in the `Validators` module. They all share a similar function defition, excluding config params. Using some loose F# pseudocode:

```fsharp
Validators.Int.greaterThan {minValue} {validation message}
// int -> ValidationMessage option -> Validator 

ValidationMessage {fieldName}
// string -> string

Validator {fieldName} {value}
// string -> 'a -> ValidationResult<'a>
```

Validators can be composed using the `<+>` operator, or `Validator.compose`. This is demonstated above in the `let htmlValidator : Validator<string> = ...` which combines two string validators, including a custom message for the case of an empty `<li></li>`.

## The UI

...

[1]: https://github.com/pimbrouwers/Falco
[2]: https://github.com/fable-compiler/Fable
[3]: https://github.com/giraffe-fsharp/giraffe
[4]: https://github.com/SaturnFramework/Saturn
[5]: https://www.nuget.org/packages/System.Data.SQLite
[6]: https://github.com/pimbrouwers/Donald
[7]: https://github.com/pimbrouwers/Validus