[« Go home](/)

# Reading CSV's and other delimited files with F#

F# really excels in automated code generation, [type providers](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/) are a perfect example of this. These type providers will automatically generate types for your data based on sampling, which is provided inline, from a `StreamReader` or sometimes via URL. If you're exploring a new data source, like an API, these are wickedly sexy and effective tools. However, in production they can become a bit of pain to use and manage.

* * *

<small class="muted monospace">FEBRUARY 2, 2019</small>

F# is absolutely loaded with what I'll call "language tools", and providers are an excellent example of this. Which makes it easy to forget that you will at times need to go lower and in most situations I'd suggest you'll either want to be there or inevitably wind up there.

Today we're going to focus on the task of parsing delimited files, most commonly comma-delimited, but often found with `\t` and `|` characters as well. For that, were going to be using [CsvHelper](https://joshclose.github.io/CsvHelper/), an incredible .NET library maintained by Josh Close & contributors.

Using imperative libraries in F# typically means writing small adapter functions. Below is a code snippet demonstrating how you could do this in an extensible way by writing a function which is configurable at runtime. 

For the purpose of the example, we'll focus on parsing pipe-delimited (i.e. `|`) files, as it often comes in handy when commas and tabs chafe.

```fsharp
module DelimitedFile

open System.IO
open CsvHelper 

/// Load, Parse and apply mapping
let read (config : Configuration) (map : CsvReader -> 'a) (path : string) =
    seq {
        use reader = new StreamReader(path = path)
        use csv = new CsvReader(reader, configuration = config)         

        csv.Read() |> ignore
        csv.ReadHeader() |> ignore

        while csv.Read() do
            yield map csv          
    }

/// Read pipe delimited files
let readPipe (map : CsvReader -> 'a) (path : string) = 
    let config = Configuration()
    config.Delimiter <- "|"
    readDelimited config path config 
```

To use simply provide a working file path and function with a `CsvReader -> 'a` signature.

```fsharp
type MyRecord = 
    { FullName : string }
    static member Create (fullName : string) = 
        { FullName = fullName }

let MyRecord =
    fromDelimited (reader : CsvReader) =
        MyRecord.Create (reader.GetField("full_name"))

let records = 
    "./some_delimited_file.txt" 
    |> DelimitedFile.readPipe MyRecord.fromDelimited 

records
|> Seq.iter (printfn "%A")
```

And that's it! You might be asking yourself why not use `csv.GetRecords<MyRecord>`? I personally like to avoid the `[<CliMutable>]` attribute when I can because that's one of the reasons I'm using F# in the first place. Creating new record types in F# is so terse that I am PERFECTLY happy to write my own mapping code, especially for tasks which involve consuming a reader.

It's important to remember that with very little code F# can be great at inteorping with imperative libraries, true to it's functional-first (but not purely functional) roots.