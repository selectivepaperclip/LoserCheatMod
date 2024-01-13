
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LoserCheatMod;
using UnityEngine;
using Object = UnityEngine.Object;

[BepInPlugin("cheatmod.loser", "noxtek & xeph555 cheats - lolkekeke2 edit", "0.10.02")]
public class Mods : BaseUnityPlugin
{
    private readonly Harmony harmony = new Harmony("cheatmod.loser");

    public void Awake()
    {

        this.harmony.PatchAll();

        toggleInfStats = Config.Bind("Toggles",                                                         // The section under which the option is shown
                                    "ToggleInfiniteStats",                                              // The key of the configuration option in the configuration file
                                    false,                                                              // The default value
                                    "Toggles infinite stats (focus, stamina, health set to max)");      // Description of the option to show in the config file
        toggleInfArousal = Config.Bind("Toggles",
                                    "ToggleInfiniteArousal",
                                    false,
                                    "Sets arousal to half of max");
        toggleResetLifestyleCD = Config.Bind("Toggles",
                                    "ToggleResetLifestyleCD",
                                    false,
                                    "Removes the cooldown of changing lifestyles");
        toggleInfCleanliness = Config.Bind("Toggles",
                                    "ToggleInfCleanliness",
                                    false,
                                    "Keeps all room at 100% clean");
        toggleSkipTimeReloadLocation = Config.Bind("Toggles",
                                    "ToggleSkipTimeReloadLocation",
                                    true,
                                    "Recalculates character locations and schedules after skipping time");
        toggleInfTopics = Config.Bind("Toggles",
                                    "ToggleInfTopics",
                                    false,
                                    "Sets the limit of topics in invetory to 99999");
        toggleKeepPermTopic = Config.Bind("Toggles",
                                    "ToggleKeepPermTopic",
                                    false,
                                    "Toggle if permanent topics should be removed from hotbar when used in conversations");
        toggleInfActions = Config.Bind("Toggles",
                                   "ToggleInfActions",
                                   false,
                                   "Toggle if actions like watching TV, working, social media etc. should have no limits");


        timeMultiplier = Config.Bind("Time",
                                    "TimeMultiplier",
                                    1,
                                    new ConfigDescription("Multiplier to time, speeds up scene transitions, attribute gains, topics/item received splash etc. Hold CTRL for 5 * multiplier",
                                    new AcceptableValueRange<int>(1, 10)));

        playerXpMultiplier = Config.Bind("Gameplay",
                                   "PlayerXpMultiplier",
                                   1,
                                   new ConfigDescription("Multiplier to skill xp gained for player",
                                   new AcceptableValueRange<int>(1, 30)));
        npcXpMultiplier = Config.Bind("Gameplay",
                                  "NpcXpMultiplier",
                                  1,
                                  new ConfigDescription("Multiplier to skill xp gained for NPCs",
                                  new AcceptableValueRange<int>(1, 10)));

        modShowKey = Config.Bind("Key",
                                    "ShowMod",
                                    KeyCode.C,
                                    "Shortcut key for showing or hiding the mod");
        maxCharactersKey = Config.Bind("Key",
                                   "MaxCharacterValues",
                                   KeyCode.M,
                                   "Shortcut key for maxing all non player character's stats and interests");
        fastForwardKey = Config.Bind("Key",
                                    "FastForward",
                                    KeyCode.LeftControl,
                                    "Shortcut key for 5x time multiplier when held");

        timeMinusKey = Config.Bind("Key",
                                    "TimeMinus",
                                    KeyCode.Q,
                                    "Moves time back 1 hour. Cannot go backwards in days");
        timePlusKey = Config.Bind("Key",
                                    "TimePlus",
                                    KeyCode.W,
                                    "Moves time forward 1 hour. If past midnight, rolls over to next day");
        skip24HoursKey = Config.Bind("Key",
                                    "Skip24Hours",
                                    KeyCode.E,
                                    "Skips 24 hours");

        page1Key = Config.Bind("Key",
                            "Page1",
                            KeyCode.Alpha1,
                            "Goes to page 1");
        page2Key = Config.Bind("Key",
                            "Page2",
                            KeyCode.Alpha2,
                            "Goes to page 2");

        resetActionsKey = Config.Bind("Key",
                                    "ResetActions",
                                    KeyCode.R,
                                    "Resets actions like watching TV or working");

        toggleInfStatsKey = Config.Bind("Key",
                                    "ToggleInfiniteStatsKey",
                                    KeyCode.F2,
                                    "Shortcut key for ToggleInfiniteStats");
        toggleInfArousalKey = Config.Bind("Key",
                                    "ToggleInfiniteArousal",
                                    KeyCode.F3,
                                    "Shortcut key for ToggleInfiniteArousal");
        toggleResetLifestyleCDKey = Config.Bind("Key",
                                    "ToggleResetLifestyleCD",
                                    KeyCode.F4,
                                    "Shortcut key for ToggleResetLifestyleCD");
        toggleSkipTimeReloadLocationKey = Config.Bind("Key",
                                    "ToggleSkipTimeReloadLocation",
                                     KeyCode.F5,
                                    "Shortcut key for ToggleSkipTimeReloadLocation");
        toggleInfCleanlinessKey = Config.Bind("Key",
                                    "ToggleInfCleanliness",
                                    KeyCode.F6,
                                    "Shortcut key for ToggleInfCleanliness");
        toggleInfTopicsKey = Config.Bind("Key",
                                    "ToggleInfTopics",
                                    KeyCode.F7,
                                    "Shortcut key for ToggleInfTopics");
        toggleKeepPermTopicKey = Config.Bind("Key",
                            "ToggleKeepPermTopic",
                            KeyCode.F8,
                            "Shortcut key for ToggleKeepPermTopic");
        toggleInfActionsKey = Config.Bind("Key",
                            "ToggleInfiniteActions",
                            KeyCode.F9,
                            "Shortcut key for ToggleInfiniteActions");

    }

    [HarmonyPatch(typeof(gamemanager), "Start")]
    private class gamemanager_cheatmod
    {
        [HarmonyPostfix]
        private static void Postfix() => Mods.CreateInstance();
    }

    public static void CreateInstance()
    {
        Mods mods = Object.FindObjectOfType<Mods>();
        if (Object.Equals((Object)mods, (Object)null))
            mods = new GameObject(nameof(Mods)).AddComponent<Mods>();
        Object.DontDestroyOnLoad((Object)mods);
        Mods.Instance = mods;
        player.ins.playerSkills.Skills.Sort((s1, s2) => s1.Moniker.CompareTo(s2.Moniker));
        Inventory.ins.TagsList.Sort((t1, t2) => t1.associated_TAG.name.CompareTo(t2.associated_TAG.name));
        Inventory.ins.Master_TopicList.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));
    }

    public static Mods Instance;
    private bool modShow;

    private ConfigEntry<KeyCode> modShowKey;

    private ConfigEntry<KeyCode> page1Key;
    private ConfigEntry<KeyCode> page2Key;

    private ConfigEntry<KeyCode> toggleInfStatsKey;
    private ConfigEntry<KeyCode> toggleInfArousalKey;
    private ConfigEntry<KeyCode> toggleResetLifestyleCDKey;
    private ConfigEntry<KeyCode> toggleInfCleanlinessKey;
    private ConfigEntry<KeyCode> toggleSkipTimeReloadLocationKey;
    private ConfigEntry<KeyCode> toggleInfTopicsKey;
    private ConfigEntry<KeyCode> toggleKeepPermTopicKey;
    private ConfigEntry<KeyCode> toggleInfActionsKey;

    private ConfigEntry<KeyCode> maxCharactersKey;

    private ConfigEntry<KeyCode> fastForwardKey;

    private ConfigEntry<KeyCode> resetActionsKey;

    private int playerSkillSelected;

    private ConfigEntry<int> playerXpMultiplier;
    private ConfigEntry<int> npcXpMultiplier;

    private bool fastForwardHeld;
    private ConfigEntry<KeyCode> timeMinusKey;
    private ConfigEntry<KeyCode> timePlusKey;
    private ConfigEntry<KeyCode> skip24HoursKey;
    private ConfigEntry<int> timeMultiplier;

    private int topicSelected;
    private int tagSelected;

    private int characterSelected;
    private int characterATSelected = -1;
    private int characterSkillSelected;

    private ConfigEntry<bool> toggleInfStats;
    private ConfigEntry<bool> toggleInfArousal;
    private ConfigEntry<bool> toggleResetLifestyleCD;
    private ConfigEntry<bool> toggleInfCleanliness;
    private ConfigEntry<bool> toggleSkipTimeReloadLocation;
    private ConfigEntry<bool> toggleInfTopics;
    private ConfigEntry<bool> toggleKeepPermTopic;
    private ConfigEntry<bool> toggleInfActions;

    private Dictionary<int, topics> convoLocationOfPermTopic = new Dictionary<int, topics>();

    string playerSkillFilter = "";
    string characterTopicFilter = "";

    string topicsFilter = "";
    string tagsFilter = "";

    public bool IsInfiniteTopics()
    {
        return toggleInfTopics.Value;
    }

    public bool IsKeepPermTopics()
    {
        return toggleKeepPermTopic.Value;
    }

    public int getPlayerXpMultiplier()
    {
        return playerXpMultiplier.Value;
    }
    public int getNPCXpMultiplier()
    {
        return npcXpMultiplier.Value;
    }

    private void Update()
    {
        this.HandleKeys();
        this.Effects();
    }

    public void Effects()
    {
        Time.timeScale = this.fastForwardHeld ? this.timeMultiplier.Value * 5 : this.timeMultiplier.Value;
        if (this.toggleInfStats.Value)
        {
            player.ins.focus = player.ins.getFocusMax();
            player.ins.health = player.ins.getHealthMax();
            player.ins.stamina = player.ins.getStaminaMax();
            player.ins.cleanRate = 10;
        }
        if (this.toggleInfArousal.Value)
            player.ins.arousal = player.ins.getArousalMax() / 2;
        if (this.toggleResetLifestyleCD.Value)
            this.ResetLifestyleCD();
        if (this.toggleInfCleanliness.Value)
           this. CleanAllRooms();
        if (this.toggleInfActions.Value)
            this.ResetActions();
    }

    public void HandleKeys()
    {
        if (Input.GetKeyDown(page1Key.Value))
        {
            modShow = true;
            PageCurrent = 0;
        }
        if (Input.GetKeyDown(page2Key.Value))
        {
            modShow = true;
            PageCurrent = 1;
        }

        if (Input.GetKeyDown(modShowKey.Value))
            this.modShow = !this.modShow;
        if (Input.GetKeyDown(toggleInfStatsKey.Value))
            this.ToggleInfiniteStats();
        if (Input.GetKeyDown(toggleInfArousalKey.Value))
            this.ToggleInfiniteArousal();
        if (Input.GetKeyDown(toggleResetLifestyleCDKey.Value))
            this.ToggleResetLifestyleCD();
        if (Input.GetKeyDown(toggleSkipTimeReloadLocationKey.Value))
            this.ToggleSkipTimeReloadLocation();
        if (Input.GetKeyDown(toggleInfCleanlinessKey.Value))
            this.ToggleInfCleanliness();
        if (Input.GetKeyDown(toggleInfTopicsKey.Value))
            this.ToggleInfTopics();
        if (Input.GetKeyDown(toggleKeepPermTopicKey.Value))
            this.ToggleKeepPermTopics();

        if (Input.GetKeyDown(timeMinusKey.Value))
            this.SkipHours(-1);
        if (Input.GetKeyDown(timePlusKey.Value))
            this.SkipHours(1);
        if (Input.GetKeyDown(skip24HoursKey.Value))
            this.SkipHours(24);

        if (Input.GetKeyDown(resetActionsKey.Value))
        {
            this.ResetActions();
            this.SkipHours(0);
        }

        this.fastForwardHeld = Input.GetKey(fastForwardKey.Value);

        if (Input.GetKeyDown(maxCharactersKey.Value))
        {
            this.MaxCharactersExp();
            this.SetCharacterValues(100);
        }
    }

    private void ToggleInfiniteStats() => this.toggleInfStats.Value = !this.toggleInfStats.Value;

    private void ToggleInfiniteArousal() => this.toggleInfArousal.Value = !this.toggleInfArousal.Value;

    public void ToggleResetLifestyleCD() => this.toggleResetLifestyleCD.Value = !this.toggleResetLifestyleCD.Value;

    public void ToggleSkipTimeReloadLocation() => this.toggleSkipTimeReloadLocation.Value = !this.toggleSkipTimeReloadLocation.Value;

    public void ToggleInfCleanliness() => this.toggleInfCleanliness.Value = !this.toggleInfCleanliness.Value;

    public void ToggleInfTopics() => this.toggleInfTopics.Value = !this.toggleInfTopics.Value;

    public void ToggleKeepPermTopics()
    {
       this.toggleKeepPermTopic.Value = !this.toggleKeepPermTopic.Value;
        if (!this.toggleKeepPermTopic.Value)
            ClearRememberedPermTopics();
    }

    public void ToggleInfActions() => this.toggleInfActions.Value = !this.toggleInfActions.Value;


    private void AddSecondarySkills()
    {
        foreach (Skill skill in player.ins.playerSkills.Skills)
        {
            player.ins.playerSkills.gainPassiveXP(skill, 100);
        }
    }

    private void MaxCharactersExp()
    {
        foreach (character_Script character in characters.ins.Character_List)
        {
            character.cheatPreferences();
        }
    }

    private void SetCharacterValues(int value)
    {
        foreach (character_Script character in characters.ins.Character_List)
        {
            character.followers = 999;
            character.lust = value;
            character.relationship = value;
            character.interest = value;
            character.intoxication = value;
            character.dominance = character.dominance > 0 ? value : -value;
        }
    }

    private void AddMoneyToPlayer(int money) => player.ins.addMoney(money);

    private void ResetLifestyleCD()
    {
        player.ins.lifestyleDominanceCD = 0;
        player.ins.lifestyleHealthyCD = 0;
        player.ins.lifestyleMoralCD = 0;
        player.ins.lifestyleOrderCD = 0;
        player.ins.lifestyleStraightCD = 0;
        player.ins.lifestyleTidyCD = 0;
    }

    private void SkipHours(int hourstoadd)
    {
        gamemanager.ins.StartEnumeration(Delay(hourstoadd));
    }
    private IEnumerator Delay(int hourstoadd)
    {
        Scheduler.ins.TimeAdvancing = true;
        Scheduler.ins.waitingForAdvance();
        yield return new WaitForSeconds(0.1f);
        Scheduler.ins.advanceTime(hourstoadd);
        while (Scheduler.QueuedScheduleCheck) { yield return null; }
        if (this.toggleSkipTimeReloadLocation.Value)
            gamemanager.ins.reloadLoacation(true);
        else
            UI_Manager.ins.UpdateUI();
    }

    private Array GetPossibleATs(character_Script character)
    {
        if (character.Assigned == CharacterList.Becky)
            return Enum.GetValues(typeof(BeckyATs));

        if (character.Assigned == CharacterList.Charlie)
            return Enum.GetValues(typeof(CharlieATs));

        if (character.Assigned == CharacterList.Diane)
            return Enum.GetValues(typeof(DianeATs));

        if (character.Assigned == CharacterList.Gina)
            return Enum.GetValues(typeof(GinaATs));

        if (character.Assigned == CharacterList.Jay)
            return Enum.GetValues(typeof(JayATs));

        if (character.Assigned == CharacterList.Jesse)
            return Enum.GetValues(typeof(JesseATs));

        if (character.Assigned == CharacterList.Lisa)
            return Enum.GetValues(typeof(LisaATs));

        if (character.Assigned == CharacterList.Morgan)
            return Enum.GetValues(typeof(MorganATs));

        if (character.Assigned == CharacterList.Richard)
            return Enum.GetValues(typeof(RichardATs));

        if (character.Assigned == CharacterList.Sally)
            return Enum.GetValues(typeof(SallyATs));

        if (character.Assigned == CharacterList.Tilley)
            return Enum.GetValues(typeof(TilleyATs));

        return null;
    }

    private String GetATToLoad(Array ats, int idx)
    {
        if (ats == null || idx < 0 || idx >= ats.Length)
            return "none";
        return ats.GetValue(idx).ToString();
    }

    private bool AddRoomCleanliness(location loc, int amt)
    {
        if (loc != null && loc.cleanable)
        {
            loc.clean(amt);
            return true;
        }
        return false;
    }

    private void CleanAllRooms()
    {
        foreach (location loc in gamemanager.ins.LM.locations.Values)
            AddRoomCleanliness(loc, 999);
    }

    private void ResetActions()
    {
        player.ins.resetVainAction();
        player.ins.napamount = 0;
        player.ins.restamount = 0;
        player.ins.tvAmount = 0;
        player.ins.resetVideogameToggle();
        player.ins.resetSnack();
        player.ins.hasgroomed = false;
        player.ins.watchedLivestream = false;
        player.ins.hasWardrobed = false;
        player.ins.hasResearched = false;
        player.ins.hasWorkedToday = false;
        player.ins.hasYardWork = false;

        foreach (location loc in gamemanager.ins.LM.locations.Values)
        {
            loc.dailyCD1 = false;
            loc.dailyCD2 = false;
            loc.dailyCD3 = false;
            loc.weeklyCD1 = false;
            loc.localCount = 0;
        }
    }

    private void GetRandomTopicWithTag(dictionary_Tag tag)
    {
        List<topics> topics = tag.TopicsWithTag.FindAll(t => t.isTemp());
        if (topics.IsNullOrEmpty())
            return;
        System.Random rnd = new System.Random();
        player.ins.gainItem(topics.ElementAt(rnd.Next(0, topics.Count - 1)));
    }

    private string PrintTags(tags[] associatedTags)
    {
        return String.Join(", ", associatedTags.Select(t => t.name).ToArray());
    }

    private void Bailout()
    {
        location location = LocationsManager.ins.GetLocation("Your Room");
        player.ins.setLocation(location.LocaleName);
        Scheduler.ins.Relocating = false;
        SkipHours(0);
    }

    public void RememberTopicLocation(int location, topics topic)
    {
        convoLocationOfPermTopic[location] =  topic;
    }

    public void RestorePermTopics()
    {
        foreach (KeyValuePair<int, topics> e in convoLocationOfPermTopic)
            Inventory.ins.setHotbarTopic_explicit(e.Key, e.Value.ID);
        ClearRememberedPermTopics();
    }

    public void ClearRememberedPermTopics()
    {
        this.convoLocationOfPermTopic.Clear();
    }

    // ------------------------------------------------------------------------

    public static int PageCurrent;
    public static int PageTotal;
    public static int[] PageLines;
    public static Rect ModWindowPos;
    public static float ModWindowWidht;

    private void OnGUI()
    {
        if (!this.modShow)
            return;
        this.UITitle();
        this.UIPages();
    }

    public void InitSize()
    {
        Mods.PageTotal = 2;
        Mods.PageLines = new int[Mods.PageTotal];
        Mods.ModWindowWidht = 750f;//(float)(Screen.width / 4) + 100f;
        Mods.ModWindowPos = new Rect(10f, 10f, Mods.ModWindowWidht, 10f);
        Mods.PageCurrent = 0;
        Mods.PageLines[0] = 0;
        Mods.PageLines[1] = 0;
    }

    public void UITitle() => GUI.Box(Draw.Window(), Info.Metadata.Version + " - " + Info.Metadata.Name, Draw.BgStyle);

    public void PageChange(int page)
    {
        if (Mods.PageCurrent + page > Mods.PageTotal - 1)
            return;
        if (Mods.PageCurrent + page <= 0)
            Mods.PageCurrent = 0;
        else
            Mods.PageCurrent += page;
    }

    private String GetCheatPageDescription(int page)
    {
        if (page == 0)
            return "(Player and global cheats) (" + page1Key.Value + ")";
        if (page == 1)
            return "(Individual character cheats) (" + page2Key.Value + ")";
        return "";
    }

    public void gainXp(Skill skill, int num)
    {
        if (skill.NotCapped())
        {
            if (skill.gainXP(num))
            {
                skill.ProcessRankUP();
            }

            gamemanager.ins.queue.EnqueueAction(player.ins.Sendmessage("+ " + num + " " + skill.Moniker + " XP"));
        }
    }

    public void UIPages()
    {

        if (GUI.Button(Draw.BtnPagePrevious(1), "<<", Draw.BtnStyle))
            this.PageChange(-1);
        if (GUI.Button(Draw.BtnPageNext(1), ">>", Draw.BtnStyle))
            this.PageChange(1);
        GUI.Label(Draw.BtnPage(1), "PAGE " + (Mods.PageCurrent+1).ToString() + " " + GetCheatPageDescription(Mods.PageCurrent), Draw.BtnStyle);
        if (Mods.PageCurrent == 0)
        {
            int btnRow = 1;

            GUI.Label(Draw.LabelShorter(++btnRow), "Money (Current: " + player.ins.money + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2Shorter(btnRow), "-100", Draw.BtnStyle))
                this.AddMoneyToPlayer(-100);
            if (GUI.Button(Draw.BtnLeft1Shorter(btnRow), "+100", Draw.BtnStyle))
                this.AddMoneyToPlayer(100);
            if (GUI.Button(Draw.BtnRight12(btnRow), "+10000", Draw.BtnStyle))
                this.AddMoneyToPlayer(10000);

            if (GUI.Button(Draw.LabelThirdLeft(++btnRow), "Add charm", Draw.BtnStyle))
                player.ins.advCharm();
            if (GUI.Button(Draw.LabelThirdCenter(btnRow), "Add physique", Draw.BtnStyle))
                player.ins.advPhysique();
            if (GUI.Button(Draw.LabelThirdRight(btnRow), "Add smarts", Draw.BtnStyle))
                player.ins.advSmarts();

            playerSkillFilter = Draw.TextFieldShort(++btnRow, playerSkillFilter, Draw.BtnStyle);
            List<Skill> filteredPlayerSkills = player.ins.playerSkills.Skills.FindAll(s => s.Moniker.ToLower().Contains(playerSkillFilter.ToLower()));
            this.playerSkillSelected = Mathf.Clamp(this.playerSkillSelected, 0, filteredPlayerSkills.Count - 1);
            GUI.Label(Draw.HalfLaberlShorter(btnRow), "Player skill xp: " + (filteredPlayerSkills.IsNullOrEmpty() ? "NaN" : filteredPlayerSkills[this.playerSkillSelected].Moniker) + " (Current: " + (filteredPlayerSkills.IsNullOrEmpty() ? "NaN" : filteredPlayerSkills[this.playerSkillSelected].XP.ToString()) + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft1Shorter(btnRow), "<<", Draw.BtnStyle))
                this.playerSkillSelected = Mathf.Clamp(this.playerSkillSelected - 1, 0, filteredPlayerSkills.Count - 1);
            if (GUI.Button(Draw.BtnLeft2Shorter(btnRow), ">>", Draw.BtnStyle))
                this.playerSkillSelected = Mathf.Clamp(this.playerSkillSelected + 1, 0, filteredPlayerSkills.Count - 1);
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+50", Draw.BtnStyle) && !filteredPlayerSkills.IsNullOrEmpty())
                player.ins.playerSkills.gainPassiveXP(filteredPlayerSkills[this.playerSkillSelected], 50);

            GUI.Label(Draw.LabelFull(++btnRow), "Add all player skills exp", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), "+100", Draw.BtnStyle))
                this.AddSecondarySkills();

            GUI.Label(Draw.LabelFull(++btnRow), "Max all other character skills and stats (" + maxCharactersKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), "Max", Draw.BtnStyle))
            {
                this.MaxCharactersExp();
                this.SetCharacterValues(100);
            }

            topicsFilter = Draw.TextField(++btnRow, topicsFilter, Draw.BtnStyle);
            List<topics> filteredSkills = Inventory.ins.Master_TopicList.FindAll(t => t.Name.ToLower().Contains(topicsFilter.ToLower()));
            this.topicSelected = Mathf.Clamp(this.topicSelected, 0, filteredSkills.Count - 1);
            if (GUI.Button(Draw.HalfLabelRight(btnRow), "Click: " + (filteredSkills.IsNullOrEmpty() ? "NaN" : filteredSkills[this.topicSelected].Name), Draw.BtnStyle) && !filteredSkills.IsNullOrEmpty())
                player.ins.gainItem(filteredSkills[this.topicSelected]);
            if (GUI.Button(Draw.BtnLeft1(btnRow), "<<", Draw.BtnStyle))
                this.topicSelected = Mathf.Clamp(this.topicSelected - 1, 0, filteredSkills.Count - 1);
            if (GUI.Button(Draw.BtnLeft2(btnRow), ">>", Draw.BtnStyle))
                this.topicSelected = Mathf.Clamp(this.topicSelected + 1, 0, filteredSkills.Count - 1);
            GUI.Label(Draw.LabelFull(++btnRow), "Selected topic tags: " + (filteredSkills.IsNullOrEmpty() ? "NaN" : PrintTags(filteredSkills[this.topicSelected].associatedTags)), Draw.BtnStyle);


            tagsFilter = Draw.TextField(++btnRow, tagsFilter, Draw.BtnStyle);
            List<dictionary_Tag> filteredTags = Inventory.ins.TagsList.FindAll(t => t.associated_TAG.name.ToLower().Contains(tagsFilter.ToLower()));
            this.tagSelected = Mathf.Clamp(this.tagSelected, 0, filteredTags.Count - 1);
            if (GUI.Button(Draw.HalfLabelRight(btnRow), "Click: " + (filteredTags.IsNullOrEmpty() ? "NaN" : filteredTags[this.tagSelected].associated_TAG.name), Draw.BtnStyle) && !filteredTags.IsNullOrEmpty())
                GetRandomTopicWithTag(filteredTags[this.tagSelected]);
            if (GUI.Button(Draw.BtnLeft1(btnRow), "<<", Draw.BtnStyle))
                this.tagSelected = Mathf.Clamp(this.tagSelected - 1, 0, filteredTags.Count - 1);
            if (GUI.Button(Draw.BtnLeft2(btnRow), ">>", Draw.BtnStyle))
                this.tagSelected = Mathf.Clamp(this.tagSelected + 1, 0, filteredTags.Count - 1);

            GUI.Label(Draw.LabelShorter(++btnRow), "Change hours (-1h = " + timeMinusKey.Value + ", +1h = " + timePlusKey.Value + ", +24h = " + skip24HoursKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2Shorter(btnRow), "-", Draw.BtnStyle))
                this.SkipHours(-1);
            if (GUI.Button(Draw.BtnLeft1Shorter(btnRow), "+", Draw.BtnStyle))
                this.SkipHours(1);
            if (GUI.Button(Draw.BtnRight12(btnRow), "+24h", Draw.BtnStyle))
                this.SkipHours(24);

            GUI.Label(Draw.LabelFull(++btnRow), "Room cleanliness (Current: " + (gamemanager.ins.getCurrentLocation() == null || !gamemanager.ins.getCurrentLocation().cleanable ? "NaN" : (100 - gamemanager.ins.getCurrentLocation().dirtyness).ToString()) + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-20", Draw.BtnStyle))
                if (this.AddRoomCleanliness(gamemanager.ins.getCurrentLocation(), -20) && toggleSkipTimeReloadLocation.Value)
                    SkipHours(0);
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+20", Draw.BtnStyle))
                if (this.AddRoomCleanliness(gamemanager.ins.getCurrentLocation(), 20) && toggleSkipTimeReloadLocation.Value)
                    SkipHours(0);

            GUI.Label(Draw.LabelFull(++btnRow), "Time multiplier (Hold " + fastForwardKey.Value + " for x5): x" + this.timeMultiplier.Value.ToString(), Draw.BtnStyle);
            this.timeMultiplier.Value = (int) Draw.Slider(btnRow, timeMultiplier.Value, 1, 10);

            GUI.Label(Draw.LabelFull(++btnRow), "Player xp multiplier: x" + this.playerXpMultiplier.Value.ToString(), Draw.BtnStyle);
            this.playerXpMultiplier.Value = (int) Draw.Slider(btnRow, playerXpMultiplier.Value, 1, 30);

            GUI.Label(Draw.LabelFull(++btnRow), "Other characters xp multiplier: x" + this.npcXpMultiplier.Value.ToString(), Draw.BtnStyle);
            this.npcXpMultiplier.Value = (int)Draw.Slider(btnRow, npcXpMultiplier.Value, 1, 10);

            GUI.Label(Draw.LabelFull(++btnRow), "Reset daily actions (" + resetActionsKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), "Reset", Draw.BtnStyle))
            {
                this.ResetActions();
                this.SkipHours(0);
            }

            if (GUI.Button(Draw.LabelFull(++btnRow), "Move to Your Room (bailout)", Draw.BtnStyle))
                this.Bailout();

            GUI.Label(Draw.LabelShortest(++btnRow), "Infinite stats (" + toggleInfStatsKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12Shortest(btnRow), this.toggleInfStats.Value.ToString(), Draw.BtnStyle))
                this.ToggleInfiniteStats();

            GUI.Label(Draw.LabelShortestRight(btnRow), "Infinite arousal (" + toggleInfArousalKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), this.toggleInfArousal.Value.ToString(), Draw.BtnStyle))
                this.ToggleInfiniteArousal();

            GUI.Label(Draw.LabelShortest(++btnRow), "Remove lifestyle cooldown (" + toggleResetLifestyleCDKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12Shortest(btnRow), this.toggleResetLifestyleCD.Value.ToString(), Draw.BtnStyle))
                this.ToggleResetLifestyleCD();

            GUI.Label(Draw.LabelShortestRight(btnRow), "Reload after time skip (" + toggleSkipTimeReloadLocationKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), this.toggleSkipTimeReloadLocation.Value.ToString(), Draw.BtnStyle))
                this.ToggleSkipTimeReloadLocation();

            GUI.Label(Draw.LabelShortest(++btnRow), "Infinite room cleanliness (" + toggleInfCleanlinessKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12Shortest(btnRow), this.toggleInfCleanliness.Value.ToString(), Draw.BtnStyle))
                this.ToggleInfCleanliness();

            GUI.Label(Draw.LabelShortestRight(btnRow), "Infinite topic limit (" + toggleInfTopicsKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), this.toggleInfTopics.Value.ToString(), Draw.BtnStyle))
                this.ToggleInfTopics();

            GUI.Label(Draw.LabelShortest(++btnRow), "Conversations keep perm. topics (" + toggleKeepPermTopicKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12Shortest(btnRow), this.toggleKeepPermTopic.Value.ToString(), Draw.BtnStyle))
                this.ToggleKeepPermTopics();

            GUI.Label(Draw.LabelShortestRight(btnRow), "Toggle infinite actions (" + toggleInfActionsKey.Value + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), this.toggleInfActions.Value.ToString(), Draw.BtnStyle))
                this.ToggleInfActions();

          

            Mods.PageLines[0] = btnRow;
        }
        if (Mods.PageCurrent == 1)
        {
            int btnRow = 1;

            character_Script character = characters.ins.Character_List[this.characterSelected];
            Array ats = GetPossibleATs(character);

            GUI.Label(Draw.BtnPage(++btnRow), "Character selected: " + character.name, Draw.BtnStyle);
            if (GUI.Button(Draw.BtnPagePrevious(btnRow), "<<", Draw.BtnStyle) && this.characterSelected > 0)
            {
                --this.characterSelected;
                this.characterATSelected = -1;
            }
            if (GUI.Button(Draw.BtnPageNext(btnRow), ">>", Draw.BtnStyle) && this.characterSelected < characters.ins.Character_List.Count - 1)
            {
                ++this.characterSelected;
                this.characterATSelected = -1;
            }

            if (GUI.Button(Draw.LabelFull(++btnRow), "Current AT: " + character.Archtype.name + ", click to load AT: " + GetATToLoad(ats, this.characterATSelected), Draw.BtnStyle) && GetATToLoad(ats, this.characterATSelected) != "none")
            {
                character.loadedAT = (GetATToLoad(ats, this.characterATSelected));
                character.ArchetypeTransformation();
                gamemanager.ins.updateCharacters();
                SkipHours(0);
            }
            if (GUI.Button(Draw.BtnLeft1(btnRow), "<<", Draw.BtnStyle) && ats != null)
                this.characterATSelected = Mathf.Clamp(this.characterATSelected - 1, -1, ats.Length - 1);
            if (GUI.Button(Draw.BtnLeft2(btnRow), ">>", Draw.BtnStyle) && ats != null)
                this.characterATSelected = Mathf.Clamp(this.characterATSelected + 1, -1, ats.Length - 1);

            GUI.Label(Draw.LabelFull(++btnRow), "Relationship (Current: " + character.relationship + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle))
                character.relationship -= 10;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle))
                character.relationship += 10;

            GUI.Label(Draw.LabelFull(++btnRow), "Lust (Current: " + character.lust + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle))
                character.lust -= 10;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle))
                character.lust += 10;

            GUI.Label(Draw.LabelFull(++btnRow), "Interest (Current: " + character.interest + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle))
                character.interest -= 10;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle))
                character.interest += 10;

            GUI.Label(Draw.LabelFull(++btnRow), "Assertiveness (Current: " + character.dominance + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle))
                character.dominance -= 10;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle))
                character.dominance += 10;

            GUI.Label(Draw.LabelFull(++btnRow), "Intoxication (Current: " + character.intoxication + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle))
                character.intoxication -= 10;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle))
                character.intoxication += 10;

            GUI.Label(Draw.LabelFull(++btnRow), "Followers (Current: " + character.followers + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle))
                character.followers -= 10;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle))
                character.followers += 10;

            GUI.Label(Draw.LabelFull(++btnRow), "weekly_count (Current: " + character.weekly_count + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-1", Draw.BtnStyle))
                character.weekly_count -= 1;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+1", Draw.BtnStyle))
                character.weekly_count += 1;

            GUI.Label(Draw.LabelFull(++btnRow), "AT_Weekly_Counter1 (Current: " + character.AT_Weekly_Counter1 + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-1", Draw.BtnStyle))
                character.AT_Weekly_Counter1 -= 1;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+1", Draw.BtnStyle))
                character.AT_Weekly_Counter1 += 1;

            GUI.Label(Draw.LabelFull(++btnRow), "AT_Counter1 (Current: " + character.AT_Counter1 + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-1", Draw.BtnStyle))
                character.AT_Counter1 -= 1;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+1", Draw.BtnStyle))
                character.AT_Counter1 += 1;

            GUI.Label(Draw.LabelFull(++btnRow), "AT_Counter2 (Current: " + character.AT_Counter2 + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-1", Draw.BtnStyle))
                character.AT_Counter2 -= 1;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+1", Draw.BtnStyle))
                character.AT_Counter2 += 1;

            GUI.Label(Draw.LabelFull(++btnRow), "AT_ChangeDelay (Current: " + character.AT_ChangeDelay + ")", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-1", Draw.BtnStyle))
                character.AT_ChangeDelay -= 1;
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+1", Draw.BtnStyle))
                character.AT_ChangeDelay += 1;

            characterTopicFilter = Draw.TextFieldShort(++btnRow, characterTopicFilter, Draw.BtnStyle);
            List<Skill> filteredTopics = player.ins.playerSkills.Skills.FindAll(s => s.Moniker.ToLower().Contains(characterTopicFilter.ToLower()));
            this.characterSkillSelected = Mathf.Clamp(this.characterSkillSelected, 0, filteredTopics.Count - 1);
            GUI.Label(Draw.HalfLaberlShorter(btnRow), "Topic: " + (filteredTopics.IsNullOrEmpty() ? "NaN" : filteredTopics[this.characterSkillSelected].Moniker) + " (Current: " + (filteredTopics.IsNullOrEmpty() ? "NaN" : character.preferences[filteredTopics[this.characterSkillSelected].Moniker].ToString()) + "%)", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnLeft1Shorter(btnRow), "<<", Draw.BtnStyle))
                this.characterSkillSelected = Mathf.Clamp(this.characterSkillSelected - 1, 0, filteredTopics.Count - 1);
            if (GUI.Button(Draw.BtnLeft2Shorter(btnRow), ">>", Draw.BtnStyle))
                this.characterSkillSelected = Mathf.Clamp(this.characterSkillSelected + 1, 0, filteredTopics.Count - 1);
            if (GUI.Button(Draw.BtnLeft2(btnRow), "-10", Draw.BtnStyle) && !filteredTopics.IsNullOrEmpty())
                characters.ins.setPref(filteredTopics[this.characterSkillSelected].AssignedSO, character, Mathf.Clamp(character.preferences[filteredTopics[this.characterSkillSelected].Moniker] - 10, 0, 100));
            if (GUI.Button(Draw.BtnLeft1(btnRow), "+10", Draw.BtnStyle) && !filteredTopics.IsNullOrEmpty())
                characters.ins.setPref(filteredTopics[this.characterSkillSelected].AssignedSO, character, Mathf.Clamp(character.preferences[filteredTopics[this.characterSkillSelected].Moniker] + 10, 0, 100));

            GUI.Label(Draw.LabelFull(++btnRow), "Max All Skills", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), "Max", Draw.BtnStyle))
            {
                foreach (Skill skill in player.ins.playerSkills.Skills)
                {
                    characters.ins.setPref(skill.AssignedSO, character, 100);
                }
            }

            GUI.Label(Draw.LabelFull(++btnRow), "Reset all flags (massage, photoshoot, homework etc.)", Draw.BtnStyle);
            if (GUI.Button(Draw.BtnRight12(btnRow), "Reset", Draw.BtnStyle))
            {
                character.DailyReset();
                SkipHours(0);
            }

            Mods.PageLines[1] = btnRow;
        }
    }

    public void Start()
    {
        Draw.InitStyles();
        this.InitSize();
    }

}