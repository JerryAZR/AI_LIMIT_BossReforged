using MelonLoader;
using System.Text.Json;

namespace BossReforged {
    public class MonsterOverrideEntry {
        public int? HP { get; set; }
        public int? BaseDamage { get; set; }
        public int? FullReplace { get; set; }
    }

    public class MonsterOverrideLoader {
        public static Dictionary<int, MonsterOverrideEntry> LoadOverrides(string path) {
            Dictionary<int, MonsterOverrideEntry> overrides = new();
            try {
                if (!File.Exists(path)) {
                    MelonLogger.Msg($"Override file not found: {path}");
                    return overrides;
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

                MelonLogger.Msg($"Loaded {overrides.Count} Monster overrides.");
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to load Monster overrides: {ex}");
            }

            return overrides;
        }
    }
}
