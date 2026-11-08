## Why

The FFmpeg.Rx README is functional but doesn't match the pattern established across the other .NET libraries (WebSocket.Rx, Flickr.Net, GaudiHTTP). Missing: feature overview, "why this library" pitch, architecture diagram, .NET version badge, downloads badge, and contributing section. A developer landing on the NuGet page or GitHub repo has no quick way to understand what differentiates FFmpeg.Rx from FFMpegCore or Xabe.FFmpeg.

Additionally, the library is already AOT-clean (source-generated JSON, no reflection) but doesn't prove it in CI. Following the Terp pattern: an AOT example project + CI hook proves compatibility without committing the library itself to `IsAotCompatible`.

## Goals

- Align README structure with the library family pattern (WebSocket.Rx, Flickr.Net, GaudiHTTP)
- Add "Why FFmpeg.Rx?" positioning section
- Add grouped feature bullets
- Add architecture ASCII diagram (3-layer model)
- Add missing badges (downloads, .NET version)
- Add contributing section
- Prove AOT compatibility via example project + CI (Terp pattern)

## Non-Goals

- Logo creation (separate effort)
- Documentation site / wiki
- API reference tables (code-level docs are out of scope)
- `IsAotCompatible` on the library csproj itself (proven by example, not promised)

## What Changes

- **README.md**: Restructured to match family pattern — header/badges → why → features → install → quick start → examples → architecture → building → contributing → license. Mention AOT compatibility.
- **src/FFmpeg.Rx.Example/**: New project — `PublishAot=true`, `IsAotCompatible=true`, `InvariantGlobalization=true`. Real app exercising builders, probing, events. No ffmpeg binary needed at compile time.
- **.github/actions/ci-setup/action.yml**: Add `clang` + `zlib1g-dev` alongside existing `ffmpeg` install
- **.github/actions/ci-post-build/action.yml**: New hook — `dotnet publish Example -c Release -p:ContinuousIntegrationBuild=true`
- **FFmpeg.Rx.slnx**: Add Example project to solution
- **dotnet format**: Run as part of the change

## Capabilities

### New Capabilities

- `readme-structure`: README layout, section ordering, badges, feature overview, architecture diagram, contributing section, AOT mention
- `aot-verification`: AOT example project, CI hooks for native AOT publish verification (Terp pattern)

### Modified Capabilities

- `packaging`: Solution file updated to include example project

## Impact

- `README.md` — full rewrite maintaining existing code examples
- `src/FFmpeg.Rx.Example/` — new project (not packable, not shipped)
- `.github/actions/` — CI hook additions
- `FFmpeg.Rx.slnx` — solution membership
- No library code changes, no API changes, no test changes
