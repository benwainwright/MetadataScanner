# Path to the dotnet executable
$dotnet = "C:\Program Files\dotnet\dotnet.exe"

# Arguments to be passed to dotnet in order to initiate the test run
$targetargs = "test"

# Path to the opencover executable
$openCover = "C:\Users\bwain\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe"

# Path to the reportGenerator executable
$reportGenerator = "C:\Users\bwain\.nuget\packages\reportgenerator\3.1.2\tools\ReportGenerator.exe"

# Base name for report xml files (they will be named '<test.assembly.name>.$coverageFile')
$coverageFile = "Coverage.xml"

function Main {
    Run-All-Tests-In-Root -output "Coverage\raw"
    Generate-Reports-From-Reports-Dir -raw "Coverage\raw" -output "Coverage\reports" -history "Coverage\history" 
}

function Run-All-Tests-In-Root {

    Param($path, $output, $filter)

    if(!$path) {
        $path = (Get-Item -Path ".\" -Verbose).FullName
    }

    if(!(Test-Path -Path "$path\$output")){
        Write-Debug "$path\$output doesn't exist - creating"
        New-Item -ItemType directory -Path "$path\$output"
    }

    Get-ChildItem -Directory | ForEach-Object {
        
        $parts = $_.BaseName.split('.')
        $extension = $parts[$parts.Length - 1]
        if($parts.Length -gt 1 -and $extension -like "Test") {
            Run-Tests-In -directory $_.FullName -output $output -filter $filter
        }
    }
}

function Delete-Files-In {
    Param($directory)

    if($directory) {
        Get-ChildItem "$directory" -File -Filter "*.xml" | ForEach-Object {
            Remove-Item -Path $_.FullName
        }
    }
}

function Run-Tests-In {
    Param($directory, $output, $filter)

    $targetargs = "test"
    $coverageFile = "Coverage.xml"
    $root = (Get-Item -Path ".\" -Verbose).FullName
    $coverageDir = "$root\$output"
    if(Test-Path -Path "$coverageDir") {
        Delete-Files-In -directory "$coverageDir"
    }
    $baseName = (Get-Item -Path "$directory").BaseName

    $runOpenCoverCommand = "$openCover -oldStyle " + 
                                     " -register:user" +
                                     " -targetdir:`"$directory`"" +
                                     " -target:`"$dotnet`"" +
                                     " -output:`"$coverageDir\$baseName.$coverageFile`"" +
                                     " -targetargs:`"$targetargs`"" +
                                     " -returntargetcode" +
                                     " -skipautoprops" +
                                     " -hideskipped:All" +
                                     " -excludebyfile:`"*.Test.dll`""                                   

    Write-Debug "Running command - $runOpenCoverCommand"
    Invoke-Expression $runOpenCoverCommand
}

function Generate-Reports-From-Reports-Dir {

    Param($raw, $output, $history)

    $root = (Get-Item -Path ".\" -Verbose).FullName

    if($raw) {
        $raw = "$root\$raw"
    }

    if($output) {
        $output = "$root\$output"
    }

    if(Test-Path $output) {
        Delete-Files-In $output
    }

    $reports = (Get-ChildItem "$raw" -File -Filter "*.xml").FullName -join ';'

    $generateReportsCommand = "$reportGenerator -targetDir:`"$output`"" +
                                              " -reporttypes:`"Html;Badges`"" +
                                              " -reports:`"$reports`"" +
                                              " -verbosity:Error"

	if($history) {
		$history = "$root\$history"	
		if(!(Test-Path -Path "$history")){
			New-Item -ItemType directory -Path "`"$history"`"
		}	
		$generateReportsCommand  += " -historydir:`"$history`""
	}
    Write-Debug "Running command - $generateReportsCommand"
    Invoke-Expression $generateReportsCommand
}

Main