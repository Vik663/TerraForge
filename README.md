# TerraForge

![CI](https://github.com/Vik663/TerraForge/actions/workflows/ci.yml/badge.svg)
![Coverage](https://vik663.github.io/TerraForge/coverage-badge.svg)
![License](https://img.shields.io/github/license/Vik663/TerraForge)
![Platform](https://img.shields.io/badge/.NET-10-blue)

TerraForge is a modular terrain generation toolkit built on .NET. It focuses on predictable world generation, clear separation of responsibilities, and a structure that keeps the codebase easy to test and extend. Core logic stays lightweight and platform-independent, while rendering and native dependencies are isolated in their own modules.

---

## Overview

TerraForge provides:

- deterministic world generation based on seed values
- biome classification using height and moisture
- ASCII rendering for quick inspection
- PNG rendering powered by SkiaSharp
- a clean Domain/Core/Rendering architecture

The goal is to offer a simple and understandable foundation for procedural world generation that can be extended in many directions.

---

## Project Structure

TerraForge.Domain               // domain models and shared abstractions  
TerraForge.Core                 // world generation, biome logic, algorithms  
TerraForge.Rendering.Skia       // PNG rendering using SkiaSharp  
TerraForge.Tests                // tests for Domain/Core  
TerraForge.Rendering.Skia.Tests // rendering tests (Windows only)

This structure keeps the core logic platform-independent and ensures that rendering tests run only where native dependencies are supported.

---

## Building

```bash
dotnet build TerraForge.sln -c Release
```

Testing
Logic tests (Linux, macOS, Windows)

```bash
dotnet test TerraForge.Tests -c Release
```

Rendering tests (Windows only)

```bash
dotnet test TerraForge.Rendering.Skia.Tests -c Release
```

Rendering tests are isolated because SkiaSharp relies on native libraries that are not consistently available on Linux CI runners.

---

## Code Coverage

A full HTML coverage report is published automatically:

Coverage Report:  
https://Vik663.github.io/TerraForge/coverage/index.html

The coverage badge at the top of this page reflects the latest results from the main branch.

---

## Continuous Integration

The CI pipeline runs in two stages:

1. Linux job:
   - builds the solution
   - runs analyzers
   - executes all logic tests
   - generates and publishes coverage reports

2. Windows job:
   - runs PNG rendering tests

This setup keeps the pipeline fast and stable while ensuring rendering is validated in an environment where SkiaSharp behaves consistently.

---

## License

This project is released under the MIT License.
