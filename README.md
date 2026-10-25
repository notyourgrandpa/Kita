# Kita

A sari-sari store management system for tracking inventory, sales, and expenses.
Under development.

## Projects

- `Kita.Client` — Angular frontend.
- `Kita.Server` — ASP.NET Core API.
- `Kita.Application` — application services and use cases.
- `Kita.Domain` — domain models and contracts.
- `Kita.Infrastructure` — persistence, identity, and integrations.
- `Kita.Tests` — backend tests.

## Local development

Requires the .NET 8 SDK (or a compatible newer SDK), Node.js 20, and SQL Server.
Dependency versions are defined in the project files and the client lockfile.

```powershell
git clone https://github.com/notyourgrandpa/Kita.git
cd Kita
dotnet tool restore
cd Kita.Client
npm ci
cd ..
```

Configure your SQL Server connection string, JWT signing key, and seed-user
passwords using user secrets for `Kita.Server`. The existing UserSecretsId is
preserved, so developers migrating from GPA retain their existing secrets.

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your SQL Server connection string>" --project Kita.Server
dotnet user-secrets set "JwtSettings:SecurityKey" "<your signing key, at least 32 bytes>" --project Kita.Server
dotnet user-secrets set "DefaultPasswords:RegisteredUser" "<your seed-user password>" --project Kita.Server
dotnet user-secrets set "DefaultPasswords:Administrator" "<your seed-admin password>" --project Kita.Server
```

Apply the existing migrations to your development database:

```powershell
dotnet ef database update --project Kita.Infrastructure --startup-project Kita.Server -- --SkipSqlLogging=true
```

Run the backend:

```powershell
dotnet run --project Kita.Server --launch-profile https
```

The SPA proxy can start the frontend automatically. To run it separately:

```powershell
cd Kita.Client
npm start
```

The client uses HTTPS at `https://localhost:4200`; its prestart script exports the
development certificate under the new `kita.client` name. If needed, trust the
certificate using `dotnet dev-certs https --trust`. The API runs at
`https://localhost:8701`.

## Verification

```powershell
dotnet test Kita.Tests/Kita.Tests.csproj -p:DockerBuild=true
cd Kita.Client
npm run build
npm test -- --watch=false --browsers=EdgeHeadless
```

`DockerBuild=true` excludes the JavaScript project from the backend build;
the client is built separately using npm.

## Documentation

- [Docker setup](DOCKER.md)
- [FAQ](FAQ.md)
- [Troubleshooting](TROUBLESHOOTING.md)
- [Repository migration](docs/REPOSITORY-MIGRATION.md)

Kita was extracted from
[GeneralPurposeApplication](https://github.com/notyourgrandpa/GeneralPurposeApplication),
which now serves as the project directory. The application's Git history is preserved.
