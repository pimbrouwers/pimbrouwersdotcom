[« Go home](/)

# F# on the Web: A guide to building websites with Falco, .NET 5.x and ASP.NET Core

F# has a rich and mature development scene for both client- and server-side web programming. [Fable][2] revolutionized the durability with which you can craft client-side code, providing F#'s steadfast type system and an MVU programming model. On there server, [Giraffe][3], [Saturn][4] and [Falco][1] each offer their own unique way of providing similar durability on the server.

* * *

<small class="muted monospace">DECEMBER 8, 2020</small>

The focus of this post is going to be the [Falco Framework][1], where will aim to build a small, but complete-_ish_ bullet journal using .NET 5.x and ASP.NET Core. 

> The final solution can be found [here][8] if you'd like to follow along that way.

We'll be bringing in a couple of other packages to help us get the job done as well:

- [System.Data.SQLite][5]
- [Donald][6]
- [Validus][7]

The final app will look something like:

!["FalcoJournal screenshot"](/img/2020-12-08-fsharp-falco/app.png)

## Domain

Our bullet journal has a dead simple domain, since we effectively only have one entity to concern ourselves with, a journal entry. But this is great because it will let us focus most of our energy on the web side of things.

To encapsulate our journal entry we'll need to know a few things, 1) the content, 2) when it was written and 3) a way to look it up in the future. 

In F# it will look something like:

```fsharp
module FalcoJournal.Domain 

open System

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

Another thing we'll want to model is the state changes of our entry, which will take the form of either 1) creating a new entry, or 2) updating existing entry. I like this approach, despite it involving some duplicate code, becuase it uses the type system to make our intent clear and more importantly prevents us from filling meaningful fields with placeholder data to knowingly ignore.

```fsharp
module FalcoJournal.Domain 

open System

// ...

type NewEntry = 
    { HtmlContent : string 
      TextContent : string }

type UpdateEntry =
    { EntryId     : int
      HtmlContent : string 
      TextContent : string }
```

### Input Validation

It's important to stop here for a minute and appreciate that these two entities represent points of input from the user. This means it would likely be wise of us to put some guards in place to ensure that data follows the shape we expect it to. To do this we'll run input validation against the raw input values, using [Validus][7].

Our goal will be to:

- Ensure the user can't submit an empty entry.
- Ensure the user can't submit an entry with an invalid identifier.

```fsharp
module FalcoJournal.Domain 

open System

// ...

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

## UI

Next on our list are some of the UI elements that will be shared amongst our handlers, including things like buttons, form fields, the layout, top bar and error summary. We'll be using [tachyons][9] atomic CSS library since it's lovely for rapid UI development.

> Note that to save some space we won't be outlining all of the UI elements in the post. But the full source can be found in final [repository][8].

### Falco View Engine

A core feature of [Falco][1] is the XML markup module. It can be used to produce any form of angle-bracket markup (i.e. HTML, SVG, XML etc.).

Most of the standard HTML tags & attributes have been built into the markup module, which are pure functions composed into well-formed markup at run time. HTML tag functions are found in the `Elem` module, attributes in the `Attr` module. For string literal output there are several functions available in the `Text` module.

```fsharp
Elem.div [ Attr.class' "bg-red" ] [ Text.rawf "Hello %s" "world" ]
```

Rumor has it that the [Falco][1] creator makes a dog sound everytime he uses `Text.rawf`.

> **Note**: I am the creator, and this is entirely true.

### Extensible UI

Idiomatic usage of the `Markup` module will involve building yourself a suite of project-specific functions for shared UI elements/sections/components. Doing this will darastically simplify the markup code in the feature layer(s).

To demonstrate how this might look, the code below shows some of the common UI elements in our app.

The important takeaway is how extensible having a view engine comprised of pure functions is. End-of-the-line elements, like the `pageTitle` below, darastically simplify the API for creating the specific chunk of markup. Wrapper elements, like the `topBar` below, show how elegantly we can compose complex view thanks to the recursive structure of the `XmlNode` type.

```fsharp
module FalcoJournal.UI

open Falco.Markup

module Common = 
    /// Display a list of errors as <ul>...</ul>
    let errorSummary (errors : string list) =
        match errors.Length with
        | n when n > 0 ->
            Elem.ul [ Attr.class' "mt0 pa2 pl4 red bg-washed-red ba br1 b--red" ] 
                    (errors |> List.map (fun e -> Elem.li [] [ Text.raw e ]))

        | _ -> 
            Elem.div [] []

    /// Page title as <h1></h1>
    let pageTitle (title : string) =
        Elem.h1 [ Attr.class' "pb3 code tc white-90" ] [ Text.raw title ]

    /// Top bar with pluggable actions
    let topBar (actions : XmlNode list) =
        Elem.header [ Attr.class' "pv4" ] [
            Elem.nav [ Attr.class' "flex items-center" ] [
                Elem.a [ Attr.class' "f4 f3-l white-90 no-underline" 
                         Attr.href Urls.index ] [ Text.raw "Falco Journal" ]
                Elem.div [ Attr.class' "flex-grow-1 tr" ] actions ] ]
```

## Data Access

We're going to build ourselves a simple, yet robust data access layer using [Donald][6]. For the sake of simplicity, we'll be sticking to a synchronous approach here, which suits us well anyway given our use of SQLite.

The programming model for [Donald][6] is simple, build a `IDbCommand` from a SQL statement and parameter set, and execute against either an `IDbConnection` or `IDbTransaction` (for transactional scenarios). Given that the operation is occurring against a medium with side effects, the return type is always of `DbResult<'a>`.

To make the development process easier, I find it very useful to devise a "log and pass-through" function for `DbResult<'a>` (which conveniently returns the full statement and exception thrown):

> **Fun fact**: Unlike ADO.NET, [Donald][6] will return field-specific errors when they occur. Providing not only the full statement, but also the specific field which caused the error.

```fsharp
module FalcoJournal.Provider

open System.Data
open Donald
open Microsoft.Extensions.Logging
open FalcoJournal.Domain

// ...

module private DbResult = 
    /// Log DbResult, if error, and return
    let logError (log : ILogger) (dbResult : DbResult<'a>) : DbResult<'a> =
        match dbResult with
        | Ok _     -> dbResult
        | Error ex ->
            log.LogError(ex.Error, sprintf "DB ERROR: Failed to execute\n%s" ex.Statement)        
            dbResult

// ...
```

With this in place, we're ready to write some data access code. Again, to save time and space only a single example will be shown here. But the full source can be found in the [repository][8].

```fsharp
module FalcoJournal.Provider

open System.Data
open Donald
open Microsoft.Extensions.Logging
open FalcoJournal.Domain

// ...

module EntryProvider =  
    type Get = int -> DbResult<Entry option>

    let get (log : ILogger) (conn : IDbConnection) =
        fun entryId ->
            let sql = "
            SELECT    entry_id
                    , html_content
                    , text_content
                    , entry_date
            FROM      entry
            WHERE     entry_id = @entry_id"

            let fromDataReader (rd : IDataReader) : Entry =
                { EntryId      = rd.ReadInt32 "entry_id"
                  HtmlContent  = rd.ReadString "html_content"
                  TextContent  = rd.ReadString "text_content"
                  EntryDate    = rd.ReadDateTime "entry_date" }

            dbCommand conn {
                cmdText  sql
                cmdParam [ "entry_id", SqlType.Int32 entryId ]
            }
            |> DbConn.querySingle fromDataReader
            |> DbResult.logError log
    // ...
```

Nothing crazy going on here, but a couple of interesting points to discuss. Firstly, you'll notice that we've specified a function type to represent the operation. I enjoy the ergonomics of this approach, since it ultimately becomes the interface between the IO and business layer's.

The other point of note is that the function type contains no trace of the logger or database connection. So why is that? Since these are not concretely required to _perform_ the operation, they aren't the concern of our final consumers. But they're still required, so how to we fulfill this dependency? Partial application of course! And for that, we'll use a [**composition root**](#the-composition-root) to ensure we aren't having to manually manifest these depdendencies each time we want to interact with the database.

### A brief tangent on datbase connections

My preference for database-bound projects is to only specify how to create new `IDbConnection` instances in one place. So for that, we'll create a type to represent our **connection factory**.

```fsharp
type DbConnectionFactory = unit -> IDbConnection
```

And we'll register an instance of this within our service collection in `Program.fs`:

```fsharp
module Falco.Program

// ------------
// Register services
// ------------
let configureServices (connectionFactory : DbConnectionFactory) (services : IServiceCollection) =    
    services.AddSingleton<DbConnectionFactory>(connectionFactory)
    // ...

// -----------
// Configure Web host
// -----------
let configureWebHost (endpoints : HttpEndpoint list) (webHost : IWebHostBuilder) =        
    // ...
    let connectionString = appConfiguration.GetConnectionString("Default")
    let connectionFactory () = new SQLiteConnection(connectionString, true) :> IDbConnection
    // ...

```

## The composition root

Now this next little bit is completely optional, you can very effectively build Falco apps without doing this. But I love this approach because it gets me away from having to do the lambda-style `HttpHandler` every time I need access to dependencies, and almost becomes a tiny DSL for doing your business-logic that automatically enriches your IO layer with it's dependencies.

Since we're now in the web-tier so to speak, we can begin to leverage the composable `HttpHandler` type. In combination with a very simple function definition, we can save ourselves a TON of code repetition in our feature layer. The best part? All of this amazing functionality only costs you 29 lines of code, with comments!

```fsharp
module FalcoJournal.Service

open System.Data
open Falco
open Microsoft.Extensions.Logging
open FalcoJournal.Provider

/// Work to be done that has input and will generate output
type ServiceHandler<'input, 'output, 'error> = 'input -> Result<'output, 'error>

/// An HttpHandler to execute services, and can help reduce code
/// repetition by acting as a composition root for injecting
/// dependencies for logging, database, http etc.
let run
    (serviceHandler: ILogger -> IDbConnection -> ServiceHandler<'input, 'output, 'error>)
    (handleOk : 'output -> HttpHandler)
    (handleError : 'input -> 'error -> HttpHandler)
    (input : 'input) : HttpHandler =
    fun ctx ->
        let connectionFactory = ctx.GetService<DbConnectionFactory>()
        use connection = connectionFactory ()
        let log = ctx.GetLogger "FalcoJournal.Service"
                
        let respondWith = 
            match serviceHandler log connection input with
            | Ok output -> handleOk output
            | Error error -> handleError input error

        respondWith ctx
```

In a nutshell, we define a function type to represent a "service" (i.e. the work to be done) which effectively says "given an input you will either receive a successful result containing output, or an error". It's basic, but powerful. Next, we create an `HttpHandler` which will manifiest our dependencies, inject them into the provided service and based on the response type invoke either a _success_ or _failure_ handler.

With this in place, our final handlers will look like this, instead of each containing the code above.

```fsharp
let handle : HttpHandler = 
    let handleError input error : HttpHandler = // ...
    
    let handleOk output  : HttpHandler = // ...

    let workflow log conn input : ServiceHandler<unit, Output, Error> = // ...
    
    Service.run workflow handleOk handleError ()
```

## Entry Feature

Still with me? I realize it's been a lot of content to get here, but it's time for the raison d'être. Our app will be reponsible for 3 primary actions:

1. List all journal entries by date descending - `GET /`

2. Persistently create a journal entry - `POST /entry/create`

3. Persistently update a journal entry - `POST /entry/edit/{id}`

To do this, we're going to use a **feature-based** approach and encapsulate each action into it's own module with roughly the following shape:

```fsharp
module FalcoJournal.Entry

module Index = 
    type Input = // ... (optional)

    type Error = // ...

    let service : ServiceHandler<'a, 'b, Error> = // ...

    let view : XmlNode = // ...

    let handle : HttpHandler = // ...
```

[1]: https://github.com/pimbrouwers/Falco
[2]: https://github.com/fable-compiler/Fable
[3]: https://github.com/giraffe-fsharp/giraffe
[4]: https://github.com/SaturnFramework/Saturn
[5]: https://www.nuget.org/packages/System.Data.SQLite
[6]: https://github.com/pimbrouwers/Donald
[7]: https://github.com/pimbrouwers/Validus
[8]: https://github.com/pimbrouwers/FalcoJournal
[9]: /2018/12/05/tachyons-the-best-library-you-re-not-using.html