# Contributing

Thanks for your interest in ProjectName!

## Development

ProjectName is a .NET library. Build and test from the repo root:

```bash
dotnet build src/ProjectName.slnx
dotnet test --solution ProjectName.slnx   # run from src/
```

## Pull requests

- Branch from `main` and open a PR. CI (build, test, lint) runs on pull requests.
- Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/)
  (`feat:`, `fix:`, `docs:`, `ci:`, `chore:`, …); commitlint enforces this and
  release-please derives the version and changelog from them.
- Keep changes focused and add or update tests for any behavior change.

## Reporting bugs / requesting features

Use the issue templates. For security issues see [`SECURITY.md`](SECURITY.md).

By contributing you agree to the [Code of Conduct](CODE_OF_CONDUCT.md).
