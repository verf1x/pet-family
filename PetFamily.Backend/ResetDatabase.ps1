param(
    [string]$InfrastructureProject = ".\src\PetFamily.Infrastructure\",
    [string]$StartupProject        = ".\src\PetFamily.API\",
    [string]$MigrationName         = "Initial"
)

Write-Host "🗑  Attempting to drop database (if it exists)…"
try {
    dotnet ef database drop `
        -p $InfrastructureProject `
        -s $StartupProject `
        --force `
        --no-build  # optional, to avoid rebuilding every time
}
catch [Npgsql.PostgresException] {
    if ($_.SqlState -eq '3D000') {
        Write-Host "   Database not found — skipping drop."
    }
    else {
        throw
    }
}

Write-Host "📂 Removing old Migrations folder (if it exists)…"
$MigrationsPath = Join-Path $InfrastructureProject "Migrations"
if (Test-Path $MigrationsPath) {
    Remove-Item $MigrationsPath -Recurse -Force
    Write-Host "   Deleted: $MigrationsPath"
}
else {
    Write-Host "   No Migrations folder found."
}

Write-Host "➕ Adding new migration '$MigrationName'…"
dotnet ef migrations add $MigrationName `
    -p $InfrastructureProject `
    -s $StartupProject

Write-Host "🚀 Applying migrations (and creating database if it doesn’t exist)…"
dotnet ef database update `
  -p $InfrastructureProject `
  -s $StartupProject `
  --verbose

Write-Host "✅ Done!"

Write-Host "📜 Generating SQL script for migrations…"
dotnet ef migrations script `
    -p $InfrastructureProject `
    -s $StartupProject
