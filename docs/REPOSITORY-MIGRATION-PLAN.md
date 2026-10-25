# Kita repository migration plan

Historical proposal. See REPOSITORY-MIGRATION.md for implementation and verification.
Planning branch: `codex/kita-repository-migration-plan`, created from local `main`
at `7099be0`. Local `main` matched the cached `origin/main`; remote freshness
must be checked before execution.

## Decision

Create `notyourgrandpa/Kita` with the existing Git history, then turn the existing
`notyourgrandpa/GeneralPurposeApplication` default branch into a README-only
project directory. Kita owns its source, documentation, tests, releases, and
deployment configuration. GPA links to independently maintained repositories;
it has no solution, shared build, submodules, or application dependencies.

This is a history-preserving migration, not a filesystem copy with a new initial
commit. Preserve branches and tags, including local work not yet pushed.

| Option | Benefit | Cost | Decision |
| --- | --- | --- | --- |
| Rename GPA to Kita; create another GPA | Keeps existing GitHub metadata with the application | Reusing GPA's name breaks the old repository redirects | Avoid for this requirement |
| Create Kita with Git history; simplify existing GPA | Keeps the GPA URL stable and retains code history in Kita | GitHub metadata/settings need separate handling; GPA keeps historical source | Recommended |

GitHub explicitly warns against reusing the original repository name after a
rename because it disables redirects:
[GitHub repository rename documentation](https://docs.github.com/en/repositories/creating-and-managing-repositories/renaming-a-repository).

README-only means the current default-branch tree. Historical commits and legacy
branches still contain source. This does not reduce historical clone size. Do not
rewrite published history to achieve a visually minimal repository.

## Findings from this checkout

- The README mixes application setup with seven project ideas. Only Kita should
  appear in the new directory. Preserve useful setup instructions in Kita and
  leave the old ideas accessible in Git history.
- There are five .NET projects (Server, Application, Domain, Infrastructure,
  Tests) and one Angular client in the solution.
- A case-insensitive tracked-content search found GPA naming in 220 files.
- `feature/products` has commits absent from `main` and was three commits ahead
  of its cached upstream when inspected. Preserve it without silently merging it.
- Docker paths already contain inconsistent client casing: the tracked directory
  is `generalpurposeapplication.client`, while Compose references
  `GeneralPurposeApplication.Client`. Use exact casing throughout the migration.
- EF migration history is present. A branding rename must not reset that history
  or create an unrelated empty database.
- The README links to `GUIDE.md`, which was not present in the inspected root.
  Repair or remove that link when preparing Kita's documentation.

## Execution sequence

1. **Record and preserve the starting state.** Fetch remote refs, review divergence,
   record source commit IDs, branches, tags, and local-only work. Make and verify
   a Git bundle containing all local refs. Inventory any Git LFS objects, submodules,
   open issues/PRs, releases/assets, Actions settings, secrets, environments,
   permissions, webhooks, branch rules, and registry packages before migration.
   Local ignored files and secrets are not part of Git history; preserve necessary
   local configuration separately without publishing it.
2. **Create the destination.** Check whether `notyourgrandpa/Kita` already exists
   and inspect it before writing. For a new repository, use the intended visibility
   and do not initialize a README. Push the reviewed branch tips and tags into the
   empty destination. Resolve local/remote tip divergence explicitly, preserving
   both tips under distinct refs if necessary. Do not blindly mirror into an
   occupied destination. Verify the copied commit IDs, including feature work.
3. **Prepare Kita on a branch from its main.** Apply the naming map below, preserve
   the license, and rewrite its README around Kita's purpose and actual setup.
   Keep feature branches available for later reconciliation with the rename.
   Establish Kita as the sole home for new application work after cutover.
4. **Validate Kita independently.** Use a fresh checkout, run the verification
   checklist, and configure the destination's required GitHub settings. Git pushes
   do not copy issues, PRs, release assets, secrets, or repository settings. Handle
   applicable metadata separately; retain historical PR links in GPA and transfer
   or link active work deliberately.
5. **Cut over development and deployment.** Update developer remotes, CI checkout
   URLs, registry permissions, and deployment references. Existing GPA clones keep
   pointing at GPA unless explicitly changed. Keep the previous deployment
   available for rollback and verify Kita against a restored database copy first.
6. **Simplify GPA last.** Only after the Kita repository and required refs are
   verified and its builds pass, create a separate GPA cleanup branch. Remove
   tracked application/configuration/documentation files from its current tree
   and replace the README with the draft below. Review the deletion diff before
   merging. Disable obsolete GPA build/deployment automation, prevent accidental
   application work on old branches, and retain historical refs for recovery.

The planning document belongs with Kita's migration records and is removed from
GPA's final tree, leaving only `README.md`.

## Naming map and compatibility

| Current | Kita target |
| --- | --- |
| `GeneralPurposeApplication.sln` | `Kita.sln` |
| `GeneralPurposeApplication.Server` | `Kita.Server` |
| `GeneralPurposeApplication.Application` | `Kita.Application` |
| `GeneralPurposeApplication.Domain` | `Kita.Domain` |
| `GeneralPurposeApplication.Infrastructure` | `Kita.Infrastructure` |
| `GeneralPurposeApplication.Tests` | `Kita.Tests` |
| `generalpurposeapplication.client` directory/project | `Kita.Client` directory and `Kita.Client.esproj` |
| npm package and Angular project key | `kita.client` |
| Angular output directory | `dist/kita.client` |

Rename .NET directories, project filenames, namespaces, imports, assembly
references, solution entries, and test references together. Update SPA root,
Angular build targets, npm lockfile metadata, certificate scripts, client output
paths, Docker COPY/entrypoint paths, migration-bundle commands, Compose files,
documentation, and any CI configuration. Retain project GUIDs. Update tracked
solution launch configuration if it is kept; do not copy generated IDE caches.

Inspect references semantically rather than replacing every matching string:

- Preserve EF migration IDs and operations. Update namespace/type references in
  migrations and snapshots consistently; verify the model has no unexpected
  schema changes. Do not regenerate an initial migration.
- Keep existing production database names, SQL users, connection values, and
  persistent volumes compatible for the first release. Renaming these is a
  separate data migration, not a requirement for shorter project names.
- Explicitly preserve the existing Compose project/volume identity or map the
  existing volume before changing the checkout folder. A new Compose project name
  can otherwise select a new volume and make existing data appear missing.
- Preserve the UserSecretsId and existing authentication identifiers/keys unless
  there is an intentional configuration migration. Check data-protection key
  storage/application identity and authentication across the assembly rename.
- Regenerate local HTTPS certificates as needed when changing the npm package
  name; its current start scripts derive certificate paths from that name.
- Introduce new image names/tags deliberately and retain old images for rollback.
  Historical URLs and compatibility identifiers can retain the old name with an
  explanation; runtime code/project naming should use Kita.

Example developer commands after migration:

```powershell
git clone https://github.com/notyourgrandpa/Kita.git
cd Kita
dotnet run --project Kita.Server
# In another terminal, from the Kita checkout:
cd Kita.Client
npm ci
npm start
```

## Verification and rollback gates

- Record baseline build/test results before renaming so pre-existing failures are
  distinguished from regressions.
- Restore/build the renamed .NET projects and run `Kita.Tests`; install the client
  with `npm ci`, build production assets, and run its configured tests.
- Build frontend/backend/migration images and validate each maintained Compose
  configuration with its required environment. Verify Linux path casing.
- Confirm migration IDs and pending migrations are unchanged by the rename and
  test against both a fresh database and a restored existing database copy.
- Smoke-test login, product/inventory management, sales, expenses, and persistence
  after restart. Check local HTTPS/SPA proxy and production frontend routing.
- Search tracked content and filenames for all casings of the old name. Classify
  any remainder as intentional compatibility/history or a missed reference.
- Check the published Kita URL and all README documentation links. GPA's final
  `git ls-tree -r --name-only HEAD` must contain only `README.md`, with one project
  link to Kita. Only add future entries when their repositories exist.
- If Kita validation fails, leave GPA source intact. If a problem appears after
  cutover, revert the GPA cleanup commit and restore the previous deployment;
  use the recorded refs/bundle if needed. Do not delete Kita or force-push shared
  history as a rollback mechanism.

## Proposed final GPA README

Publish this only once the Kita destination is verified:

```markdown
# GeneralPurposeApplication

A directory of applications I develop. Each project has its own repository,
documentation, and setup instructions.

## Projects

- [Kita](https://github.com/notyourgrandpa/Kita) — A sari-sari store management
  system for tracking inventory, sales, and expenses. Under development.
```

Future projects follow the same pattern: an independent repository and a short
link here. Application-specific setup and dependencies stay in each project.
