# MyBGList

https://stackoverflow.com/questions/48758249/ef-core-migration-cant-use-secret-manager

$env:ASPNETCORE_ENVIRONMENT
$env:ASPNETCORE_ENVIRONMENT="Development"

dotnet ef migrations add Initial

dotnet ef database update Initial
