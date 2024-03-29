[CmdletBinding()]
param()

#
# Template engine
function Invoke-Template {
	param([ScriptBlock] $scriptBlock)
  function Render-Template {
		param([string] $template)        
		Invoke-Expression "@`"`r`n$template`r`n`"@"
	}
	& $scriptBlock
}

$outputDir = "docs"
$template = Get-Content -Path .\template.html -Raw | Out-String

if(Test-Path -Path $outputDir) {
  Remove-Item $outputDir -Recurse -Force 
}

New-Item -ItemType Directory $outputDir | Write-Verbose

#
# Copy cruft
Copy-Item -Path CNAME, prism.css, prism.js, favicon.ico -Destination $outputDir -Recurse

#
# Copy images
New-Item -Path $outputDir\img -ItemType Directory | Write-Verbose
Copy-Item -Path img\* -Destination $outputDir\img -Recurse

#
# Build posts
$postMeta = 
Get-ChildItem .\posts -Filter *.md | Sort-Object -Descending | ForEach-Object {
  $fullName = $_.FullName

  Invoke-Template {
    $markdown = Get-Content $fullName -Raw | ConvertFrom-Markdown    
        
    $title = $markdown | 
      Select-Object -ExpandProperty Tokens | 
      Select-Object -Property @{n="Content";e={$_.Inline.Content}} -First 1 -Skip 1 | 
      Select-Object -ExpandProperty Content 
  
    $date = $markdown |
      Select-Object -ExpandProperty Tokens | 
      Select-Object -Property @{n="PostDate";e={$_.Inline.Content}} -First 1 -Skip 4 | 
      Select-Object -ExpandProperty PostDate
  
    $postHtml = $markdown | Select-Object -ExpandProperty Html  
  
    Write-Debug $postHtml

    $postNameUrlSafe = [regex]::Replace([regex]::Replace($title.ToString().ToLower().Replace("#", "sharp").Replace(".net", "dotnet"), "[^a-z]", "-"), "\-{2,}", "-")
    $postDateDir = [DateTime]::ParseExact($date, "MMMM d, yyyy", $null).ToString("yyyy/MM/dd")
    $postOutputDir = Join-Path -Path $outputDir -ChildPath $postDateDir 

    New-Item -ItemType Directory -Path $postOutputDir -Force
    | Write-Verbose

    $postFilename = "$($postNameUrlSafe).html"

    Render-Template $template 
    | Out-File -Path (Join-Path -Path $postOutputDir -ChildPath $postFilename)
    | Write-Verbose

    $meta = [pscustomobject]@{
      Title = $title
      Date  = $date
      Url   = (Join-Path $postDateDir -ChildPath $postFilename)
    }

    Write-Output $meta
  }
} 

Write-Verbose ($postMeta | Out-String)

#
# 404
Invoke-Template {  
  $title = "404 - Not Found | Pim Brouwers"
  $markdown = Get-Content 404.md -Raw | ConvertFrom-Markdown    

  $postHtml = $markdown | Select-Object -ExpandProperty Html  

  Write-Debug $title
  Write-Debug $postHtml

  Render-Template $template 
  | Out-File -Path (Join-Path -Path $outputDir -ChildPath "404.html")
  | Write-Verbose
}

#
# Index
Invoke-Template {
  $title = "Pim Brouwers"
  $rawContent = Get-Content index.md -Raw # load template
 
  $rawContentWithPostRoll = Render-Template $rawContent # expand variables
  $markdown = $rawContentWithPostRoll | ConvertFrom-Markdown 

  $postHtml = $markdown | Select-Object -ExpandProperty Html  

  Write-Debug $title
  Write-Debug $postHtml

  Render-Template $template 
  | Out-File -Path (Join-Path -Path $outputDir -ChildPath "index.html")
  | Write-Verbose
}