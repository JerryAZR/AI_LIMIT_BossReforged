using MelonLoader;
using System.Text.Json;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace BossReforged {
    public class MonsterOverrideEntry {
        public int? HP { get; set; }
        public int? BaseDamage { get; set; }
        public int? FullReplace { get; set; }
    }

    public class MonsterOverrideLoader {
        public static Dictionary<int, MonsterOverrideEntry> LoadOverrides() {
            Dictionary<int, MonsterOverrideEntry> overrides = new();
            LoadOverridesJson("UserData/BossReforged/MonsterOverrides.json", overrides);
            LoadOverridesToml("UserData/BossReforged/MonsterOverrides.toml", overrides);
            MelonLogger.Msg($"Loaded {overrides.Count} Monster overrides.");
            return overrides;
        }

        public static void LoadOverridesJson(string path, Dictionary<int, MonsterOverrideEntry> overrides) {
            try {
                if (!File.Exists(path)) {
                    MelonLogger.Msg($"Override file not found: {path}");
                    return;
                }

                var json = File.ReadAllText(path);
                var raw = JsonSerializer.Deserialize<Dictionary<string, MonsterOverrideEntry>>(json);

                foreach (var kvp in raw) {
                    if (int.TryParse(kvp.Key, out int monsterID)) {
                        overrides[monsterID] = kvp.Value;
                    } else {
                        MelonLogger.Warning($"Unknown key type: {kvp.Key}");
                    }
                }
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to load Monster overrides from json: {ex}");
            }
        }

        public static void LoadOverridesToml(string path, Dictionary<int, MonsterOverrideEntry> overrides) {
            try {
                if (!File.Exists(path)) {
                    MelonLogger.Msg($"Override file not found: {path}");
                    return;
                }

                TomlDocument doc = TomlParser.ParseFile(path);

                foreach (var kvp in doc) {
                    if (int.TryParse(kvp.Key, out int id)) {
                        try {
                            MonsterOverrideEntry entry = TomletMain.To<MonsterOverrideEntry>(kvp.Value);
                            overrides[id] = entry;
                        } catch (TomlException tomlEx) {
                            MelonLogger.Warning($"Exception while parsing {kvp.Value}: {tomlEx}");
                        }
                    } else {
                        MelonLogger.Error($"[MonsterOverrideEntry] Invalid ID: {kvp.Key}");
                    }
                }
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to load Monster overrides from toml: {ex}");
            }
        }
    }
}
