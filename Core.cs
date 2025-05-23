﻿using HarmonyLib;
using Il2Cpp;
using Il2CppGameDef;
using Il2Cpptabtoy;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(BossReforged.Core), "BossReforged", "1.0.1", "Jerry", null)]
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
            MonsterOverrides = MonsterOverrideLoader.LoadOverrides();
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

        [HarmonyPatch(typeof(Config), "Deserialize", new System.Type[] { typeof(Il2CppGameDef.MonsterDropDefine), typeof(DataReader) })]
        public static class MonsterDropOverridePatch {
            static MonsterDropDefine UrsulaDrops = null;
            static MonsterDropDefine MutatedDrops = null;
            static void Postfix(ref Il2CppGameDef.MonsterDropDefine ins) {
                if (ins.ID == 260 || ins.ID == 261) {
                    if (ins.ID == 260) UrsulaDrops = ins;
                    else MutatedDrops = ins;
                    // Ursula 2nd form dropn list update
                    if (UrsulaDrops != null && MutatedDrops != null) {
                        UrsulaDrops.DropAccessoryList = MutatedDrops.DropAccessoryList;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Config), "GetMonsterByID")]
        public static class GetMonsterByIdPatch {
            static void Postfix(Config __instance, ref Il2CppGameDef.MonsterDefine __result, int ID) {
                if (MonsterFullReplaces.TryGetValue(ID, out MonsterOverrideEntry replacement)) {
                    Il2CppGameDef.MonsterDefine newMD = __instance._MonsterByID[replacement.FullReplace.Value];
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