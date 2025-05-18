# AI LIMIT Boss Reforged

A utility mod for AI LIMIT that allows you to revive and rebattle bosses directly from branch locations (campfires).

Also supports optional boss stat overrides, such as HP and base damage.

## ⚠️ Save File Warning

> **Always back up your save file before enabling or modifying any mod.**

## 📦 Installation

1. Install MelonLoader.

2. Drop the `BossReforged.dll` into your `Mods` folder.

3. (Optional) Place `MonsterOverrides.json/toml` in the `UserData/BossReforged/` directory.

## 📝 Usage

1. Travel to a branch (campfire).

2. If the branch corresponds to one or more defeated bosses, a **Re:BossName** button will appear.

3. Click the Boss Revive button at a branch to resurrect defeated boss(es).

    ✅ Most bosses respawn instantly.

    ⏳ Some may require a scene reload — a brief black screen is normal. Please wait a few seconds for the reload to complete.

4. If configured, modified stats will be applied automatically.

## ⚠️ Known Limitations

1. Some stateful/NPC-like bosses might be in a weird state after revived using this mod
    a. Eunomia, the Resplendent Bishop will start in her final form

2. Some post-battle events won't trigger

### 🛠 Workaround Applied:
For Ursula's second form, the mod manually drops the Mutant Blader Nucleus along with her outfit upon defeat, since the normal post-boss sequence does not trigger on rematches.

## 📁 Config (Optional)

To override boss stats, create a JSON config in the following format:

`UserData/BossReforged/MonsterOverrides.json`

```json
{
    "50007": {
        "Name": "L24-剧情-亚德米勒",
        "HP": 10000,
        "BaseDamage": 100
    },
    "99998": {
        "Name": "沙包机器人",
        "HP": 10000,
        "FullReplace": 50007
    }
}
```

* **Key** is MonsterDefine.ID of the boss
* **Name** is used only as a label in the config for clarity (not in-game).
* **HP** and **BaseDamage** are optional overrides
* **FullReplace** is used to fully replace one type of monster with another

Alternatively, Create a TOML file:

`UserData/BossReforged/MonsterOverrides.toml`

```toml
# L24-剧情-亚德米勒
[50007]
HP = 10000
BaseDamage = 100

# 沙包机器人
[99998]
HP = 10000
FullReplace = 50007
```

* **Section header** ([50007]) is the MonsterDefine.ID.
* **Comments** (# ...) can be used for labeling; do not use a **Name** field.
* Fields are the same as JSON: **HP**, **BaseDamage**, **FullReplace**.

See the [monster data dump](GameDataDumps/Monster.json) for monster IDs and original values.
