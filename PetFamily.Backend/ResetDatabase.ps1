param(
    [string]$InfrastructureProject = ".\src\PetFamily.Infrastructure",
    [string]$StartupProject        = ".\src\PetFamily.API",
    [string]$MigrationName         = "Initial"
)

Write-Host "==> Dropping database (if exists)..."

try {
    dotnet ef database drop `
        -p $InfrastructureProject `
        -s $StartupProject `
        --force `
        --no-build
} catch {
    Write-Host "Drop failed or database did not exist. Continuing..."
}

Write-Host "==> Removing old Migrations folder..."

$MigrationsPath = Join-Path $InfrastructureProject "Migrations"

if (Test-Path $MigrationsPath) {
    Remove-Item -Path $MigrationsPath -Recurse -Force
    Write-Host "Removed folder: $MigrationsPath"
} else {
    Write-Host "No Migrations folder found."
}

Write-Host "==> Creating new migration: $MigrationName"

dotnet ef migrations add $MigrationName `
    -p $InfrastructureProject `
    -s $StartupProject

Write-Host "==> Updating database..."

dotnet ef database update `
    -p $InfrastructureProject `
    -s $StartupProject `
    --verbose

Write-Host "==> Generating SQL script..."

dotnet ef migrations script `
    -p $InfrastructureProject `
    -s $StartupProject

Write-Host "==> Done!"