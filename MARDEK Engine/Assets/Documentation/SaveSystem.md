# Save System Overview

High-level architecture of MARDEK Engine's save system. This is a big-picture reference, not an exhaustive API reference — see the listed files for implementation details.

## Core Flow

Persistence is **GUID-addressable**: rather than one hand-written save-data class, any object that wants to persist registers itself under a stable `Guid` and serializes/deserializes independently. **`SaveSystem`** (`Scripts/MARDEK/Save/SaveSystem.cs`) is the static facade all of this routes through — it holds one in-memory **`SaveState`** (a `Dictionary<Guid, JObject>`) and exposes `SaveObject`/`LoadObject` plus the file-level `SaveToFile`/`GetSaveStateFromFile`.

Saving a slot is a two-phase collection: `SaveToFile` fires a static `OnBeforeSave` event first, so every currently-enabled persistent object writes its own JSON into the shared dictionary, *then* the whole dictionary is serialized to disk as one file. Loading is the mirror image: the file is parsed back into a `SaveState`, and each object pulls its own entry back out by GUID when it wakes up.

Serialization runs on **Newtonsoft.Json** (`com.unity.nuget.newtonsoft-json`), configured via a shared `JsonSerializer` in `SaveSystem` with:
- A custom **`SaveContractResolver`** — makes Newtonsoft serialize the way Unity does: public fields plus `[SerializeField]` private fields (including on base classes), no properties, and it skips `UnityEngine.Object` references entirely *except* addressable ScriptableObjects.
- A **`GuidReferenceConverter`** — see [Asset references](#asset-references-by-guid) below.

```csharp
// SaveSystem.SaveToFile
OnBeforeSave.Invoke();
string json = JsonConvert.SerializeObject(internalSaveState.addressableState, serializerSettings);
System.IO.File.WriteAllText(filePath, json);
```

```csharp
// SaveState.LoadObject
if (addressableState.TryGetValue(guid, out JObject data) == false || data == null)
    return false;
using (JsonReader reader = data.CreateReader())
    serializer.Populate(reader, addressable); // deserializes onto the live object in place
```

## Scene objects: `AddressableMonoBehaviour`

`AddressableMonoBehaviour` (`Scripts/MARDEK/Save/AddressableMonoBehaviour.cs`) is the base class for any scene `MonoBehaviour` that needs to persist. It:
- Auto-assigns and serializes a 16-byte `Guid` in `OnValidate`, exposed via `IAddressableGuid.GetGuid()`.
- Has a `SaveOptions` block controlling lifecycle: `loadOnAwake` (calls `Load()` in `Awake`), `autoSave` (subscribes `Save()` to `SaveSystem.OnBeforeSave` while enabled), `saveOnDisable` (calls `Save()` in `OnDisable`).
- `Save()`/`Load()` just delegate to `SaveSystem.SaveObject(this)` / `SaveSystem.LoadObject(this)`. Subclasses can override `Save()` to capture extra state first (see `GeneralProgressData` below).

Current subclasses:

| Class | Persists |
|---|---|
| `GeneralProgressData` (`Save/Addresables/GeneralProgressData.cs`) | The save-slot header: `currentScene` (active scene path), `SceneName` (display name), `SavedTime`, `GameName`. This is what the save/load UI previews and what `LoadScene()` uses to restore the right scene. |
| `BoolComponent` (`Save/Addresables/BoolComponent.cs`) | A single persisted bool flag (quest/plot switches); saves on `OnDestroy`. |
| `ExploredAreas` (`Progress/ExploredAreas.cs`) | Minimap fog-of-war — a `Dictionary<string, ExploredArea>` keyed by scene ID. |

## Asset references by GUID

Persistent asset data (items, skills, character profiles, party rosters, etc.) is defined as **ScriptableObjects**, which can't be meaningfully deep-copied into a save file — instead they're saved **by reference**.

- **`AddressableScriptableObject`** (`Core/AddressableSO/AddressableScriptableObject.cs`) is the base class for any such asset; its `GetGuid()` looks itself up in the database.
- **`AddressableDatabase`** (`Core/AddressableDatabase.cs`) is a `ScriptableObject` (loaded via `Resources.Load("Database")`) holding parallel `guids`/`objects` lists, rebuilt in the editor from `AssetDatabase.FindAssets` filters. It's the two-way lookup: GUID → asset and asset → GUID.
- **`GuidReferenceConverter`** (`Core/GuidReferenceConverter.cs`) is registered on the shared `JsonSerializer` and applies to any `AddressableScriptableObject` field. It writes `{"refGuid": "<guid>"}` instead of the object's contents, and on read resolves that GUID back through `AddressableDatabase.GetAddressableByGuid` rather than constructing a new instance — so a saved reference to, say, an `Item` asset always resolves back to the same singleton asset rather than a disconnected copy.

## Save/Load UI flow

- **Save Crystal** (`Prefabs/WorldObjects/Save Crystal.prefab`) is a world object with a `CommandTrigger` → dialogue → `ChoicesCommand` chain offering Save/Load/Party-selection. Choosing Save or Load calls `SaveLoadMenu.EnableAsSaveOrLoad(bool)`.
- **`SaveLoadMenu`** (`UI/SaveUI/SaveLoadMenu.cs`) opens the menu and instantiates a paginated grid of `SaveFileBox`es, 7 per page, numbered sequentially across pages.
- **`SaveFileBox`** (`UI/SaveUI/SaveFileBox.cs`) is one slot, named `"MARDEK_save_" + number`. On click:
  - Save mode → `SaveSystem.SaveToFile(saveFileName)`.
  - Load mode (if the file exists) → `SaveSystem.CallGameFileLoaderScene(saveFileName)`.
  - It also previews itself by loading the file's `SaveState` into a scratch `GeneralProgressData` to read name/time/scene without touching the live game state.
- **`LastSavedAndLoadedPage`** (`UI/SaveUI/LastSavedAndLoadedPage.cs`) shows quick-access boxes for the most recent save/load, read from `PlayerPrefs`.

**Important:** the save slot the player picks is just a filename — `PlayerPrefs["lastSavedFile"]` is updated on *every* `SaveToFile` call regardless of which slot number was used, so it always points at whichever crystal/slot the player saved at most recently. This is what the battle-defeat reload (see below) keys off of.

## The loader scene

`SaveSystem.CallGameFileLoaderScene(fileName)` doesn't load the save synchronously — it stashes the filename in a static field, writes `PlayerPrefs["lastLoadedFile"]`, and loads a dedicated scene (build index 1, `Scenes/GameFileLoader.unity`) whose objects run the actual restore on `Awake`, in this order:

1. `SaveSystem.LoadCurrentSaveFileNameIntoInternalSaveState()` — parses the file into the in-memory `SaveState`.
2. `MapParty.SetForceLoadOnNextAwake()` — tells the map party controller to rebuild from saved data rather than scene defaults.
3. `GeneralProgressData.Load()` — pulls `currentScene`/`SceneName`/`SavedTime` out of the loaded `SaveState`.
4. `GeneralProgressData.LoadScene()` — loads whichever scene path was recorded in the save.

Once that scene loads, every `AddressableMonoBehaviour` with `loadOnAwake` set pulls its own state back out of the (now-populated) internal `SaveState` by GUID in its own `Awake`.

## Triggering a load from gameplay: battle defeat

When the player's party is wiped in battle, `BattleManager.Defeat()` (`Battle/BattleManager.cs`) reloads the most recent save automatically:

```csharp
string lastSavedFile = PlayerPrefs.GetString("lastSavedFile", string.Empty);
if (string.IsNullOrEmpty(lastSavedFile))
{
    Debug.LogWarning("Party was defeated but no save file exists to reload");
    yield break;
}
SaveSystem.CallGameFileLoaderScene(lastSavedFile);
```

This reuses the exact same loader-scene mechanism a manual load does — there's no separate "continue" or "game over" code path. If the player has never saved, it logs a warning and does nothing (the battle scene is left as-is) rather than crashing on an empty filename.

## Options: a separate persistence layer

Player options (audio volume, key rebinds, framerate cap) are **not** part of save slots — they're stored directly in Unity `PlayerPrefs`, keyed independently of any save file, and apply globally regardless of which save is loaded:

- `AudioGroupVolume` (`UI/Options/AudioGroupVolume.cs`) — per-mixer-group volume.
- `KeyRebind` / `LoadKeyRebinds` (`UI/Options/`) — input binding overrides, keyed by binding GUID.
- `FramerateCapOption` (`UI/Options/FramerateCapOption.cs`) — framerate cap int.

## Notable Gaps

- **No versioning/migration.** There's no schema-version field, so a save made before a persisted class's fields change will simply deserialize with whatever data happens to line up — no migration or compatibility layer exists yet.
- **No atomic writes.** `SaveToFile` writes straight over the target file with `File.WriteAllText`; a crash or power loss mid-write would corrupt that slot rather than leaving the previous save intact.
- **No corrupt-file handling.** `GetSaveStateFromFile` doesn't guard against a missing/corrupt file — a bad save throws rather than failing gracefully.
- **Victory has the same "never leaves the battle scene" gap defeat used to have** — defeat now reloads the last save, but a battle *won* still leaves the player sitting in `BattleScene` with no transition back to the map (see `Documentation/BattleSystem.md`).

## Key Files

| Area | Path |
|---|---|
| Core facade | `Scripts/MARDEK/Save/SaveSystem.cs` |
| In-memory save data | `Scripts/MARDEK/Save/SaveState.cs` |
| Serialization contract | `Scripts/MARDEK/Save/SaveContractResolver.cs` |
| Scene-object base class | `Scripts/MARDEK/Save/AddressableMonoBehaviour.cs` |
| Save-slot header | `Scripts/MARDEK/Save/Addresables/GeneralProgressData.cs` |
| Other persisted scene objects | `Scripts/MARDEK/Save/Addresables/BoolComponent.cs`, `Scripts/MARDEK/Progress/ExploredAreas.cs` |
| Asset-by-reference | `Scripts/MARDEK/Core/AddressableSO/AddressableScriptableObject.cs`, `Scripts/MARDEK/Core/AddressableDatabase.cs`, `Scripts/MARDEK/Core/GuidReferenceConverter.cs` |
| GUID interface | `Scripts/MARDEK/Core/IAddressableGuid.cs` |
| Save/Load UI | `Scripts/MARDEK/UI/SaveUI/SaveLoadMenu.cs`, `SaveFileBox.cs`, `LastSavedAndLoadedPage.cs` |
| Loader scene | `Scenes/GameFileLoader.unity` (build index 1) |
| Save Crystal | `Prefabs/WorldObjects/Save Crystal.prefab` |
| Battle-defeat reload | `Scripts/MARDEK/Battle/BattleManager.cs` (`Defeat()`) |
| Options (PlayerPrefs, not save-slot data) | `Scripts/MARDEK/UI/Options/AudioGroupVolume.cs`, `KeyRebind.cs`, `LoadKeyRebinds.cs`, `FramerateCapOption.cs` |
