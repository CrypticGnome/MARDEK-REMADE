using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MARDEK.Battle;
using MARDEK.CharacterSystem;
using MARDEK.Progress;

// Ensures every battle-model prefab referenced by a CharacterProfile has the right
// BattleCharacter component on its root, with its battleModel reference wired:
// HeroBattleCharacter for profiles whose Character appears in a PartySO (on-field or
// required setup), EnemyBattleCharacter for everything else. BattleManager still adds
// the component at runtime as a fallback, but prefabs should carry it so it can be
// inspected and serialized.
public static class EnsureBattleCharacterComponents
{
     [MenuItem("Custom/Ensure Battle Character Components On Battle Models")]
     static void EnsureAll()
     {
          HashSet<CharacterProfile> heroProfiles = CollectHeroProfiles();

          // Map each battle-model prefab to hero/enemy via the profiles referencing it
          var prefabIsHero = new Dictionary<GameObject, bool>();
          foreach (string guid in AssetDatabase.FindAssets("t:CharacterProfile"))
          {
               var profile = AssetDatabase.LoadAssetAtPath<CharacterProfile>(AssetDatabase.GUIDToAssetPath(guid));
               if (profile == null || profile.BattleModelPrefab == null)
                    continue;

               bool isHero = heroProfiles.Contains(profile);
               if (prefabIsHero.TryGetValue(profile.BattleModelPrefab, out bool existing) && existing != isHero)
               {
                    Debug.LogError($"{profile.BattleModelPrefab.name} is referenced by both hero and enemy profiles - treating it as a hero. Give the two profiles separate prefabs.", profile);
                    isHero = true;
               }
               prefabIsHero[profile.BattleModelPrefab] = isHero;
          }

          int updated = 0, unchanged = 0;
          foreach (KeyValuePair<GameObject, bool> pair in prefabIsHero)
          {
               if (EnsureOnPrefab(AssetDatabase.GetAssetPath(pair.Key), pair.Value ? typeof(HeroBattleCharacter) : typeof(EnemyBattleCharacter)))
                    updated++;
               else
                    unchanged++;
          }
          Debug.Log($"Ensured battle character components: {updated} prefab(s) updated, {unchanged} already correct ({heroProfiles.Count} hero profile(s) found via PartySO assets)");
     }

     static HashSet<CharacterProfile> CollectHeroProfiles()
     {
          var heroProfiles = new HashSet<CharacterProfile>();
          foreach (string guid in AssetDatabase.FindAssets("t:PartySO"))
          {
               var party = AssetDatabase.LoadAssetAtPath<PartySO>(AssetDatabase.GUIDToAssetPath(guid));
               if (party == null)
                    continue;
               AddProfiles(party.OnFieldCharacters);
               AddProfiles(party.RequiredSetup);
          }
          return heroProfiles;

          void AddProfiles(List<Character> characters)
          {
               if (characters == null)
                    return;
               foreach (Character character in characters)
                    if (character != null && character.Profile != null)
                         heroProfiles.Add(character.Profile);
          }
     }

     // Returns true if the prefab was changed and saved.
     static bool EnsureOnPrefab(string path, System.Type wantedType)
     {
          GameObject root = PrefabUtility.LoadPrefabContents(path);
          try
          {
               var existing = root.GetComponent<BattleCharacter>();
               if (existing != null && existing.GetType() != wantedType)
               {
                    Debug.LogWarning($"{path}: replacing {existing.GetType().Name} with {wantedType.Name}");
                    Object.DestroyImmediate(existing);
                    existing = null;
               }

               var battleCharacter = existing != null ? existing : (BattleCharacter)root.AddComponent(wantedType);
               bool added = existing == null;

               bool rewired = false;
               if (battleCharacter.battleModel == null)
               {
                    battleCharacter.battleModel = root.GetComponentInChildren<BattleModelAnimator>(true);
                    rewired = battleCharacter.battleModel != null;
               }

               if (!added && !rewired)
                    return false;

               PrefabUtility.SaveAsPrefabAsset(root, path);
               Debug.Log($"{path}: {(added ? $"added {wantedType.Name}" : "rewired battleModel reference")}");
               return true;
          }
          finally
          {
               PrefabUtility.UnloadPrefabContents(root);
          }
     }
}
