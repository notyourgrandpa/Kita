# Repository migration

Kita was extracted from GeneralPurposeApplication on 2026-10-25.

## Preserved history

- Original main: `7099be0cffce501d967812da06e1344f4c78a50a`.
- Feature branch: `feature/products` at `873a1d43be84e7d088da736fc213b959a214bb63`, including the local-only commits.
- Release branch: `release/09-30-2026` at `7cdcd774f0df36aa8e0e467b6a4ab57a0d5b316a`.
- Recovery tag: `gpa-before-kita-rename`.
- The original GPA repository retains its historical commits and closed pull requests.
- No existing releases, open issues, webhooks, Actions secrets, variables, environments, or branch rules required migration.

Development now belongs in this repository. Use `Kita.sln` and `Kita.Client`.
Historical feature/release branches retain their original naming until they are
reconciled with main. Do not merge GPA's README-only cleanup into Kita.

## Compatibility

Project names, namespaces, SPA references, Docker paths, and client metadata use
Kita. The database name `GeneralPurposeApplication`, Compose default project
name `generalpurposeapplication`, volume keys, UserSecretsId, and data-protection
application identity `KitaPOS` are preserved. Production audience/CORS values
remain unchanged; configure actual deployment values in the environment.

All EF migration IDs and operations are preserved, with namespace/type references
renamed consistently. EF reports no pending model changes.

The client lockfile is now tracked. The repository-local EF CLI is pinned to
8.0.16 to match the EF packages. Client Docker ignores exclude host dependencies;
the server restore explicitly excludes the JavaScript project.

## Verification

- Original Angular build and renamed Angular build pass with the same bundle hashes.
- Backend tests: 3 passed after repairing pre-existing stale test fixtures.
- EF model comparison: no changes since the last migration.
- Browser tests: all 3 pass in Linux CI; API readiness returns HTTP 200 locally.
- [Linux CI run 40196687897](https://github.com/notyourgrandpa/Kita/actions/runs/40196687897) passed the application checks, all three Docker image builds, and both Compose configurations.
- Existing nullable/decimal-precision and Angular bundle-size warnings are outside
  this repository migration.

The product fixture now exercises the current query handler and DTO/error
contract. The seed fixture uses the real seed service, isolated in-memory storage,
and password hashing, including an idempotence assertion. Client fixtures use
the current product/category service methods.

Swagger generation returns HTTP 500 in both the original and renamed builds because ProductsController.IsDupeProduct lacks an explicit HTTP method attribute. This pre-existing API documentation issue is unchanged.

No live production database or deployment has been modified. Existing deployments
must retain their Compose project identity and configuration when adopting Kita.
See [Docker setup](../DOCKER.md).

## Recovery

The verified all-ref Git bundle is retained locally outside both repository trees.
For code recovery, use the recovery tag or preserved branches. The GPA cleanup is
a normal commit and can be reverted without rewriting history.
