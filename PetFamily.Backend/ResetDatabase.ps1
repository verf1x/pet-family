param(
    [string]$InfrastructureProject = ".\src\PetFamily.Infrastructure\",
    [string]$StartupProject        = ".\src\PetFamily.API\",
    [string]$MigrationName         = "Initial"
)

Write-Host "🗑  Dropping database..."
dotnet ef database drop `
    -p $InfrastructureProject `
    -s $StartupProject `
    --force

Write-Host "🧹 Removing old Migrations folder..."
$MigrationsPath = Join-Path $InfrastructureProject "Migrations"
if (Test-Path $MigrationsPath) {
    Remove-Item $MigrationsPath -Recurse -Force
    Write-Host "   Deleted: $MigrationsPath"
} else {
    Write-Host "   No Migrations folder found."
}

Write-Host "➕ Adding new migration '$MigrationName'..."
dotnet ef migrations add $MigrationName `
    -p $InfrastructureProject `
    -s $StartupProject

Write-Host "🚀 Updating database..."
dotnet ef database update `
    -p $InfrastructureProject `
    -s $StartupProject

Write-Host "✅ Done!"
dotnet ef migrations script `
    -p $InfrastructureProject `
    -s $StartupProject
