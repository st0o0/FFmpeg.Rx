# packaging Specification

## Purpose
TBD - created by archiving change v1-release. Update Purpose after archive.
## Requirements
### Requirement: SDK version pinning
The repository SHALL contain `src/global.json` pinning the .NET SDK version.

#### Scenario: Consistent SDK
- **WHEN** `dotnet --version` is run in the repo
- **THEN** it uses the pinned SDK version

### Requirement: Locked restore
`packages.lock.json` SHALL be committed for all projects. CI SHALL use `--locked-mode`.

#### Scenario: Reproducible restore
- **WHEN** CI runs `dotnet restore --locked-mode`
- **THEN** it succeeds without updating any packages

### Requirement: SourceLink
The library project SHALL include `Microsoft.SourceLink.GitHub` for source-level debugging in NuGet packages.

#### Scenario: Debug into source
- **WHEN** a consumer debugs into FFmpeg.Rx code
- **THEN** Visual Studio can fetch the source from GitHub

### Requirement: README with usage examples
`README.md` SHALL contain: project description, installation (`dotnet add package`), quick start example, builder API example, probing example, and link to full docs.

#### Scenario: Consumer onboarding
- **WHEN** a developer visits the NuGet page or GitHub repo
- **THEN** they can start using the library within 5 minutes

### Requirement: Release workflow
The repository SHALL use `dotnet-release-nuget.yml` from the shared workflows for NuGet publishing.

#### Scenario: NuGet release
- **WHEN** a GitHub Release is created
- **THEN** the package is built, tested, and pushed to NuGet

### Requirement: NuGet metadata
`Directory.Build.props` SHALL include `Description`, `PackageTags`, and `PackageIcon` for the NuGet listing.

#### Scenario: NuGet listing
- **WHEN** the package is on NuGet
- **THEN** it has a description, tags, and icon

