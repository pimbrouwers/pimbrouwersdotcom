[« Go home](/)

# SQL |> F#: Simple & powerful data access for .NET

In the spirit of starting small and finishing big, we'll look at the simple task of working with a relational database. Rather than immediately reaching for one of the libraries like SqlProvider (which is an excellent tool, but not the point here), we'll stick to just basic ADO.NET.

* * *

<small class="muted monospace">FEBRUARY 28, 2020</small>

To achieve a very useful module you actually need very little code, which is one of the major benefits of F# in my opinion. For our simple scope, we know we need:

* A way to create new commands.
* A way to execute statements (i.e. INSERT, UPDATE).
* A way to query records (i.e. SELECT).

To support the above syntax we'll create ourselves a module called SQL, import `System.Data` and for this example we'll use `System.Data.SqlClient` (though you could sub this with any vendor). Once we've gotten our dependencies imported, creating wrapper for ADO becomes trivial.

```fsharp
module Db

open System.Data
open System.Data.SqlClient

/// Create a new IDbCommand from SQL statement & parameters,
/// and execute against connection
let newCommand
    (sql : string)
    (parameters : seq<string * obj>)
    (conn : IDbConnection) =
    let createParameter (cmd: SqlCommand) (name : string, value : obj) =
        let p = cmd.CreateParameter()
        p.ParameterName <- name
        p.Value <- value
        p

    let addParameter (cmd: SqlCommand) (p : SqlParameter) =
        cmd.Parameters.Add(p) |> ignore

    let cmd = new SqlCommand(connection = conn, cmdText = sql)

    cmd.CommandType <- CommandType.Text
    parameters
    |> Seq.iter (fun p -> p |> createParameter cmd |> addParameter cmd)
    cmd

/// Execute an IDbCommand that has no results
let exec
    (sql : string)
    (parameters : seq<string * obj>)
    (conn : IDbConnection) =
    let cmd = newCommand sql param conn
    cmd.ExecuteNonQuery()

/// Execute an IDbCommand and map results during enumeration
let query
    (sql : string)
    (parameters : seq<string * obj>)
    (map : IDataReader -> 'a)
    (conn : IDbConnection) =
    let cmd = newCommand sql param conn
    use rd = cmd.ExecuteReader()
    [ while rd.Read() do yield map rd ]
```

> Notice that there is mutation happening here (anywhere you see `<-`). But that's okay because we've tucked it away and sand boxed it with a function wrapper.

So in about 40 lines of code we've created ourselves a small, well-understood and self documenting solution that can help us fully achieve the goal of interacting with a database. Pretty sweet!

Now let's see how we can use this module to interact with a database. We'll use a simple example of a table called `author` with columns `author_id`, `full_name`.

```fsharp
module MyApp

open System.Data
open System.Data.SqlClient

type Author = { Id : int; FullName : string }

let connStr = "{connection string goes here}"
let conn = new SqlConnection(connStr)
conn.Open()

let fullName = " Jim Bob-Magoo"
Db.exec
    "INSERT INTO author (full_name) VALUES (@name)"
    [ "name", fullName ]
    conn

let authors =
    Db.query
        "SELECT * FROM author"
        []
        (fun rd ->
            { Id = rd.GetInt32(0)
              FullName = rd.GetString(1) })

conn.Close()
```

If you find this approach helpful, feel free to checkout my open-source project [Donald](https://github.com/pimbrouwers/Donald), a well-tested F# interface for ADO.NET which is vendor agnostic built in this style.