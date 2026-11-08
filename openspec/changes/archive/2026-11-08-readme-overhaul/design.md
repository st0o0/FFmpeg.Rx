## Context

FFmpeg.Rx README is minimal compared to the family pattern (WebSocket.Rx, Flickr.Net, GaudiHTTP). The library is AOT-clean but doesn't prove it in CI. Terp established the AOT verification pattern: example project + CI hooks, no `IsAotCompatible` on the library itself.

## Goals / Non-Goals

**Goals:**
- README follows the same structure as WebSocket.Rx / Flickr.Net / GaudiHTTP
- AOT proven in CI via example project (Terp pattern)
- `dotnet format` clean

**Non-Goals:**
- Logo, documentation site, API reference tables
- `IsAotCompatible` on the library csproj

## Decisions

### 1. README structure follows GaudiHTTP as primary template

GaudiHTTP is the closest analog (similar complexity, layer architecture, same author). Structure: badges → why → features → install → quick start → examples → architecture → building → contributing → license.

**Alternative: Flickr.Net style** — flatter, more emoji-heavy. Rejected: FFmpeg.Rx has architectural depth (3-layer model) that benefits from GaudiHTTP's architecture section pattern.

### 2. AOT via Example project, not library flag (Terp pattern)

Create `FFmpeg.Rx.Example` with `PublishAot=true`. CI hook publishes it after build. If library breaks AOT, CI fails.

**Alternative: `IsAotCompatible` on library csproj.** Rejected: If R3 or CliWrap introduce AOT issues, our NuGet package would be lying. Example-project approach proves it works without promising it.

### 3. Example project exercises builders only (no ffmpeg binary needed)

Program.cs builds pipeline specs and prints the serialized args. Doesn't execute ffmpeg. This means CI doesn't need ffmpeg to verify AOT — the existing `ci-setup` hook installs ffmpeg for integration tests, AOT verification is purely compile-time.

### 4. CI hooks use existing shared workflow infrastructure

`ci-setup/action.yml` already exists (installs ffmpeg). Add `clang` + `zlib1g-dev` there. New `ci-post-build/action.yml` runs the AOT publish. The shared `dotnet-ci.yml` auto-discovers both hooks via `hashFiles()`.

## Risks / Trade-offs

- **[R3/CliWrap AOT breakage]** → CI catches it immediately via example publish failure. No mitigation needed beyond the CI hook.
- **[InvariantGlobalization]** → Example uses `InvariantGlobalization=true` to avoid ICU. Library itself doesn't restrict this — consumers choose their own globalization mode.
