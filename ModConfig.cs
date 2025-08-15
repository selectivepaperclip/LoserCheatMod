using BepInEx.Configuration;
using System;
using UnityEngine;

namespace LoserCheatMod
{
    internal class ModConfig
    {
        public static ModConfig Instance = new ModConfig();
        private ModConfig() { }

        private ConfigEntry<bool>[] toggleConfigs;
        private ConfigEntry<KeyCode>[] keyConfigs;
        private ConfigEntry<int>[] multiplierConfigs;


        public KeyCode GetShowModsKey() => GetConfigEntry("ShowModsKey", keyConfigs).Value;
        public KeyCode GetPage1Key() => GetConfigEntry("Page1Key", keyConfigs).Value;
        public KeyCode GetPage2Key() => GetConfigEntry("Page2Key", keyConfigs).Value;
        public KeyCode GetPreviousCharacterKey() => GetConfigEntry("PreviousCharacterKey", keyConfigs).Value;
        public KeyCode GetNextCharacterKey() => GetConfigEntry("NextCharacterKey", keyConfigs).Value;

        public int GetPlayerXpMultiplier() => GetConfigEntry("PlayerXpMultiplier", multiplierConfigs).Value;
        public int SetPlayerXpMultiplier(int playerXpMultiplierValue) => GetConfigEntry("PlayerXpMultiplier", multiplierConfigs).Value = playerXpMultiplierValue;

        public int GetPlayerXpMultiplierMinValue() => GetConfigMinValue(GetConfigEntry("PlayerXpMultiplier", multiplierConfigs));
        public int GetPlayerXpMultiplierMaxValue() => GetConfigMaxValue(GetConfigEntry("PlayerXpMultiplier", multiplierConfigs));

        public int GetNpcXpMultiplier() => GetConfigEntry("NpcXpMultiplier", multiplierConfigs).Value;
        public int SetNpcXpMultiplier(int npcXpMultiplierValue) => GetConfigEntry("NpcXpMultiplier", multiplierConfigs).Value = npcXpMultiplierValue;

        public int GetNpcXpMultiplierMinValue() => GetConfigMinValue(GetConfigEntry("NpcXpMultiplier", multiplierConfigs));
        public int GetNpcXpMultiplierMaxValue() => GetConfigMaxValue(GetConfigEntry("NpcXpMultiplier", multiplierConfigs));

        public int GetTimeMultiplier() => GetConfigEntry("TimeMultiplier", multiplierConfigs).Value;
        public int SetTimeMultiplier(int timeMultiplierValue) => GetConfigEntry("TimeMultiplier", multiplierConfigs).Value = timeMultiplierValue;
        public int GetTimeMultiplierMinValue() => GetConfigMinValue(GetConfigEntry("TimeMultiplier", multiplierConfigs));
        public int GetTimeMultiplierMaxValue() => GetConfigMaxValue(GetConfigEntry("TimeMultiplier", multiplierConfigs));

        public int SetSpecifiedHourForSkipping(int specifiedHour) => GetConfigEntry("SpecifiedHourForSkipping", multiplierConfigs).Value = specifiedHour;
        public int GetSpecifiedHourForSkipping() => GetConfigEntry("SpecifiedHourForSkipping", multiplierConfigs).Value;
        public int GetSpecifiedHourForSkippingMinValue() => GetConfigMinValue(GetConfigEntry("SpecifiedHourForSkipping", multiplierConfigs));
        public int GetSpecifiedHourForSkippingMaxValue() => GetConfigMaxValue(GetConfigEntry("SpecifiedHourForSkipping", multiplierConfigs));

        public KeyCode GetToggleInfiniteStatsKey() => GetConfigEntry("ToggleInfiniteStatsKey", keyConfigs).Value;
        public KeyCode GetToggleInfiniteArousalKey() => GetConfigEntry("ToggleInfiniteArousalKey", keyConfigs).Value;
        public KeyCode GetToggleResetLifestyleCDKey() => GetConfigEntry("ToggleResetLifestyleCDKey", keyConfigs).Value;
        public KeyCode GetToggleInfiniteCleanlinessKey() => GetConfigEntry("ToggleInfiniteCleanlinessKey", keyConfigs).Value;
        public KeyCode GetToggleInfiniteTopicsKey() => GetConfigEntry("ToggleInfiniteTopicsKey", keyConfigs).Value;

        public void ToggleInfiniteStats() => GetConfigEntry("ToggleInfiniteStats", toggleConfigs).Value = !IsToggleInfiniteStats();
        public void ToggleInfiniteArousal() => GetConfigEntry("ToggleInfiniteArousal", toggleConfigs).Value = !IsToggleInfiniteArousal();
        public void ToggleResetLifestyleCD() => GetConfigEntry("ToggleResetLifestyleCD", toggleConfigs).Value = !IsToggleResetLifestyleCD();
        public void ToggleInfiniteCleanliness() => GetConfigEntry("ToggleInfiniteCleanliness", toggleConfigs).Value = !IsToggleInfiniteCleanliness();
        public void ToggleInfiniteTopics() => GetConfigEntry("ToggleInfiniteTopics", toggleConfigs).Value = !IsToggleInfiniteTopics();
        public void ToggleHotbarChecksCanSearchInventory() => GetConfigEntry("ToggleHotbarChecksCanSearchInventory", toggleConfigs).Value = !IsToggleHotbarChecksCanSearchInventory();
        public void ToggleShowOpenModHint() => GetConfigEntry("ToggleShowOpenModHint", toggleConfigs).Value = !IsToggleShowOpenModHint();


        public bool IsToggleInfiniteStats() => GetConfigEntry("ToggleInfiniteStats", toggleConfigs).Value;
        public bool IsToggleInfiniteArousal() => GetConfigEntry("ToggleInfiniteArousal", toggleConfigs).Value;
        public bool IsToggleResetLifestyleCD() => GetConfigEntry("ToggleResetLifestyleCD", toggleConfigs).Value;
        public bool IsToggleInfiniteCleanliness() => GetConfigEntry("ToggleInfiniteCleanliness", toggleConfigs).Value;
        public bool IsToggleInfiniteTopics() => GetConfigEntry("ToggleInfiniteTopics", toggleConfigs).Value;
        public bool IsToggleHotbarChecksCanSearchInventory() => GetConfigEntry("ToggleHotbarChecksCanSearchInventory", toggleConfigs).Value;

        public bool IsToggleShowOpenModHint() => GetConfigEntry("ToggleShowOpenModHint", toggleConfigs).Value;

        public KeyCode GetMaxPlayerStatsKey() => GetConfigEntry("MaxPlayerStatsKey", keyConfigs).Value;
        public KeyCode GetResetActionsKey() => GetConfigEntry("ResetActionsKey", keyConfigs).Value;

        public KeyCode GetFastForwardKey() => GetConfigEntry("FastForwardKey", keyConfigs).Value;

        public KeyCode GetTimeMinusKey() => GetConfigEntry("TimeMinusKey", keyConfigs).Value;
        public KeyCode GetTimePlusKey() => GetConfigEntry("TimePlusKey", keyConfigs).Value;
        public KeyCode GetSkip24HoursKey() => GetConfigEntry("Skip24HoursKey", keyConfigs).Value;
        public KeyCode GetSkipToNextSpecifiedHourKey() => GetConfigEntry("SkipToNextSpecifiedHourKey", keyConfigs).Value;


        public int GetConfigMinValue(ConfigEntry<int> configEntry) => ((AcceptableValueRange<int>)configEntry.Description.AcceptableValues).MinValue;
        public int GetConfigMaxValue(ConfigEntry<int> configEntry) => ((AcceptableValueRange<int>)configEntry.Description.AcceptableValues).MaxValue;

        private ConfigEntry<bool> GetConfigEntry(string key, ConfigEntry<bool>[] configs) => Array.Find(configs, c => c.Definition.Key.Equals(key));
        private ConfigEntry<KeyCode> GetConfigEntry(string key, ConfigEntry<KeyCode>[] configs) => Array.Find(configs, c => c.Definition.Key.Equals(key));
        private ConfigEntry<int> GetConfigEntry(string key, ConfigEntry<int>[] configs) => Array.Find(configs, c => c.Definition.Key.Equals(key));

        public void ConfigBindings(ConfigFile config)
        {
            toggleConfigs = [
                config.Bind("Toggles",                                                          // The section under which the option is shown
                            "ToggleInfiniteStats",                                              // The key of the configuration option in the configuration file
                            false,                                                              // The default value
                            "Toggles infinite stats (focus, stamina, health set to max)"),      // Description of the option to show in the config file
                config.Bind("Toggles", "ToggleInfiniteArousal", false, "Infinite arousal (slightly below max), needs to be turned off to cum in sex minigame"),
                config.Bind("Toggles", "ToggleResetLifestyleCD", false, "Removes the cooldown of changing lifestyles"),
                config.Bind("Toggles", "ToggleInfiniteCleanliness", false, "Keeps all room at 100% clean"),
                config.Bind("Toggles", "ToggleInfiniteTopics", false, "Sets the limit of topics in invetory to 99999"),
                config.Bind("Toggles", "ToggleHotbarChecksCanSearchInventory", false, "Toggle whether actions that require certain items/topics are allowed to search inventory (items/topics not in hotbar wont be consumed)"),
                config.Bind("Toggles", "ToggleShowOpenModHint", true, "Toggle if 'press ShowModsKey to open' is displayed, when mod is closed"),
            ];

            keyConfigs = [
                config.Bind("Key", "ShowModsKey", KeyCode.X, "Shortcut key for showing or hiding the mod"),
                config.Bind("Key", "Page1Key", KeyCode.LeftArrow, "Goes to page 1"),
                config.Bind("Key", "Page2Key", KeyCode.RightArrow, "Goes to page 2"),
                config.Bind("Key", "PreviousCharacterKey", KeyCode.UpArrow, "Switches to previous character selection"),
                config.Bind("Key", "NextCharacterKey", KeyCode.DownArrow, "Switches to next character selection"),
                config.Bind("Key", "MaxPlayerStatsKey", KeyCode.M, "Shortcut key for maxing all player stats (focus, stamina, health)"),
                config.Bind("Key", "FastForwardKey", KeyCode.LeftControl, "Shortcut key for 5x time multiplier when held"),
                config.Bind("Key", "TimeMinusKey", KeyCode.Alpha1, "Moves time back 1 hour. Can go backwards in days"),
                config.Bind("Key", "TimePlusKey", KeyCode.Alpha2, "Moves time forward 1 hour. If past midnight, rolls over to next day"),
                config.Bind("Key", "Skip24HoursKey", KeyCode.Alpha3, "Skips 24 hours"),
                config.Bind("Key", "SkipToNextSpecifiedHourKey", KeyCode.Alpha4, "Skips to next specific hour"),
                config.Bind("Key", "ResetActionsKey", KeyCode.R, "Resets actions like watching TV or working"),
                config.Bind("Key", "ToggleInfiniteStatsKey", KeyCode.F2, "Shortcut key for ToggleInfiniteStats"),
                config.Bind("Key", "ToggleInfiniteArousalKey", KeyCode.F3, "Shortcut key for ToggleInfiniteArousal"),
                config.Bind("Key", "ToggleResetLifestyleCDKey", KeyCode.F4, "Shortcut key for ToggleResetLifestyleCD"),
                config.Bind("Key", "ToggleInfiniteCleanlinessKey", KeyCode.F5, "Shortcut key for ToggleInfiniteCleanliness"),
                config.Bind("Key", "ToggleInfiniteTopicsKey", KeyCode.F6, "Shortcut key for ToggleInfiniteTopics"),
            ];

            multiplierConfigs = [
                config.Bind("Time", "TimeMultiplier", 1, new ConfigDescription("Multiplier to time, speeds up scene transitions, attribute gains, topics/item received splash etc. Hold CTRL for 5 * multiplier", new AcceptableValueRange<int>(1, 10))),
                config.Bind("Time", "SpecifiedHourForSkipping", 18, new ConfigDescription("Specified hour for 'skip to specified hour' feature", new AcceptableValueRange<int>(0, 23))),
                config.Bind("Gameplay", "PlayerXpMultiplier", 1, new ConfigDescription("Multiplier to skill xp gained for player", new AcceptableValueRange<int>(1, 5))),
                config.Bind("Gameplay", "NpcXpMultiplier", 1, new ConfigDescription("Multiplier to skill xp gained for NPCs", new AcceptableValueRange<int>(1, 5))),
            ];
        }

    }
}
