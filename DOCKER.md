# Docker

Build from the Kita repository root. Copy `.env.example` to `.env` and supply
your database password, JWT key, seed-user passwords, and allowed origin.
Docker Engine and Docker Compose v2 are required; the production overlay uses
the `!reset` and `!override` YAML tags.

## Development

```sh
docker compose up --build
```

The frontend is served at `http://localhost:4200`, with API traffic proxied through
nginx. SQL Server is published on port 1434 and the API on port 8080.

## Production configuration

```sh
docker compose -p YOUR_EXISTING_PROJECT_NAME -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

Keep your deployment's existing project name. For a new deployment, choose a stable
name. Configure the host reverse proxy to forward to `127.0.0.1:8081`.
The base file currently uses SQL Server Developer edition and the `sa` login;
production deployments need an appropriate edition and access configuration.

## Persistent data during the repository rename

The base file explicitly retains `name: generalpurposeapplication`, matching the
old default derived from the GPA checkout folder. Existing deployments that use
`-p` or `COMPOSE_PROJECT_NAME` must retain that override (for example,
`kita-prod-test`), which takes precedence over the base name.

The SQL database remains `GeneralPurposeApplication`. The volume keys remain
`sqlserver_data` and `dataprotection_keys`; the data-protection application identity
remains `KitaPOS`. This source rename does not migrate or clear data. Back up the
database and confirm the resolved volume names before a deployment cutover.
Do not use `docker compose down -v` on an installation whose data you need.

## Build layout

| File | Purpose |
| --- | --- |
| `Kita.Server/Dockerfile` | Publishes the API; starts `Kita.Server.dll` |
| `Kita.Infrastructure/Dockerfile.migrations` | Builds existing EF migrations into a bundle |
| `Kita.Client/Dockerfile` | Builds locked dependencies and serves `dist/kita.client/browser` using nginx |
| `Kita.Client/.dockerignore` | Excludes host node_modules and build output |
| `docker-compose.yml` | Database, health gate, API, and frontend |
| `docker-compose.override.yml` | Development settings |
| `docker-compose.prod.yml` | Migration job, persistent keys, production ports |

In production, startup proceeds from database readiness to the migration job,
then API readiness and the frontend. The migration image sets `DockerBuild=true`
to exclude the frontend project and `SkipSqlLogging=true` to avoid connecting to
SQL Server while building the bundle. Build-only placeholders are replaced by
the configured connection at runtime.

Historical release and feature branches are preserved separately; their unmerged
release Compose files have not been silently added to main.
