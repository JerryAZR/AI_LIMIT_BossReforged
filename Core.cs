using HarmonyLib;
using Il2Cpp;
using Il2CppGameDef;
using Il2Cpptabtoy;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(BossReforged.Core), "BossReforged", "1.0.0", "Jerry", null)]
[assembly: MelonGame("SenseGames", "AILIMIT")]

namespace BossReforged {
    public class Core : MelonMod {
        public static Dictionary<int, MonsterOverrideEntry> MonsterOverrides;
        public static Dictionary<int, MonsterOverrideEntry> MonsterFullReplaces;

        //static JsonSerializerSettings serializerSettings = new JsonSerializerSettings {
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //    NullValueHandling = NullValueHandling.Include
        //};

        public override void OnInitializeMelon() {
            MonsterOverrides = MonsterOverrideLoader.LoadOverrides("UserData/BossReforged/MonsterOverrides.json");
            MonsterFullReplaces = new();
            LoggerInstance.Msg("Initialized.");
        }

        [HarmonyPatch(typeof(BranchView), "OnEnable")]
        public static class BranchViewOnEnablePatch {
            public static void Postfix(BranchView __instance) {
                Melon<Core>.Logger.Msg($"CurTransferID={Transfer.CurTransferID}");
                BossReviveButton reviveBtn = __instance.ButtonGroup.GetComponentInChildren<BossReviveButton>(includeInactive: true);
                reviveBtn ??= CreateButton(__instance);
                reviveBtn.SetBranch(Transfer.CurTransferID);
                Melon<Core>.Logger.Msg($"Button updated.");
            }

            private static BossReviveButton CreateButton(BranchView __instance) {
                Button template = __instance.transform.GetComponentInChildren<Button>();
                if (template == null) {
                    Melon<Core>.Logger.Error("Cannot find template button under BranchView.");
                    return null;
                }

                Melon<Core>.Logger.Msg("Initializing boss revive button on first activation");

                Button newButton = GameObject.Instantiate(template, template.transform.parent);
                BossReviveButton reviveBtn = newButton.gameObject.AddComponent<BossReviveButton>();
                reviveBtn.Button = newButton;
                reviveBtn.transform.SetAsLastSibling();
                return reviveBtn;
            }
        }

        [HarmonyPatch(typeof(Config), "Deserialize", new System.Type[] { typeof(Il2CppGameDef.MonsterDefine), typeof(DataReader) })]
        public static class MonsterOverridePatch {
            static void Postfix(ref Il2CppGameDef.MonsterDefine ins) {
                if (MonsterOverrides.TryGetValue(ins.ID, out MonsterOverrideEntry entry)) {
                    if (entry.FullReplace.HasValue) {
                        MonsterFullReplaces[ins.ID] = entry;
                    } else {
                        if (entry.HP.HasValue) {
                            Melon<Core>.Logger.Msg($"Override {ins.Name} HP {ins.nHP} -> {entry.HP.Value}");
                            ins.nHP = entry.HP.Value;
                        }
                        if (entry.BaseDamage.HasValue) {
                            Melon<Core>.Logger.Msg($"Override {ins.Name} base damage {ins.nBasePhysicsDamage} -> {entry.BaseDamage.Value}");
                            ins.nBasePhysicsDamage = entry.BaseDamage.Value;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Config), "GetMonsterByID")]
        public static class GetMonsterByIdPatch {
            static void Postfix(Config __instance, ref Il2CppGameDef.MonsterDefine __result, int ID) {
                if (MonsterFullReplaces.TryGetValue(ID, out MonsterOverrideEntry replacement)) {
                    if (replacement.FullReplace.Value == ID) return; // Prevent infinite recursion 
                    Il2CppGameDef.MonsterDefine newMD = __instance.GetMonsterByID(replacement.FullReplace.Value);
                    __result = newMD.MemberwiseClone().Cast<Il2CppGameDef.MonsterDefine>();

                    __result.ID = ID;
                    __result.NPCMonster = false;
                    if (replacement.HP.HasValue) {
                        __result.nHP = replacement.HP.Value;
                    }
                    if (replacement.BaseDamage.HasValue) {
                        __result.nBasePhysicsDamage = replacement.BaseDamage.Value;
                    }
                }
            }

        }

        //[HarmonyPatch(typeof(InteractiveManager), "GetInteractiveStateModelByID")]
        //public static class InteractiveStatePatch {
        //    static void Postfix(InteractiveStateModel __result, int id) {
        //        Melon<Core>.Logger.Msg($"GetInteractiveStateModelByID({id}) ->");
        //        Melon<Core>.Logger.Msg(JsonConvert.SerializeObject(__result));
        //    }
        //}
    }
}