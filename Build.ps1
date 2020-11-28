function Invoke-Template {
	param([ScriptBlock] $scriptBlock)

	function Render-Template {
		param([string] $template)    

		Invoke-Expression "@`"`r`n$template`r`n`"@"
	}
	& $scriptBlock
}

$template = Get-Content -Path .\test-template.html -Raw | Out-String

Invoke-Template {
  $markdown = Get-Content .\test.md -Raw | ConvertFrom-Markdown
  

  $title = $markdown | 
    Select-Object -ExpandProperty Tokens | 
    Select-Object -Property @{n="Content";e={$_.Inline.Content}} -First 1 -Skip 1 | 
    Select-Object -ExpandProperty Content 

  $date = $markdown |
    Select-Object -ExpandProperty Tokens | 
    Select-Object -Property @{n="PostDate";e={$_.Inline.Content}} -First 1 -Skip 4 | 
    Select-Object -ExpandProperty PostDate

  $postHtml = $markdown | Select-Object -ExpandProperty Html  

  $outputDir = [DateTime]::ParseExact($date, "MMMM d, yyyy", $null).ToString("yyyy/MM/dd")
  $outputName = [regex]::Replace([regex]::Replace($title.ToString().ToLower().Replace("#", "sharp").Replace(".net", "dotnet"), "[^a-z]", "-"), "\-{2,}", "-")
  $outputPath = Join-Path -Path $outputDir -ChildPath "$outputName.html"
  
  Render-Template $template | Out-File -Path .\test.html
}
