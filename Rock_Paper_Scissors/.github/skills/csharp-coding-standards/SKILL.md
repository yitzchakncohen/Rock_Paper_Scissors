---
name: csharp-coding-standards
description: "Use when writing or refactoring non-Unity-specific C# code and you need conventions for naming, modern C# syntax, async behavior, architecture, and error handling."
---

# C# Coding & Architectural Standards

Follow these conventions and rules for C# code generation, refactoring, and architectural guidance.

## 1. Code Style & Naming Conventions
- **PascalCase**: Classes, Structs, Enums, Interfaces, Properties, Methods, and Namespaces.
- **camelCase**: Method parameters, local variables, and private/internal fields.
- **Prefixes & Suffixes**:
  - Prefix interfaces with `I` (e.g., `IRepository`).
  - Prefix private fields with an underscore `_` (e.g., `_logger`).
  - Suffix asynchronous methods with `Async` (e.g., `FetchDataAsync`).
- **Implicit Types**: Use `var` when the right-hand side clearly indicates the type (`var list = new List<string>()`). Use explicit types when the type is ambiguous or primitive (`int count = 5;`).

## 2. Modern C# Syntax Rules (C# 10+)
- **File-Scoped Namespaces**: Always use file-scoped namespaces (`namespace MyProject.Services;`) to reduce indentation.
- **Primary Constructors**: Use primary constructors for dependency injection in classes and record declarations where applicable.
- **Pattern Matching**: Prefer pattern matching expressions (`switch` expressions, `is not null`, property patterns) over legacy `if-else` chains.
- **Records**: Use `record` or `readonly record struct` for immutable data transfer objects (DTOs) and domain events.
- **Nullability**: Enforce `#nullable enable`. Explicitly annotate nullable references (`string?`) and use null-coalescing operators (`??`, `??=`, `?.`).

## 3. Asynchronous & Performance Guidelines
- **Async/Await**: Avoid `async void` except for event handlers. Always return `Task` or `ValueTask<T>`.
- **Cancellation**: Pass `CancellationToken cancellationToken = default` as the final parameter in asynchronous operations.
- **Configure Context**: Call `.ConfigureAwait(false)` in non-UI libraries to prevent deadlocks.
- **Memory Allocation**: Use `ReadOnlySpan<T>` and `ReadOnlyMemory<T>` for high-performance string manipulation or collection slicing to minimize GC pressure.
- **Collection Initialization**: Use target-typed collection expressions `[]` where clear (e.g., `List<int> numbers = [1, 2, 3];`).

## 4. Architecture & Error Handling
- **Dependency Injection**: Design classes around constructor-based dependency injection. Interfaces should be loosely coupled.
- **Exceptions**: Throw specific standard or custom exceptions (`ArgumentNullException`, `InvalidOperationException`). Do not catch generic `Exception` without rethrowing or handling appropriately.
- **Result Pattern**: Prefer functional result/either patterns or custom wrapper types over throwing exceptions for expected domain control flows.
- **Resource Cleanup**: Always use `using` statements/declarations (`using var stream = ...`) or implement `IAsyncDisposable` / `IDisposable` properly.

## 5. Documentation & Comments
- Provide XML documentation comments (`/// <summary>`) for public APIs, classes, and complex methods.
- Write self-documenting code with clear variable and method names rather than adding obvious inline comments.
