[« Go home](/)

# PowerShell

The longer I work as a software developer, the more I realize how important it is to draw your own conclusions. Curious about something? Try it. The cargo cult in programming can lead you down a path of misunderstanding. Uniformed opinions are everywhere. People snap to judgement about things they hardly understand, or worse have never used at all.

* * *

<small class="muted monospace">FEBRUARY 28, 2025</small>

I've noticed that .NET technology is especially victim to this. The hoard has deemed it bloated, enterprise garbage. And while there may have been growing pains in it's evoluation and vendor lock for a time, it couldn't be further from the truth today.

I want to focus on one of these technologies, PowerShell. Dun dun dun!!!

Did you know it was open-sourced back in 2016? It's even cross-platform now. Running on the latest [.NET](https://dotnet.microsoft.com/en-us/) runtime. It's dynamic, interactive, fast (enough) and comes packed with productivity. Once you learn even a small portion of it's features, you'll begin automating and generating scripts for everything.

> If you're unfamilar with the syntax, have a quick look at [Learn PowerShell in Y minutes](https://learnxinyminutes.com/powershell/). It's a great resource for getting started.

## More than a shell

If you're coming from a different shell environments one of the first things you need to know is that we're in the land of objects not strings now. (finish this thought)

### Getting help

PowerShell has an impressive built-in help system.

You can get help on any command by using the `Get-Help` cmdlet. For example, to get help on the `Get-Content` cmdlet, you would type:

```powershell
Get-Help Get-Content

# Need examples?
Get-Help Get-Content -Examples

# Or, looking for everything?
Get-Help Get-Content -Detailed

# It even support wildcards.
Get-Help Get-C*
```

Even more impressive are it's autocomplete and intellisense features. Just start typing a command and hit `Tab` to cursor through the available options. Or using `Ctrl+Space` to see all available options displayed in an interactive grid below the input line.

The beauty in all of this? Since PowerShell is object based you can even access this

### Deliciously simple syntax

Skipping over the basics, I want to quickly go over some of the things that can help make your scripts more interesting and configurable.

```powershell
$imAList = 1, 2, 3, 4, 5
$imAListToo = @(
    1
    2
    3
    4
    5)

Write-Output $imAList[0]
Write-Output $imAList[4]
$imAList += 6
Write-Output $imAList
```

```powershell
$imAHashTable = @{
    Name = 'John Doe'
    Age = 30
    Location = 'New York'
    Hobbies = @('Reading code', 'Writing code', 'Coding')
}
Write-Output $imAHashTable.Name
Write-Output $imAHashTable.Hobbies
Get-Member -InputObject $imAHashTable
```

```powershell
$imAnObject = [pscustomobject]@{
    Code = '
}
Write-Output $imAHashTable.Name
Write-Output $imAHashTable.Hobbies
Get-Member -InputObject $imAnObject
```

### Logging for opt-in debugging


### Design philosophy

The core philosophy behind PowerShell are single-purpose commands called "cmdlets". Your scripts are built by orchestrating and composing several of these cmdlets together to achieve a goal (ex: generating an HTML blog from markdown files).

Keep a very close mind on this **important** distinction between cmdlets and scripts. This will help keep your work segemented and easier to maintain.

Truthfully, most of your work will likely be scripting because the built-in cmdlets will take you very.


### Generating code and documents