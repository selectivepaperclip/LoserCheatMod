using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoserCheatMod
{
    // as the name implies, handles everything related to first page of the cheatmod
    // global and player related cheats like toggle infinite stats, cheat money, spawn topics, etc. are found here
    internal class Page1GUI : MonoBehaviour, ModsGUI.IPageGUI
    {
        public static Page1GUI Instance;
        private Page1GUI() { }

        private int playerSkillSelected;
        private int topicSelected;
        private int tagSelected;
        private int lockedLocationSelected;

        private string playerSkillFilter = "";
        private string topicsFilter = "";
        private string tagsFilter = "";


        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        public string GetPageDescription()
        {
            return "Player and global cheats";
        }

        public void RenderPage()
        {
            ModsGUI.NewLine(ModsGUI.HalfWidth(EditPlayerFocus), ModsGUI.HalfWidth(EditPlayerStamina));
            ModsGUI.NewLine(ModsGUI.HalfWidth(EditPlayerHealth), ModsGUI.HalfWidth(EditPlayerArousal));
            ModsGUI.NewLine(MaxAllPlayerStats);

            ModsGUI.NewLine(AddPlayerStats);

            ModsGUI.NewLine(EditPlayerMoney);
            ModsGUI.NewLine(EditPlayerSkillsXp);
            ModsGUI.NewLine(GetAllPlayerSkillsXp);

            List<topics> filteredSkills = Mods.Instance.GetAllTopics().FindAll(t => t.Name.ToLower().Contains(topicsFilter.ToLower()));
            topicSelected = Mathf.Clamp(topicSelected, 0, filteredSkills.Count - 1);
            ModsGUI.NewLine(() => SpawnSelectedTopic(filteredSkills));
            ModsGUI.NewLine(() => SelectedTopicTags(filteredSkills));
            ModsGUI.NewLine(SpawnTopicBySelectedTag);

            if (gamemanager.ins.getCurrentLocation() != null && gamemanager.ins.getCurrentLocation().cleanable)
                ModsGUI.NewLine(RoomCleaning);

            ModsGUI.NewLine(ChangeTime);
            ModsGUI.NewLine(TimeMultiplierSelector);
            ModsGUI.NewLine(ModsGUI.HalfWidth(PlayerXpMultiplierSelector), ModsGUI.HalfWidth(NpcXpMultiplierSelector));

            ModsGUI.NewLine(ResetDailyActions);
            ModsGUI.NewLine(MoveToPlayerRoom);

            List<Location_Script> hiddenLocations = Mods.Instance.GetLockedLocations();
            if (!hiddenLocations.IsNullOrEmpty())
                ModsGUI.NewLine(() => UnlockLocations(hiddenLocations));

            ModsGUI.NewLine(ModsGUI.HalfWidth(InfiniteStatsToggle), ModsGUI.HalfWidth(InfiniteArousalToggle));
            ModsGUI.NewLine(ModsGUI.HalfWidth(LifestyleCDToggle), ModsGUI.HalfWidth(RoomCleanlinessToggle));
            ModsGUI.NewLine(ModsGUI.HalfWidth(InfiniteTopicLimitToggle), ModsGUI.HalfWidth(InfiniteActionsToggle));

            ModsGUI.NewLine(ShowModHintToggle);
        }

        private void EditPlayerFocus()
        {
            GUILayout.Label("Focus (" + player.ins.getFocus() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerFocus(-5);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerFocus(5);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditPlayerStamina()
        {
            GUILayout.Label("Stamina (" + player.ins.getStamina() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerStamina(-5);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerStamina(5);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditPlayerHealth()
        {
            GUILayout.Label("Health (" + player.ins.getHealth() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerHealth(-5);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerHealth(5);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditPlayerArousal()
        {
            GUILayout.Label("Arousal (" + player.ins.getArousal() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerArousal(-5);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+5", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddPlayerArousal(5);
                Mods.Instance.RefreshButtons();
            }
        }

        private void MaxAllPlayerStats()
        {
            GUILayout.Label("Max focus, stamina and health (" + ModConfig.Instance.GetMaxPlayerStatsKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Max", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.MaxPlayerStats();
                Mods.Instance.RefreshButtons();
            }
        }

        private void AddPlayerStats()
        {
            if (ModsGUI.CMButton("Add charm", ModGUIStyles.LabelStyle))
            {
                player.ins.advCharm();
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("Add physique", ModGUIStyles.LabelStyle))
            {
                player.ins.advPhysique();
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("Add smarts", ModGUIStyles.LabelStyle))
            {
                player.ins.advSmarts();
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditPlayerMoney()
        {
            GUILayout.Label("Money (Current: " + player.ins.getMoney() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-100", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddMoneyToPlayer(-100);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+100", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddMoneyToPlayer(100);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+10000", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.AddMoneyToPlayer(10000);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditPlayerSkillsXp()
        {
            playerSkillFilter = ModsGUI.SearchTextField(playerSkillFilter);
            List<Skill> filteredPlayerSkills = Mods.Instance.GetPlayerSkills().FindAll(s => s.Moniker.ToLower().Contains(playerSkillFilter.ToLower()));
            playerSkillSelected = Mathf.Clamp(playerSkillSelected, 0, filteredPlayerSkills.Count - 1);
            GUILayout.Label("Skill xp: " + (filteredPlayerSkills.IsNullOrEmpty() ? "none" : filteredPlayerSkills[playerSkillSelected].Moniker + " (Current: " + filteredPlayerSkills[playerSkillSelected].XP.ToString()) + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle, clearFocus: false))
                playerSkillSelected = Mathf.Clamp(playerSkillSelected - 1, 0, filteredPlayerSkills.Count - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle, clearFocus: false))
                playerSkillSelected = Mathf.Clamp(playerSkillSelected + 1, 0, filteredPlayerSkills.Count - 1);
            if (ModsGUI.CMButton("+10", ModGUIStyles.BtnStyle) && !filteredPlayerSkills.IsNullOrEmpty())
            {
                Mods.Instance.AddPlayerSkill(filteredPlayerSkills[playerSkillSelected], 10);
                Mods.Instance.RefreshButtons();
            }
        }

        private void GetAllPlayerSkillsXp()
        {
            GUILayout.Label("Add all player skills exp", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("+100", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddAllPlayerSkills(100);
                Mods.Instance.RefreshButtons();
            }
        }

        private void TimeMultiplierSelector()
        {
            GUILayout.Label("Time multiplier (Hold " + ModConfig.Instance.GetFastForwardKey() + " for additional x" + Mods.FastForwardMultiplier + "): x" + ModConfig.Instance.GetTimeMultiplier(), ModGUIStyles.LabelStyle);
            ModConfig.Instance.SetTimeMultiplier((int)GUILayout.HorizontalSlider(ModConfig.Instance.GetTimeMultiplier(), ModConfig.Instance.GetTimeMultiplierMinValue(), ModConfig.Instance.GetTimeMultiplierMaxValue(), ModGUIStyles.SliderStyle, ModGUIStyles.SliderThumbStyle));
        }

        private void PlayerXpMultiplierSelector()
        {
            GUILayout.Label("Player base xp multiplier: x" + ModConfig.Instance.GetPlayerXpMultiplier(), ModGUIStyles.LabelStyle);
            ModConfig.Instance.SetPlayerXpMultiplier((int)GUILayout.HorizontalSlider(ModConfig.Instance.GetPlayerXpMultiplier(), ModConfig.Instance.GetPlayerXpMultiplierMinValue(), ModConfig.Instance.GetPlayerXpMultiplierMaxValue(), ModGUIStyles.SliderStyle, ModGUIStyles.SliderThumbStyle));
        }

        private void NpcXpMultiplierSelector()
        {
            GUILayout.Label("Other characters base xp multiplier: x" + ModConfig.Instance.GetNpcXpMultiplier(), ModGUIStyles.LabelStyle);
            ModConfig.Instance.SetNpcXpMultiplier((int)GUILayout.HorizontalSlider(ModConfig.Instance.GetNpcXpMultiplier(), ModConfig.Instance.GetNpcXpMultiplierMinValue(), ModConfig.Instance.GetNpcXpMultiplierMaxValue(), ModGUIStyles.SliderStyle, ModGUIStyles.SliderThumbStyle));
        }

        private void ResetDailyActions()
        {
            GUILayout.Label("Reset actions (" + ModConfig.Instance.GetResetActionsKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Reset", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.ResetActions();
                Mods.Instance.RefreshButtons();
            }
        }

        private void MoveToPlayerRoom()
        {
            GUILayout.Label("Move to " + gamemanager.ins.LM.GetLocation(Location_list.Your_Room).LocaleName, ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Move", ModGUIStyles.WideBtnStyle))
                Mods.Instance.Bailout();
        }

        private void UnlockLocations(List<Location_Script> hiddenLocations)
        {
            GUILayout.Label("Unlock location: " + (hiddenLocations.IsNullOrEmpty() ? "none" : hiddenLocations.ElementAt(lockedLocationSelected).LocaleName), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle))
                lockedLocationSelected = Mathf.Clamp(lockedLocationSelected - 1, 0, hiddenLocations.Count - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle))
                lockedLocationSelected = Mathf.Clamp(lockedLocationSelected + 1, 0, hiddenLocations.Count - 1);
            if (ModsGUI.CMButton("Unlock", ModGUIStyles.WideBtnStyle) && !hiddenLocations.IsNullOrEmpty())
            {
                Mods.Instance.UnlockLocation(hiddenLocations.ElementAt(lockedLocationSelected));
                Mods.Instance.RefreshScene();
            }
        }

        private void ChangeTime()
        {
            GUILayout.Label("Change time", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-1h (" + ModConfig.Instance.GetTimeMinusKey() + ")", ModGUIStyles.WideBtnStyle))
                Mods.Instance.SkipHours(-1);
            if (ModsGUI.CMButton("+1h (" + ModConfig.Instance.GetTimePlusKey() + ")", ModGUIStyles.WideBtnStyle))
                Mods.Instance.SkipHours(1);
            if (ModsGUI.CMButton("+24h (" + ModConfig.Instance.GetSkip24HoursKey() + ")", ModGUIStyles.WideBtnStyle))
                Mods.Instance.SkipHours(24);
        }

        private void RoomCleaning()
        {
            GUILayout.Label("Room cleanliness (Current: " + (gamemanager.ins.getCurrentLocation() == null || !gamemanager.ins.getCurrentLocation().cleanable ? "none" : (100 - gamemanager.ins.getCurrentLocation().dirtyness).ToString()) + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-20", ModGUIStyles.BtnStyle))
                if (Mods.Instance.AddRoomCleanliness(gamemanager.ins.getCurrentLocation(), -20))
                    Mods.Instance.RefreshScene();
            if (ModsGUI.CMButton("+20", ModGUIStyles.BtnStyle))
                if (Mods.Instance.AddRoomCleanliness(gamemanager.ins.getCurrentLocation(), 20))
                    Mods.Instance.RefreshScene();
        }

        private void SpawnSelectedTopic(List<topics> filteredSkills)
        {

            topicsFilter = ModsGUI.SearchTextField(topicsFilter);
            GUILayout.Label("Topic or item: " + (filteredSkills.IsNullOrEmpty() ? "none" : filteredSkills[topicSelected].Name), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle, clearFocus: false))
                topicSelected = Mathf.Clamp(topicSelected - 1, 0, filteredSkills.Count - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle, clearFocus: false))
                topicSelected = Mathf.Clamp(topicSelected + 1, 0, filteredSkills.Count - 1);
            if (ModsGUI.CMButton("Spawn", ModGUIStyles.BtnStyle) && !filteredSkills.IsNullOrEmpty())
            {
                Mods.Instance.GainTopic(filteredSkills[topicSelected]);
                Mods.Instance.RefreshButtons();
            }
        }

        private void SelectedTopicTags(List<topics> filteredSkills)
        {
            GUILayout.Label("Selected topic or item tags: " + (filteredSkills.IsNullOrEmpty() || filteredSkills[topicSelected].associatedTags.IsNullOrEmpty() ? "none" : String.Join(", ", (filteredSkills[topicSelected].associatedTags).Select(t => t.name).ToArray())), ModGUIStyles.LabelStyle);
        }

        private void SpawnTopicBySelectedTag()
        {
            tagsFilter = ModsGUI.SearchTextField(tagsFilter);
            List<dictionary_Tag> filteredTags = Mods.Instance.GetAllDictionaryTags().FindAll(t => t.associated_TAG.name.ToLower().Contains(tagsFilter.ToLower()));
            tagSelected = Mathf.Clamp(tagSelected, 0, filteredTags.Count - 1);
            GUILayout.Label("Tag for random topic: " + (filteredTags.IsNullOrEmpty() ? "none" : filteredTags[tagSelected].associated_TAG.name), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle, clearFocus: false))
                tagSelected = Mathf.Clamp(tagSelected - 1, 0, filteredTags.Count - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle, clearFocus: false))
                tagSelected = Mathf.Clamp(tagSelected + 1, 0, filteredTags.Count - 1);
            if (ModsGUI.CMButton("Spawn", ModGUIStyles.BtnStyle) && !filteredTags.IsNullOrEmpty())
            {
                Mods.Instance.GetRandomTempTopicWithTag(filteredTags[tagSelected]);
                Mods.Instance.RefreshButtons();
            }
        }

        private void InfiniteStatsToggle()
        {
            GUILayout.Label("Infinite stats (" + ModConfig.Instance.GetToggleInfiniteStatsKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleInfiniteStats().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleInfiniteStats();
        }

        private void InfiniteArousalToggle()
        {
            GUILayout.Label("Infinite arousal (" + ModConfig.Instance.GetToggleInfiniteArousalKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleInfiniteArousal().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleInfiniteArousal();
        }

        private void LifestyleCDToggle()
        {
            GUILayout.Label("Remove lifestyle cooldown (" + ModConfig.Instance.GetToggleResetLifestyleCDKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleResetLifestyleCD().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleResetLifestyleCD();
        }

        private void RoomCleanlinessToggle()
        {
            GUILayout.Label("Infinite room cleanliness (" + ModConfig.Instance.GetToggleInfiniteCleanlinessKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleInfiniteCleanliness().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleInfiniteCleanliness();
        }

        private void InfiniteTopicLimitToggle()
        {
            GUILayout.Label("Infinite topic limit (" + ModConfig.Instance.GetToggleInfiniteTopicsKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleInfiniteTopics().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleInfiniteTopics();
        }

        private void InfiniteActionsToggle()
        {
            GUILayout.Label("Toggle infinite actions (" + ModConfig.Instance.GetToggleInfiniteActionsKey() + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleInfiniteActions().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleInfiniteActions();
        }

        private void ShowModHintToggle()
        {
            GUILayout.Label("Show 'open cheatmod' hint when closed", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton(ModConfig.Instance.IsToggleShowOpenModHint().ToString(), ModGUIStyles.BtnStyle))
                ModConfig.Instance.ToggleShowOpenModHint();
        }

    }
}
