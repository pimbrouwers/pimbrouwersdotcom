param (
	[Parameter(HelpMessage='Minify the final output')]
	[switch] $Minify		
)

function Join-String {
	param(
		[Parameter(Mandatory=$True, ValueFromPipeline=$True)]
		$InputObject,

		[Parameter(Mandatory=$True)]
		[string] $Separator
	)

	begin {
		$first = $true
		$sb = [System.Text.StringBuilder]::new()
	}

	process {
		foreach($o in $InputObject) {
			if($first){
				[void]$sb.Append($o)
				$first = $false
			}
			else {				
				[void]$sb.Append($Separator)
				[void]$sb.Append($o)
			}
		}
	}

	end {		
		Write-Output $sb.ToString()
	}
}

function Invoke-Template {
	param([ScriptBlock] $scriptBlock)

	function Render-Template {
		param([string] $template)    

		Invoke-Expression "@`"`r`n$template`r`n`"@"
	}
	& $scriptBlock
}

#
# Globals
$workDir = $PSScriptRoot
$etcDir = Join-Path $workDir ".etc"
$binDir = Join-Path $workDir "bin"

#
# Output
if(Test-Path $binDir) {
	Get-ChildItem $binDir -Recurse | ForEach-Object {
		if($_.Name -ne "CNAME") {
			Remove-Item $_.FullName -Recurse
		}
	}	
} 
else {
	New-Item $binDir -ItemType Directory
}

#
# Assets
Join-Path $workDir "assets" |
Copy-Item -Destination $binDir -Recurse -Container

#
# Template literals
$templateDefault = Get-Content -Path (Join-Path $etcDir "template.html") -Raw | Out-String

#
# Make pages
$pagesDir = Join-Path $workDir "pages"
Get-ChildItem $pagesDir -Recurse -Filter *.html | ForEach-Object {			
	$processingFrontMatter = $false
	
	Invoke-Template {		
		[string]$body
				
		$body = [System.IO.File]::ReadLines($_.FullName) | ForEach-Object {
			if($_ -eq "---") {			
				if($processingFrontMatter -eq $true) {				
					# done processing front matter
					$processingFrontMatter = $false
				}						
				else {
					$processingFrontMatter = $true	
				}
			}
			else {						
				if($processingFrontMatter){
					# process front matter key-value
					$key, $value = $_.Split(":")
					New-Variable -Name $key -Value $value.Trim()					
				}
				else {									
					Write-Output $_
				}
			}
		} | Join-String -Separator `r`n
		
		$htmlFilename = $_.Name

		if($_.Name -ne "index.html") {
			if ($date) { # comes from front-matter
				$dirName = Join-Path (Join-Path "blog" $date) $_.Name.Replace(".html", "")				
			}		
			else {
				$dirName = $_.Name.Replace(".html", "")
			}

			$htmlFilename = Join-Path $dirName "/index.html"							
			New-Item -Path (Join-Path $binDir $dirName) -ItemType Directory
		}
		
		$body = Render-Template $body
		
		Render-Template $templateDefault | Out-File (Join-Path $binDir $htmlFilename)
	}
}