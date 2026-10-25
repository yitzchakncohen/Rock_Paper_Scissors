---
name: csharp-unity-standards
description: "Use when writing or refactoring Unity C# gameplay/runtime code and you need style, lifecycle, performance, and architecture guardrails for MonoBehaviours, events, serialization, and GC-sensitive paths."
---

# C# & Unity Architecture Standards

Follow these conventions, memory safety rules, and architectural guidelines for Unity code generation and refactoring in this workspace.

## 1. C# & Unity Code Style
- **Naming Conventions**:
  - PascalCase for `MonoBehaviour` classes, Structs, Enums, Interfaces, Properties, and Methods.
  - camelCase for parameters and local variables.
  - Private/Serialized fields use an underscore prefix: `_moveSpeed`, `_rigidBody`.
  - Interfaces prefix with `I` (e.g., `IDamageable`).
- **Explicit Types vs. `var`**:
  - Use `var` only when the type is explicit on the right-hand side (`var health = new HealthComponent()`).
  - Use explicit types (`float`, `Vector3`, `int`) for primitives and ambiguous engine calls.
- **Namespaces**: Group game features into logical namespaces (e.g., `namespace MyGame.Gameplay.Combat`).

## 2. Modern C# in Unity Rules
- **Nullability**: Respect `#nullable enable` where enabled. Treat Unity custom null checking carefully (do not use `?.` or `??` on `UnityEngine.Object` subclasses, as it bypasses Unity's native `Destroyed` state checks).
- **Primary Constructors & Records**: Use `readonly struct` or `record struct` for lightweight data containers, network payloads, and event data.
- **Pattern Matching**: Prefer `switch` expressions and property patterns over deep `if-else` branching.

## 3. Unity Performance & Garbage Collection (GC) Optimization
- **Zero Allocations in Update**:
  - Never call `GetComponent<T>()`, `Find()`, `new`, or LINQ queries inside `Update()`, `FixedUpdate()`, or tight inner loops.
  - Cache component references (`GetComponent<Rigidbody>()`) and `Transform` references in `Awake()` or `Start()`.
- **String Handling**: Use `StringBuilder` or pre-formatted string buffers for UI text updates to avoid allocation spikes in `Update()`.
- **String / Hash IDs**: Store Animator, Shader, and Input parameter IDs using `Animator.StringToHash()` or `Shader.PropertyToID()` as static readonly fields rather than passing string literals repeatedly.
- **Physics**:
  - Move rigidbodies only during `FixedUpdate()` or using `Rigidbody` API methods (`MovePosition`, `AddForce`).
  - Use NonAlloc physics queries (e.g., `Physics.OverlapSphereNonAlloc`) with reusable array buffers instead of standard array-returning physics calls.

## 4. Lifecycle & Component Architecture
- **Component Access**: Always check for missing dependencies in `Reset()` or `Awake()` via explicit `TryGetComponent` or `Debug.Assert`.
- **Coroutines & Async**:
  - Cache `WaitForSeconds` instances or yield instructions in member fields instead of allocating new instances inside Coroutines (`yield return _waitForOneSecond`).
  - Prefer `UniTask` or `Task` with proper `CancellationToken` binding to the `MonoBehaviour` lifecycle (e.g., `destroyCancellationToken`).
- **Events & Subscriptions**:
  - Unsubscribe from events/delegates in `OnDisable()` or `OnDestroy()` to prevent memory leaks and orphaned reference callbacks.

## 5. Serialized Fields & Editor Customization
- **Encapsulation**: Mark inspector fields as `[SerializeField] private` rather than `public`.
- **Attributes**: Annotate inspector fields with `[Header(...)]`, `[Tooltip(...)]`, and `[Range(...)]` to improve designer usability.
- **Serialization Safety**: Avoid referencing dynamic runtime state inside `OnValidate()` or field initializers.

## 6. XML Documentation
- Provide concise `<summary>` comments for all public methods, interfaces, custom editor tools, and scriptable objects.
