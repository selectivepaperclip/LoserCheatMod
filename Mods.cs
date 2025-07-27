
using BepInEx;
using HarmonyLib;
using LoserCheatMod;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin(NAMESPACE, TITLE, VERSION)]
public class Mods : BaseUnityPlugin
{
    public const string NAMESPACE = "LoserCheatmod";
    public const string TITLE = "noxtek & xeph555 cheats - lolkekeke2 edit";
    public const string VERSION = "0.11.00";

    // entry point, called once from Unity due to BaseUnityPlugin (MonoBehaviour)
    // Mind the split between awake and start, awake should only handle instance creation and gameObject relevant stuff
    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        gameObject.AddComponent<ModsGUI>();

        var harmony = new Harmony(NAMESPACE);
        harmony.PatchAll();
    }

    public static Mods Instance;
    private Mods() { }

    public const int FastForwardMultiplier = 5;
    private bool fastForwardHeld;


    // Start is called once from Unity before first frame and after all awakes are handled, due to BaseUnityPlugin (MonoBehaviour),
    private void Start()
    {
        ModConfig.Instance.ConfigBindings(Instance.Config);
    }

    // Update is called for each frame from Unity due to BaseUnityPlugin (MonoBehaviour)
    private void Update()
    {
        // modify timescale even if cheatmod is not currently usable
        fastForwardHeld = Input.GetKey(ModConfig.Instance.GetFastForwardKey());
        Time.timeScale = fastForwardHeld ? ModConfig.Instance.GetTimeMultiplier() * FastForwardMultiplier : ModConfig.Instance.GetTimeMultiplier();

        if (Instance.CheatModBlocked())
            return;

        HandleKeys();
        Effects();
    }

    // handle persistant effects like infinite stats, etc.
    private void Effects()
    {
        if (ModConfig.Instance.IsToggleInfiniteStats())
            MaxPlayerStats();
        if (ModConfig.Instance.IsToggleInfiniteArousal())
            MaxPlayerArousal();
        if (ModConfig.Instance.IsToggleResetLifestyleCD())
            ResetLifestyleCD();
        if (ModConfig.Instance.IsToggleInfiniteCleanliness())
            CleanAllRooms();
        if (ModConfig.Instance.IsToggleInfiniteActions())
            ResetActions();
    }

    // handle all keypresses here
    private void HandleKeys()
    {
        if (Input.GetKeyDown(ModConfig.Instance.GetToggleInfiniteStatsKey()))
            ModConfig.Instance.ToggleInfiniteStats();
        if (Input.GetKeyDown(ModConfig.Instance.GetToggleInfiniteArousalKey()))
            ModConfig.Instance.ToggleInfiniteArousal();
        if (Input.GetKeyDown(ModConfig.Instance.GetToggleResetLifestyleCDKey()))
            ModConfig.Instance.ToggleResetLifestyleCD();
        if (Input.GetKeyDown(ModConfig.Instance.GetToggleInfiniteCleanlinessKey()))
            ModConfig.Instance.ToggleInfiniteCleanliness();
        if (Input.GetKeyDown(ModConfig.Instance.GetToggleInfiniteTopicsKey()))
            ModConfig.Instance.ToggleInfiniteTopics();
        if (Input.GetKeyDown(ModConfig.Instance.GetToggleInfiniteActionsKey()))
            ModConfig.Instance.ToggleInfiniteActions();

        if (Input.GetKeyDown(ModConfig.Instance.GetTimeMinusKey()))
            SkipHours(-1);
        if (Input.GetKeyDown(ModConfig.Instance.GetTimePlusKey()))
            SkipHours(1);
        if (Input.GetKeyDown(ModConfig.Instance.GetSkip24HoursKey()))
            SkipHours(24);

        if (Input.GetKeyDown(ModConfig.Instance.GetResetActionsKey()))
        {
            ResetActions();
            RefreshButtons();
        }

        if (Input.GetKeyDown(ModConfig.Instance.GetMaxPlayerStatsKey()))
        {
            MaxPlayerStats();
            RefreshButtons();
        }
    }

    public List<Skill> GetPlayerSkills() => player.ins.playerSkills.Skills.OrderBy(skill => skill.Moniker).ToList();
    public List<dictionary_Tag> GetAllDictionaryTags() => Inventory.ins.TagsList.OrderBy(tag => tag.associated_TAG.name).ToList();
    public List<topics> GetAllTopics() => Inventory.ins.Master_TopicList.OrderBy(topic => topic.Name).ToList();
    public List<tags> GetRelevantCharacterSkills(character_Script character)
    {
        // multiple interests have different tags (but we only actually care about the relevant xp)
        Dictionary<xpSO, tags> tagsByXP = [];
        foreach (string atName in character.Archtype.possibleATs)
        {
            if (atName != "none")
            {
                ArcheType at = archetypes.ins.GetArchetypeByName(atName);
                foreach (interestSO interest in at.interests)
                    foreach (tags tag in interest.associatedtags)
                        if (character.preferences[tag.associatedXP.Moniker] != 100)
                            tagsByXP[tag.associatedXP] = tag;
            }
        }
        // distinct tags
        return tagsByXP.Values.OrderBy(tag => tag.associatedXP.Moniker).ToList();
    }

    public void MaxPlayerStats()
    {
        AddPlayerFocus(player.ins.getFocusMax());
        AddPlayerHealth(player.ins.getHealthMax());
        AddPlayerStamina(player.ins.getStaminaMax());
    }
    // set player arousal to below max, as to not end sex minigame instantly
    public void MaxPlayerArousal() => AddPlayerArousal(player.ins.getArousalMax() - 1 - player.ins.getArousal());

    public void AddPlayerFocus(int amount) => player.ins.addFocus(amount);
    public void AddPlayerStamina(int amount) => player.ins.addStamina(amount);
    public void AddPlayerHealth(int amount) => player.ins.addHealth(amount);
    public void AddPlayerArousal(int amount) => player.ins.addArousal(amount);

    public void AddMoneyToPlayer(int money) => player.ins.addMoney(money);

    public void AddAllPlayerSkills(int amount) => GetPlayerSkills().ForEach(skill => AddPlayerSkill(skill, amount));
    public void AddPlayerSkill(Skill skill, int amount) => player.ins.playerSkills.gainPassiveXP(skill, amount);

    // limit exp gain to prevent 2 rank ups at once (game logic does not support it)
    public int ExperienceGainMod(int amount, int modifier) => Mathf.Clamp(amount * modifier, 0, 50);

    public void MaxCharacterExp(character_Script character)
    {
        foreach (tags tag in GetRelevantCharacterSkills(character))
            AddCharacterStat(character, CharStatList.Preference, 999, tag: tag, silent: true);
    }

    public void AddCharacterStat(character_Script character, CharStatList stat, int amount = 0, tags tag = null, interestSO interest = null, bool silent = false)
    {
        CharacterStatModifications CSM = new()
        {
            character = character.Assigned,
            Stat = stat,
            Amount = amount
        };
        if (interest != null)
            CSM._String = interest.Moniker;
        if (tag != null)
            CSM.tag = tag;

        if (silent)
            CSM.modifySilent();
        else
            CSM.modify();

        if (CharStatList.interest1 == CSM.Stat || CharStatList.interest2 == CSM.Stat)
            character.updateIntrestedTags();
    }

    public void SetCharacterFlag(character_Script character, CharFlagList flag, bool value, bool silent = false)
    {
        // massaged flag doesnt play nice with CSM.modify()
        if (CharFlagList.massaged == flag && character.massaged && !value)
        {
            character.massaged = value;
        }
        else
        {
            CharacterStatModifications CSM = new()
            {
                character = character.Assigned,
                Flag = flag,
                Toggle = value
            };

            if (silent)
                CSM.modifySilent();
            else
                CSM.modify();
        }
    }

    public void MaxCharacterStats(character_Script character, int value = 99999)
    {
        AddCharacterStat(character, CharStatList.relationship, value, silent: true);
        AddCharacterStat(character, CharStatList.lust, value, silent: true);
        AddCharacterStat(character, CharStatList.interest, value, silent: true);
        AddCharacterStat(character, CharStatList.intoxication, value, silent: true);
        AddCharacterStat(character, CharStatList.followers, value, silent: true);
        // make a dominant character more dominant, and submissive more submissive. 0 is currently treated as submissive
        AddCharacterStat(character, CharStatList.dominance, character.getStat(CharStatList.dominance) > 0 ? value : -value, silent: true);
    }

    public void TransformCharacter(character_Script character, string AT)
    {
        character.loadedAT = AT;
        character.ArchetypeTransformation();
        //gamemanager.ins.updateCharacters(); not needed anymore
    }

    public void GainTopic(topics topic) => player.ins.gainItem(topic);

    public void GetRandomTempTopicWithTag(dictionary_Tag tag)
    {
        List<topics> topics = tag.TopicsWithTag.FindAll(t => t.isTemp());
        if (topics.IsNullOrEmpty())
            return;
        System.Random rnd = new();
        GainTopic(topics.ElementAt(rnd.Next(0, topics.Count - 1)));
    }

    public void ResetLifestyleCD() => Enumerable.Range(0, 99).ForEach(n => player.ins.updateLifeStyleCDs());

    public void SkipHours(int hourstoadd)
    {
        // if time is 0, and user goes back in time, need to wrap around to previous day
        // but in game logic, days can only go forward, so we do a bit of trickery
        if (Scheduler.ins.time == 0 && hourstoadd == -1)
        {
            // set the time to the maximum hour of the previous day
            Scheduler.ins.time = 23;
            int numberOfDays = Enum.GetValues(Scheduler.ins.weekday.GetType()).Length; // = 7
            // go back 1 day
            int previousDay = (Array.IndexOf(Enum.GetValues(Scheduler.ins.weekday.GetType()), Scheduler.ins.weekday) - 1 + numberOfDays) % numberOfDays;
            Scheduler.ins.weekday = (weekday_list)Enum.GetValues(Scheduler.ins.weekday.GetType()).GetValue(previousDay);
        }
        // implement going back in time as setting time to time - 2, then going forward 1 hour (game logic doesnt support time going back as actual negative value)
        else if (hourstoadd == -1)
        {
            hourstoadd = 1;
            Scheduler.ins.time -= 2;
        }
        // needs to be async (IEnumerator) due to game logic
        gamemanager.ins.StartEnumeration(SkipHoursDelayed(hourstoadd));
    }

    // asked the dev, and this is how time passing should be handled:
    private IEnumerator SkipHoursDelayed(int hourstoadd)
    {
        Scheduler.ins.TimeAdvancing = true;
        Scheduler.ins.waitingForAdvance();
        yield return new WaitForSeconds(0.1f);
        AdvanceTime(hourstoadd);
        while (Scheduler.QueuedScheduleCheck) { yield return null; }
        RefreshButtons(true);
    }
    // own method so that IsCalledFromCheatMod can be used
    private void AdvanceTime(int hourstoadd) => Scheduler.ins.advanceTime(hourstoadd);

    public bool AddRoomCleanliness(Location_Script loc, int amt = 999)
    {
        if (loc != null && loc.cleanable)
        {
            loc.clean(amt);
            return true;
        }
        return false;
    }

    public void CleanAllRooms() => gamemanager.ins.LM.locations.Values.ForEach(loc => AddRoomCleanliness(loc));

    public void ResetActions()
    {
        ResetPlayerFlags();
        gamemanager.ins.LM.locations.Values.ForEach(loc => ResetLocation(loc));
        characters.ins.Character_List.ForEach(character => ResetNpcCharacterFlags(character));
    }

    public void ResetPlayerFlags()
    {
        player.ins.DailyResets();
        // remember and restore hasCarKey flag after resetSleep
        bool hasCarKey = player.ins.getHasCarKey();
        player.ins.resetSleep();
        player.ins.setHasCarKey(hasCarKey);
        // weekly reset
        player.ins.setHasYardwork(Bool: false);
    }

    public void ResetLocation(Location_Script loc)
    {
        loc.DailyReset();
        InvokePrivateMember(loc, "WeeklyReset");
    }

    public void ResetNpcCharacterFlags(character_Script character)
    {
        // character.dailyReset clears too many stats and flags, character.weeklyReset clears hacked profiles
        //character.DailyReset();
        //character.WeeklyReset();
        foreach (CharFlagList flag in GetRelevantCharacterFlags())
            SetCharacterFlag(character, flag, false, silent: true);
    }

    public List<CharFlagList> GetRelevantCharacterFlags() => [CharFlagList.hasModeled, CharFlagList.massaged, CharFlagList.hadSex, CharFlagList.dailyFlag1, CharFlagList.dailyFlag2, CharFlagList.Invited];

    // sometimes, due to bugs in the game, a scene might get stuck. added this as an emergency escape to player room
    public void Bailout()
    {
        player.ins.setLocation(Location_list.Your_Room);
        Scheduler.ins.Relocating = false;
        RefreshScene();
    }

    public List<Location_Script> GetLockedLocations()
    {
        // filter out job locations
        List<Location_Script> hiddenLocations = gamemanager.ins.LM.locations.Values.Where(l => l.hiddenLocation && !JobManager.ins.jobs.Select(j => j.AssignedLocation).Contains(l.assignedLocale)).ToList();
        hiddenLocations.Sort((l1, l2) => l1.LocaleName.CompareTo(l2.LocaleName));
        return hiddenLocations;
    }

    public void UnlockLocation(Location_Script loc)
    {
        gamemanager.ins.unHideLocation(loc.assignedLocale);
    }

    public void RefreshButtons(bool fadeToBlack = false)
    {
        if (CanRefreshScene())
            gamemanager.ins.reloadLoacation(fadeToBlack);
    }
    public void RefreshScene()
    {
        if (CanRefreshScene())
            SkipHours(0);
    }

    // returns true when cheatmod shouldn't be doing anything, ie. during main menu, loading etc.
    public bool CheatModBlocked()
    {
        // good enough
        return !gamemanager.ins.HotkeysEnabled();
    }

    // returns true when scene can be refreshed
    // scenes that shouldn't be refreshed are ones which turn off after refresh ie. sex minigame, dialogue, selecting interests etc.
    public bool CanRefreshScene()
    {
        List<string> blockingLocations = (List<string>)InvokePrivateMember(SceneController.ins, "exclusionScenes");
        return !UI_Manager.ins.SexActive && !blockingLocations.Contains(SceneManager.GetActiveScene().name);
    }

    public object InvokePrivateMember(object o, string methodName, params object[] parameters)
    {
        return o.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty, null, o, parameters);
    }

    public static bool IsCalledFromCheatMod(params string[] methodNames)
    {
        // Check if any method in callstack belongs to cheatmod
        return Array.Exists(new StackTrace().GetFrames(), frame => frame.GetMethod().DeclaringType == typeof(Mods) && methodNames.Contains(frame.GetMethod().Name));
    }

    internal class GamePatches
    {
        [HarmonyPatch(typeof(player), nameof(player.GetInventoryBonus))]
        class PlayerInventoryLimit
        {
            // 50 is baseline limit, this gets added to it (just for a nicer number)
            static void Postfix(ref int __result) => __result = ModConfig.Instance.IsToggleInfiniteTopics() ? 99999 - 50 : __result;
        }

        [HarmonyPatch]
        class XpMultiplierNPC
        {
            static IEnumerable<MethodBase> TargetMethods() =>
            [
                AccessTools.Method(typeof(character_Script), nameof(character_Script.gainPreference), [typeof(tags), typeof(int), typeof(bool)]),
                AccessTools.Method(typeof(character_Script), nameof(character_Script.gainPreferenceSilent), [typeof(tags), typeof(int), typeof(bool)]),
            ];
            static void Prefix(ref int num) => num = Mods.Instance.ExperienceGainMod(num, ModConfig.Instance.GetNpcXpMultiplier());
        }

        [HarmonyPatch]
        class XpMultiplierPlayer
        {
            static IEnumerable<MethodBase> TargetMethods() =>
            [
                AccessTools.Method(typeof(PlayerSkills), nameof(PlayerSkills.GainXP), [typeof(Skill), typeof(int), typeof(bool)]),
                AccessTools.Method(typeof(PlayerSkills), nameof(PlayerSkills.gainPassiveXP), [typeof(Skill), typeof(int)]),
            ];
            static void Prefix(ref int amount) => amount = Mods.Instance.ExperienceGainMod(amount, ModConfig.Instance.GetPlayerXpMultiplier());
        }
    }

    [HarmonyPatch]
    internal class SkipMethodPatches
    {
        // skip these method invocations, if called from cheatmod
        static IEnumerable<MethodBase> TargetMethods() =>
        [
            AccessTools.Method(typeof(player), nameof(player.setDailyInvestmentGrowth)),
            AccessTools.Method(typeof(player), nameof(player.checkRep)),
            AccessTools.Method(typeof(player), nameof(player.updateLifeStyleCDs)),
            AccessTools.Method(typeof(player), nameof(player.processDailyFollowers)),
            AccessTools.Method(typeof(player), nameof(player.resetBonusAPP)),
            AccessTools.Method(typeof(player), nameof(player.gainPassiveXP)),
            AccessTools.Method(typeof(Location_Script), "checkUpgrades"),
            AccessTools.Method(typeof(Location_Script), "degradeLocation"),
            AccessTools.Method(typeof(CoroutineQueue), nameof(CoroutineQueue.EnqueueAction)),
        ];

        static bool Prefix() => !IsCalledFromCheatMod(nameof(ResetPlayerFlags), nameof(ResetLocation), nameof(AdvanceTime), nameof(MaxPlayerStats), nameof(MaxPlayerArousal));
    }

}