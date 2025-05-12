param (
    [string]$ProjectPath,
    [string]$Version
)

$projectDir = Split-Path -Path $ProjectPath -Parent

Write-Host "🔧 Building project at $ProjectPath with version $Version"

dotnet build "$ProjectPath" -c Release
dotnet pack "$ProjectPath" -c Release -p:PackageVersion=$Version

$nupkg = Get-ChildItem "$projectDir/bin/Release" -Filter *.nupkg | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($nupkg) {
    Write-Host "📦 Found package: $($nupkg.FullName)"
    dotnet nuget push $nupkg.FullName -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json --skip-duplicate
} else {
    Write-Error "❌ No NuGet package found!"
    exit 1
}