## ADDED Requirements

### Requirement: AOT example project exists
An example project SHALL exist at `src/FFmpeg.Rx.Example/` with `PublishAot=true`, `IsAotCompatible=true`, and `InvariantGlobalization=true`. It SHALL reference the library and exercise the builder API surface.

#### Scenario: Example project compiles with AOT
- **WHEN** `dotnet publish src/FFmpeg.Rx.Example/ -c Release` is run
- **THEN** the project publishes successfully with no trim/AOT warnings

#### Scenario: Example project is not packable
- **WHEN** `dotnet pack` is run on the solution
- **THEN** the example project does not produce a NuGet package

### Requirement: CI verifies AOT publish
The CI pipeline SHALL run `dotnet publish` on the example project after build to verify AOT compatibility.

#### Scenario: CI setup installs AOT toolchain
- **WHEN** CI runs the setup hook
- **THEN** `clang` and `zlib1g-dev` are installed alongside `ffmpeg`

#### Scenario: CI post-build verifies AOT
- **WHEN** CI runs the post-build hook
- **THEN** `dotnet publish` is executed on the example project with `-c Release -p:ContinuousIntegrationBuild=true`

### Requirement: Example project is in the solution
The example project SHALL be included in `FFmpeg.Rx.slnx` so it participates in `dotnet build` and `dotnet format`.

#### Scenario: Solution includes example
- **WHEN** `dotnet sln list` is run on the solution
- **THEN** `FFmpeg.Rx.Example` appears in the project list
