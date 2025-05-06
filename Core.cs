using MelonLoader;
using HarmonyLib;
using Il2CppGameDef;
using Il2Cpptabtoy;
using Il2Cpp;
using Il2CppSceneSettingDef;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.Events;
using Il2CppArchiveDef;

[assembly: MelonInfo(typeof(BossReforged.Core), "BossReforged", "1.0.0", "Jerry", null)]
[assembly: MelonGame("SenseGames", "AILIMIT")]

namespace BossReforged {
    public class Core : MelonMod {

        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include
        };
        public override void OnInitializeMelon() {
            LoggerInstance.Msg("Initialized.");
        }

        //[HarmonyPatch(typeof(ActorManager), "SpawnMonster")]
        //public static class SpawnMonsterPatch {
        //    private static int _spawnID = 0;

        //    public static void Prefix() {
        //        Melon<Core>.Logger.Msg($"[{Time.frameCount}]: Pre-ActorManager.SpawnMonster");
        //    }
        //    public static void Postfix(Monster __result, GameObject monsterRootGameObject, Il2CppGameDef.MonsterDefine monsterDefine, Il2CppSceneSettingDef.ActorSetting monsterSetting, Il2CppSceneSettingDef.SceneSetting sceneSetting, Il2CppSystem.Action<Il2Cpp.Monster> onBehaviorTreeLoaded) {
        //        var log = new {
        //            Time = DateTime.Now.ToString("O"),
        //            GameObjectName = monsterRootGameObject?.name,
        //            MonsterDefine = monsterDefine,
        //            ActorSetting = monsterSetting,
        //            SceneSetting = sceneSetting,
        //            CallbackInfo = onBehaviorTreeLoaded == null ? "null" : $"Target={onBehaviorTreeLoaded.Target}, Method={onBehaviorTreeLoaded.Method}"
        //        };

        //        string json = JsonConvert.SerializeObject(log, Formatting.Indented,
        //            new JsonSerializerSettings {
        //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //                NullValueHandling = NullValueHandling.Include
        //            });

        //        string dir = Path.Combine("UserData", "SpawnMonsterCallLogs", $"{__result?.gameObject.scene.name}");
        //        Directory.CreateDirectory(dir);

        //        string fileName = $"{monsterDefine.Name}_{_spawnID++}.json";
        //        //File.WriteAllText(Path.Combine(dir, fileName), json);
        //        Melon<Core>.Logger.Msg($"Post-ActorManager.SpawnMonster({monsterDefine.Name})");
        //    }
        //}

        //[HarmonyPatch(typeof(LevelLoadManager), "Load")]
        //public static class LevelLoadPatch {
        //    public static void Prefix() {
        //        Melon<Core>.Logger.Msg($"[{Time.frameCount}]: Pre-LevelLoadManager.Load");
        //    }
        //    public static void Postfix() {
        //        Melon<Core>.Logger.Msg("Post-LevelLoadManager.Load");
        //    }
        //}

        //[HarmonyPatch(typeof(InteractiveManager), "GetActorStateModelByID")]
        //public static class GetMonsterStateModelPatch {
        //    public static void Postfix(int levelID, int monsterID, ref MonsterStateModel __result) {
        //        Melon<Core>.Logger.Msg($"InteractiveManager.GetActorStateModelByID(levelID={levelID}, monsterID={monsterID})");
        //        Melon<Core>.Logger.Msg($"result={JsonConvert.SerializeObject(__result, Formatting.Indented, serializerSettings)}");
        //        //__result.IsDead = false;
        //    }

        //}

        [HarmonyPatch(typeof(BranchView), "OnEnable")]
        public static class BranchViewOnEnablePatch {
            const string BOSS_REVIVE_BTN = "BossRevive";
            public static void Postfix(BranchView __instance) {
                Transform reviveBtn = __instance.ButtonGroup.transform.Find(BOSS_REVIVE_BTN);
                reviveBtn ??= CreateButton(__instance);
                reviveBtn.GetComponentInChildren<Text>().text = BOSS_REVIVE_BTN;

                Melon<Core>.Logger.Msg($"CurTransferID={Transfer.CurTransferID}");
            }

            private static Transform CreateButton(BranchView __instance) {
                Button template = __instance.transform.GetComponentInChildren<Button>();
                if (template == null) {
                    Melon<Core>.Logger.Error("Cannot find template button under BranchView.");
                    return null;
                }

                Melon<Core>.Logger.Msg("Initializing boss revive button on first activation");

                Button newButton = GameObject.Instantiate(template, template.transform.parent);
                newButton.gameObject.name = BOSS_REVIVE_BTN;
                newButton.transform.SetAsLastSibling();
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener((UnityAction)ButtonClickHandler);
                return newButton.transform;
            }

            public static void ButtonClickHandler() {
                Melon<Core>.Logger.Msg($"{BOSS_REVIVE_BTN} Clicked.");

                if (LevelLoadManager.Instance.CurLevelRoot.TryGetMonsterInstanceIdByDefineId(1204, out int id)) {
                    Melon<Core>.Logger.Msg($"Found L32-明净骑士BOSS (instanceID={id})");

                    TransferDefine td = GlobalConfig.ConfigData.GetTransferByID(Transfer.CurTransferID);
                    var levelDict = InteractiveManager.Instance.MonsterStateDic[td.LevelID];
                    Melon<Core>.Logger.Msg($"MonsterState={JsonConvert.SerializeObject(levelDict[id], Formatting.Indented, serializerSettings)}");
                    levelDict[id].IsDead = false;
                    //LevelLoadManager.Instance.Load(td.LevelID);
                    ActorNodeManager.Instance.LoadMonster(id);
                }
            }
        }
    }
}