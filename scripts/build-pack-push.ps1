param (
    [string]$ProjectPath,
    [string]$Version
)

Write-Host "🔧 Building project at $ProjectPath with version $Version"

dotnet build "$ProjectPath" -c Release
dotnet pack "$ProjectPath" -c Release -p:PackageVersion=$Version

$nupkg = Get-ChildItem "$ProjectPath/bin/Release" -Filter *.nupkg | Select-Object -First 1
if ($nupkg) {
    Write-Host "📦 Found package: $($nupkg.FullName)"
    dotnet nuget push $nupkg.FullName -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json --skip-duplicate
} else {
    Write-Error "❌ No NuGet package found!"
    exit 1
}