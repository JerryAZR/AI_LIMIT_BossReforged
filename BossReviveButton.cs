using Il2Cpp;
using Il2CppArchiveDef;
using Il2CppGameDef;
using MelonLoader;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BossReforged {
    [RegisterTypeInIl2Cpp]
    internal class BossReviveButton : MonoBehaviour {
        const string COMBAT_ICON = "Re:";
        const string BOSS_REVIVE_BTN = "BossRevive";

        private class BossReviveInfo {
            public int ID;
        }

        private class BranchReviveInfo {
            public List<BossReviveInfo> AllBosses;
            public int? AltNameID;
            public bool ListAll = false;
            public bool Reload = false;

            public string GetName() {
                try {
                    if (AltNameID.HasValue) {
                        return LocalizationManager.Instance.GetCurLocal(AltNameID.Value);
                    } else if (AllBosses.Count > 0) {
                        if (ListAll) {
                            StringBuilder builder = new();
                            foreach (BossReviveInfo boss in AllBosses) {
                                if (boss != AllBosses.First()) {
                                    builder.Append('&');
                                }
                                Il2CppGameDef.MonsterDefine md = GlobalConfig.ConfigData.GetMonsterByID(boss.ID);
                                builder.Append(LocalizationManager.Instance.GetCurLocal(md.NameId));
                            }
                            return builder.ToString();
                        } else {
                            Il2CppGameDef.MonsterDefine md = GlobalConfig.ConfigData.GetMonsterByID(AllBosses.First().ID);
                            return LocalizationManager.Instance.GetCurLocal(md.NameId);
                        }
                        
                    }
                } catch {
                    Melon<Core>.Logger.Error("Cannot find localized boss name");
                }
                return "Boss";
            }
        }

        private static readonly Dictionary<int, BranchReviveInfo> BossReviveBranches = new() {
            {
                1508, // 大蓄水池
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 1202 },   // L1-迷失的枪兵 罗尔
                        new BossReviveInfo { ID = 120202 }, // L1-罗尔二阶段
                    },
                    Reload = true
                }
            },
            {
                2505, // 废屋
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 20208 },   // L2-拾荒者大族长
                    }
                }
            },
            {
                2506, // 墓园
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 1203 },   // L2-恐慌收割者
                    },
                    Reload = true
                }
            },
            {
                2517, // 研究所
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 120501 },
                        new BossReviveInfo { ID = 120502 },
                    },
                    Reload = true
                }
            },
            {
                3506, // 旅店走廊
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 90402 },   // L3-高级教会守卫-镰刀BOSS
                    }
                }
            },
            {
                3515, // 乙金大厦
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 1204 }
                    }
                }
            },
            //{
            //    2518, // 地下试验场
            //    new BranchReviveInfo {
            //        AllBosses = new List<BossReviveInfo> {
            //            new BossReviveInfo { ID = 50005, DisableNPC = true }
            //        },
            //        AltNameID = 1012039
            //    }
            //},
            {
                4504, // 壁画前
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 90405 },  // L4-圣者BOSS
                    }
                }
            },
            {
                4512, // 教区车站
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 90407 },  // L42-辉翼圣者BOSS
                        new BossReviveInfo { ID = 1213 },   // L42-地海徘徊者BOSS
                    },
                    ListAll = true
                }
            },
            {
                4514, // 审判厅
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 121201 },  // L42-欧诺弥亚BOSS
                        new BossReviveInfo { ID = 121202 },  // L42-狄刻BOSS
                        new BossReviveInfo { ID = 121204 },  // L42-欧诺弥亚三阶段
                    },
                    Reload = true
                }
            },
            {
                5502, // 净灵阶梯
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 1214 },   // L51-BOSS-四手
                    },
                    Reload = true
                }
            },
            {
                5506, // 献晶所
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 95461 },  // L52-教会守卫-棍子-翅膀
                    }
                }
            },
            {
                5513, // 未然领域
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 96404 },  // L52-教会守卫-壮汉-翅膀(连战1)
                        new BossReviveInfo { ID = 96405 },  // L52-圣者(连战2)
                        new BossReviveInfo { ID = 95403 },  // L52-高级教会守卫-四手(连战3)
                    }
                }
            },
            {
                5521, // 至上天顶
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 1206 },   // L54-Boss-洛希德
                        new BossReviveInfo { ID = 120602 }, // L54-Boss-洛希德二阶段
                    },
                    Reload = true
                }
            },
            {
                6509, // 狩猎场
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 1205 },  // L6-厄休拉-蒙面BOSS(不复活)
                    }
                }
            },
            {
                7506, // 灵龛底层
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 120701 },  // L7-深渊哀后BOSS
                        new BossReviveInfo { ID = 120702 },  // L7-深渊哀后二阶段BOSS
                    },
                    Reload = true
                }
            },
            {
                8504, // 圣树之门
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 120801 },  // L8-圣树守卫_直剑_BOSS
                        new BossReviveInfo { ID = 120802 },  // L8-圣树守卫_双刀_BOSS
                        new BossReviveInfo { ID = 120803 },  // L8-圣树守卫_长刀_BOSS
                        new BossReviveInfo { ID = 120804 },  // L8-圣树守卫_圣树长枪_BOSS
                    }
                }
            },
            {
                9023, // 死树林
                new BranchReviveInfo {
                    AllBosses = new List<BossReviveInfo> {
                        new BossReviveInfo { ID = 121101 },  // L10-狩猎小队-队长BOSS
                        new BossReviveInfo { ID = 121102 },  // L10-狩猎小队-胖子BOSS
                        new BossReviveInfo { ID = 121103 },  // L10-狩猎小队-远程BOSS
                        new BossReviveInfo { ID = 121104 },  // L10-狩猎小队-陷阱BOSS
                    }
                }
            },
        };


        private Button _button;
        public Button Button {
            get { return _button; }
            set {
                _button = value;
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener((UnityAction)ButtonClickHandler);
                gameObject.name = BOSS_REVIVE_BTN;
            }
        }

        private readonly List<Il2CppGameDef.MonsterDefine> Bosses = new();
        private BranchReviveInfo CurrReviveInfo;

        public void SetBranch(int branchID) {
            Bosses.Clear();
            bool needButton = false;
            if (BossReviveBranches.TryGetValue(branchID, out CurrReviveInfo)) {
                TransferDefine td = GlobalConfig.ConfigData.GetTransferByID(Transfer.CurTransferID);
                foreach (BossReviveInfo boss in CurrReviveInfo.AllBosses) {
                    if (LevelLoadManager.Instance.CurLevelRoot.TryGetMonsterInstanceIdByDefineId(boss.ID, out int id)) {
                        MonsterStateModel state = InteractiveManager.Instance.GetActorStateModelByID(td.LevelID, id);
                        if (state != null && state.IsDead) {
                            needButton = true;
                        }
                    }                    
                }
                gameObject.SetActive(needButton);
                GetComponentInChildren<Text>().text = $"{COMBAT_ICON} {CurrReviveInfo.GetName()}";
            } else {
                // No boss associated with this branch. Just disable the button.
                gameObject.SetActive(false);
            }
        }

        public void ButtonClickHandler() {
            Melon<Core>.Logger.Msg($"{BOSS_REVIVE_BTN} Clicked.");
            gameObject.SetActive(false);
            Button nextSelect = transform.parent.GetComponentInChildren<Button>();
            nextSelect.Select();

            TransferDefine td = GlobalConfig.ConfigData.GetTransferByID(Transfer.CurTransferID);

            foreach (BossReviveInfo boss in CurrReviveInfo.AllBosses) {
                Il2CppGameDef.MonsterDefine md = GlobalConfig.ConfigData.GetMonsterByID(boss.ID);
                if (LevelLoadManager.Instance.CurLevelRoot.TryGetMonsterInstanceIdByDefineId(boss.ID, out int id)) {
                    Melon<Core>.Logger.Msg($"Found {md.Name} (instanceID={id}) near {td.Name}");
                    var levelDict = InteractiveManager.Instance.MonsterStateDic[td.LevelID];
                    //Melon<Core>.Logger.Msg(JsonConvert.SerializeObject(levelDict[id]));
                    levelDict[id].IsDead = false;
                    if (CurrReviveInfo.Reload == false) ActorNodeManager.Instance.LoadMonster(id);
                } else {
                    Melon<Core>.Logger.Warning($"Cannot find instance of boss {md?.Name}");
                }
                if (CurrReviveInfo.Reload) {
                    //System.Action HideLoadingAction = ViewManager.Instance.HideLastView;
                    //System.Action<UIBehaviour> LoadLevelAction = (_) => LevelLoadManager.Instance.Load(td.LevelID, HideLoadingAction);
                    //ViewManager.Instance.Show(UIView.LoadingView, callBack: LoadLevelAction);
                    try {
                        BlackScreen.Instance.ShowBlackScreenAndDoAction((System.Action)(() => LevelLoadManager.Instance.Load(
                            td.LevelID,
                            (System.Action)(() => BlackScreen.Instance.HideBlackScreen(true))
                        )));
                    } catch (Exception ex) {
                        LevelLoadManager.Instance.Load(td.LevelID);
                        Melon<Core>.Logger.Warning($"Error when trying to show loading view: {ex}");
                    }
                }
            }
            //LevelLoadManager.Instance.CurLevelRoot.ResetNPC(false);
            //LevelLoadManager.Instance.CurLevelRoot.ResetMonster();
        }
    }
}
