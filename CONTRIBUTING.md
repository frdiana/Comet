---
title: Contributing to comet
description: Contribution guide for commits, pull requests, local checks, and releases
---

## Contributing

Thanks for helping improve comet. This project is a small .NET CLI, so
changes should stay focused, readable, and easy to verify locally.

## Local Setup

Install the .NET 10 SDK, then restore and build the solution:

```bash
dotnet restore
dotnet build
```

Run tests before opening a pull request:

```bash
dotnet test
```

Run the published CLI locally:

```bash
comet --help
```

## Commit Messages

Use Conventional Commits. Release automation uses commit messages to prepare
versions, changelogs, and GitHub release notes.

Recommended commit types:

* `feat`: Adds a user-facing feature
* `fix`: Fixes a bug
* `docs`: Updates documentation only
* `test`: Adds or updates tests
* `refactor`: Changes code structure without changing behavior
* `build`: Changes build, packaging, or dependencies
* `ci`: Changes GitHub Actions or automation
* `chore`: Maintenance that does not affect runtime behavior

Examples:

```text
feat: add json recipe runner
fix: handle empty exclude container list
docs: document azure cli authentication
test: cover nested partition key parsing
ci: publish windows release artifact
```

For breaking changes, include `!` after the type or add a `BREAKING CHANGE`
footer:

```text
feat!: change clone recipe format
```

```text
feat: change clone recipe format

BREAKING CHANGE: recipe files now use an operations array.
```

## Branches and Pull Requests

Create a small branch for each change:

```bash
git checkout -b feat/json-recipe-validation
```

Before opening a pull request:

* Keep the change focused on one goal
* Run `dotnet build`
* Run `dotnet test`
* Update documentation when behavior or commands change
* Add or update tests for parsing, validation, and command behavior
* Avoid live Cosmos DB tests in the default test suite

Pull requests should explain:

* What changed
* Why the change is needed
* How it was verified
* Any known limits or follow-up work

## Code Style

Keep Spectre.Console command code split by command. Each command should have its
own command file and, when useful, its own settings file.

Preferred layout:

```text
src/comet/Commands/CreateDbCommand.cs
src/comet/Commands/CreateDbSettings.cs
src/comet/Commands/CloneDbCommand.cs
src/comet/Commands/CloneDbSettings.cs
```

Command classes should stay thin. They should parse settings, validate input,
call services, and render output. Business logic belongs in services, parsers,
models, or recipe classes.

## Release Process

Releases are automated with release-please and GitHub Actions.

The normal flow is:

1. Merge feature and fix PRs into `main` using Conventional Commits.
2. The release workflow runs when code, tests, project files, or release
	workflow files change.
3. The release workflow builds and tests the solution.
4. release-please creates a GitHub Release directly when the commit history
	contains releasable Conventional Commits.
5. The release artifact workflow publishes a Windows `comet.exe` zip.

Commits such as `feat: ...` and `fix: ...` create release notes and version
bumps. Commits such as `docs: ...` or `chore: ...` may not create a release by
themselves.

The release asset is named like this:

```text
comet-win-x64-vX.Y.Z.zip
```

The zip contains:

```text
comet.exe
README.md
LICENSE
```

## Scope

comet is intended for small and medium Cosmos DB moves. Do not add
features that imply support for million-document migrations, continuous
replication, or zero-downtime cutovers unless the project explicitly changes
scope.

