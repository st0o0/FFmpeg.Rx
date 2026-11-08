## 1. AOT Example Project

- [x] 1.1 Create `src/FFmpeg.Rx.Example/FFmpeg.Rx.Example.csproj` with PublishAot, IsAotCompatible, InvariantGlobalization, IsPackable=false
- [x] 1.2 Create `src/FFmpeg.Rx.Example/Program.cs` exercising builders, probing types, events (no ffmpeg execution)
- [x] 1.3 Add Example project to `src/FFmpeg.Rx.slnx`

## 2. CI Hooks

- [x] 2.1 Update `.github/actions/ci-setup/action.yml` to install clang + zlib1g-dev alongside ffmpeg
- [x] 2.2 Create `.github/actions/ci-post-build/action.yml` with `dotnet publish Example -c Release -p:ContinuousIntegrationBuild=true`

## 3. README Rewrite

- [x] 3.1 Rewrite README.md following family pattern: badges → why → features → install → quick start → examples → architecture → building → contributing → license

## 4. Verify

- [x] 4.1 Run `dotnet build src/FFmpeg.Rx.slnx` — must succeed
- [x] 4.2 Run `dotnet format src/FFmpeg.Rx.slnx --verify-no-changes` — must pass
- [x] 4.3 Run `dotnet run --project src/FFmpeg.Rx.Tests/FFmpeg.Rx.Tests.csproj` — must pass
