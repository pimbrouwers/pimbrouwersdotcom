[« Go home](/)

# Reading CSV's and other delimited files with F#

F# really excels in automated code generation, [type providers](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/) are a perfect example of this. These type providers will automatically generate types for your data based on sampling, which is provided inline, from a `StreamReader` or sometimes via URL.

* * *

<small class="muted monospace">FEBRUARY 2, 2019</small>

If you're exploring a new data source, like an API, these are wickedly sexy and effective tools. However, in production they can become a bit of pain to use and manage.

F# is absolutely loaded with what I'll call "language tools", and providers are an excellent example of this. Which makes it easy to forget that you will at times need to go lower and in most situations I'd suggest you'll either want to be there or inevitably wind up there.

If you're working in .NET and not using [CsvHelper](https://joshclose.github.io/CsvHelper/) to read/write delimited files, you're missing out. It's an incredible tool.

Using imperative libraries in F# typically means writing small adapter functions. Below is a snippet from a module I frequently use to parse delimited files in my F# projects, including a sample of how to partially apply the base `read` function. `read` is configurable at runtime and accepts a file path & record mapper function.

```
module Csv =    
open CsvHelper 
open System.IO

let read config path mapCsv =
    seq {
        use reader = new StreamReader(path = path)
        use csv = new CsvReader(reader, configuration = config)         

        csv.Read() |> ignore
        csv.ReadHeader() |> ignore

        while csv.Read() do
            yield mapCsv csv          
    }

let readTabDelimited path mapCsv = 
    let config = Configuration()
    config.Delimiter <- "\t"
    read config path mapCsv</pre>
```

To use simply provide a working file path and function with a `CsvReader -> 'a` signature.

```
type MyRecord = { FullName : string }

Csv.readTabDelimited 
      "./some_tab_delimited_file.txt" 
      (fun csv -> { FullName = csv.GetField("FULL_NAME") })  </pre>
```

And that's it! You might be asking yourself why not use `csv.GetRecords<MyRecord>`? I personally like to avoid the `[<CliMutable>]` attribute when I can because that's one of the reasons I'm using F# in the first place. Creating new record types in F# is so terse that I am PERFECTLY happy to write my own mapping code, especially for tasks like consuming `IDbReader`'s.

It's important to remember that with very little massaging F# can be great at inteorping with imperative libraries.