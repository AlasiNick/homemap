# ASP.NET Core Empty

Look how to [create web APIs with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-8.0) to learn more.

## Setup

Make sure to restore dependencies and tools:

```bash
dotnet restore
```

Apply migrations:

```bash
dotnet ef database update --project ../Infrastructure/Infrastructure.Data/Infrastructure.Data.csproj
```

## Development Server

Start the development server:

```bash
dotnet run
```

[comment]: <> (TODO: Add  instructions on how to build for production)

## Local secrets (recommended approaches)

You should not commit secrets. Use one of the following during local development.

### Option A: .NET User Secrets (recommended)

Run these in the WebAPI project folder (`api/src/WebAPI`):

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Issuer" "homemap-api"
dotnet user-secrets set "Jwt:Audience" "homemap-app"
dotnet user-secrets set "Jwt:Key" "your-very-long-random-secret"
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
dotnet user-secrets list
```

User Secrets are read automatically when `ASPNETCORE_ENVIRONMENT=Development`.

### Option B: Environment variables

Use double underscores for nested keys.

Bash/Zsh (Linux/macOS):

```bash
export ASPNETCORE_ENVIRONMENT=Development
export Jwt__Issuer=homemap-api
export Jwt__Audience=homemap-app
export Jwt__Key="your-very-long-random-secret"
export Authentication__Google__ClientId="your-google-client-id"
export Authentication__Google__ClientSecret="your-google-client-secret"
dotnet run
```

PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:Jwt__Issuer = "homemap-api"
$env:Jwt__Audience = "homemap-app"
$env:Jwt__Key = "your-very-long-random-secret"
$env:Authentication__Google__ClientId = "your-google-client-id"
$env:Authentication__Google__ClientSecret = "your-google-client-secret"
dotnet run
```

CMD:

```bat
set ASPNETCORE_ENVIRONMENT=Development
set Jwt__Issuer=homemap-api
set Jwt__Audience=homemap-app
set Jwt__Key=your-very-long-random-secret
set Authentication__Google__ClientId=your-google-client-id
set Authentication__Google__ClientSecret=your-google-client-secret
dotnet run
```

## Visual Studio users
Open solution `api.sln`

Open Package Manager Console (PMC)

Select `Infrastructure.Data` (not `WebAPI`) project for PMC

Run `update-database` in PMC

Launch the solution using `https`
