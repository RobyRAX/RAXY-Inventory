# RAXY Inventory System

RAXY Inventory System provides a modular inventory foundation for Unity projects: multi-inventory support, item add/subtract/batch operations, and convert/craft flows.

## Features

- **InventoryManagerBase** — singleton manager with events for add, subtract, and batch changes
- **InventoryInstance** — per-inventory storage with stackable item support via `IItemFactory`
- **Convert/Craft** — `TryConvert` validates inputs, consumes items, and grants outputs atomically
- **Multi-inventory** — dictionary of inventory instances keyed by inventory id

## Setup

1. Create a concrete manager that extends `InventoryManagerBase` (e.g. `InventoryManager`).
2. Assign a ScriptableObject that implements `IItemDatabase`.
3. Assign a component that implements `IItemFactory`.
4. Optionally configure `InitialItems` and call `SendInitialItems()` at game start.

## Samples

Import **Basic Setup** from Package Manager → RAXY Inventory System → Samples:

- `SampleCurrencySO` + `SampleItemInstance_Currency` — minimal stackable currency
- `SampleItemDatabaseSO` + `SampleItemFactory`
- `Sample Inventory Manager` prefab with debug Add/Subtract and starter coins

## Dependencies

- **RAXY Core** (`com.raxy.core`) — addressable icon provider on `IItemEntry`
- **RAXY Localization** (`com.raxy.utility.localization`) — localized item text on `IItemEntry`
- **UniTask** (`com.cysharp.unitask`) — async item database initialization
- **Odin Inspector** (project plugin) — editor attributes on manager and containers; runtime works without Odin if attributes are stripped

## Notes

`IItemEntry` expects implementations that provide addressable icons and localization caches. Game-specific item types and factories should live in your project, not in this package. Use the package sample as a starting template.
