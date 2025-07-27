using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoserCheatMod
{
    // as the name implies, handles everything related to second page of the cheatmod
    // character specific cheats like editing relationship, interests, AT specific flags, etc. are found here
    internal class Page2GUI : MonoBehaviour, ModsGUI.IPageGUI
    {
        public static Page2GUI Instance;
        private Page2GUI() { }

        private int characterSelected;
        // recalculate some character specific variables when true
        private bool characterSelectionChanged = true;
        private int characterATSelected;
        private int characterSkillSelected;
        private int characterInterestSelected = -1;

        private string characterInterestsFilter = "";


        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        // due to MonoBehaviour, called on each frame from unity
        private void Update()
        {
            if (Mods.Instance.CheatModBlocked())
                return;
            HandleKeys();
        }

        public void HandleKeys()
        {
            if (ModsGUI.Instance.IsCurrentPage(Instance) && Input.GetKeyDown(ModConfig.Instance.GetPreviousCharacterKey()))
                SelectCharacter(-1);
            if (ModsGUI.Instance.IsCurrentPage(Instance) && Input.GetKeyDown(ModConfig.Instance.GetNextCharacterKey()))
                SelectCharacter(+1);
        }

        public string GetPageDescription()
        {
            return "Individual character cheats";
        }

        public void RenderPage()
        {
            character_Script character = characters.ins.Character_List[characterSelected];
            string[] possibleATs = GetPossibleATs(character);
            if (characterSelectionChanged)
            {
                characterATSelected = characterATSelected = Array.IndexOf(possibleATs, character.Archtype.name);
                characterSkillSelected = -1;
                characterSelectionChanged = false;
            }

            ModsGUI.NewLine(() => SelectCharacter(character));
            ModsGUI.NewLine(() => CharacterATSelector(character, possibleATs));

            ModsGUI.NewLine(() => EditCharacterStat(CharStatList.Relationship, character));
            ModsGUI.NewLine(() => EditCharacterStat(CharStatList.Lust, character));
            ModsGUI.NewLine(() => EditCharacterStat(CharStatList.Interest, character));
            ModsGUI.NewLine(() => EditCharacterStat(CharStatList.Assertiveness, character));
            ModsGUI.NewLine(() => EditCharacterStat(CharStatList.Intoxication, character));
            if (character.Archtype.showFollowers)
                ModsGUI.NewLine(() => EditCharacterStat(CharStatList.Followers, character));
            ModsGUI.NewLine(() => MaxCharacterStats(character));

            if (WeeklyCountRelevant(character))
                ModsGUI.NewLine(() => EditCharacterWeeklyCount(character));
            if (character.Archtype.UseATCounter1)
                ModsGUI.NewLine(() => EditATCounter1(character));
            if (character.Archtype.UseATCounter2)
                ModsGUI.NewLine(() => EditATCounter2(character));
            if (character.CharacterStats["AT_ChangeDelay"].Value > 0)
                ModsGUI.NewLine(() => EditATTransformationDelay(character));

            List<interestSO> filteredInterests = interests.ins.interestsSOList.FindAll(i => i.DisplayName.ToLower().Contains(characterInterestsFilter.ToLower()));
            characterInterestSelected = Mathf.Clamp(characterInterestSelected, (characterInterestsFilter.IsNullOrEmpty() || filteredInterests.IsNullOrEmpty() ? -1 : 0), filteredInterests.Count - 1);
            ModsGUI.NewLine(() => InterestSelector(filteredInterests));
            ModsGUI.NewLine(() => SetInterest1(character, filteredInterests));
            ModsGUI.NewLine(() => SetInterest2(character, filteredInterests));

            List<tags> relevantSkills = Mods.Instance.GetRelevantCharacterSkills(character);
            characterSkillSelected = Mathf.Clamp(characterSkillSelected, 0, relevantSkills.Count - 1);
            if (!relevantSkills.IsNullOrEmpty())
                ModsGUI.NewLine(() => EditCharacterPreferences(character, relevantSkills));

            int flagsCounter = 0;
            foreach (CharFlagList charFlag in Mods.Instance.GetRelevantCharacterFlags())
            {
                if (character.getFlag(charFlag, character.Assigned))
                {
                    ModsGUI.NewLine(() => EditCharacterFlag(charFlag, character));
                    flagsCounter++;
                }
            }
            if (flagsCounter > 1)
                ModsGUI.NewLine(() => ResetFlags(character));

            if (character.CharacterStats["Unlocked"].Value == 0)
                ModsGUI.NewLine(() => UnlockCharacter(character));
        }

        private void SelectCharacter(int step)
        {
            if ((step < 0 && characterSelected > 0) || (step > 0 && characterSelected < characters.ins.Character_List.Count - 1))
            {
                characterSelected += step;
                characterSelectionChanged = true;
            }
        }

        private void SelectCharacter(character_Script character)
        {
            GUILayout.Label("Character selected: " + character.CharacterName + " (" + character.Archtype.name + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<< (" + ModConfig.Instance.GetPreviousCharacterKey() + ")", ModGUIStyles.WideBtnStyle))
                SelectCharacter(-1);
            if (ModsGUI.CMButton(">> (" + ModConfig.Instance.GetNextCharacterKey() + ")", ModGUIStyles.WideBtnStyle))
                SelectCharacter(+1);
        }

        private void CharacterATSelector(character_Script character, string[] possibleATs)
        {
            GUILayout.Label("Transform to AT: " + (characterATSelected < 0 ? "none" : possibleATs[characterATSelected]), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle) && possibleATs.Length > 1)
                characterATSelected = Mathf.Clamp(characterATSelected - 1, 0, possibleATs.Length - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle) && possibleATs.Length > 1)
                characterATSelected = Mathf.Clamp(characterATSelected + 1, 0, possibleATs.Length - 1);
            if (ModsGUI.CMButton("Transform", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.TransformCharacter(character, possibleATs[characterATSelected]);
                Mods.Instance.RefreshScene();
            }
        }

        private void EditCharacterStat(CharStatList stat, character_Script character, int step = 10)
        {
            GUILayout.Label(stat + " (Current: " + character.getStat(stat) + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-" + step, ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, stat, -step);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+" + step, ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, stat, step);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditCharacterFlag(CharFlagList flag, character_Script character)
        {
            GUILayout.Label(flag.ToString(), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Reset flag", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.SetCharacterFlag(character, flag, false);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditCharacterWeeklyCount(character_Script character)
        {
            int step = 1;
            GUILayout.Label(GetWeeklyCountLabel(character) + " (Current: " + character.getStat(CharStatList.weekly_count) + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-" + step, ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.weekly_count, -step);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+" + step, ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.weekly_count, step);
                Mods.Instance.RefreshButtons();
            }
        }

        // refactor this when character.Archtype.UseWeeklyATCounter and character.Archtype.UseWeeklyATCounterMessage are added
        private bool WeeklyCountRelevant(character_Script character)
        {
            return CharacterList.Becky == character.Assigned;
        }

        private string GetWeeklyCountLabel(character_Script character)
        {
            if (CharacterList.Becky == character.Assigned)
                return "Homework";
            return CharStatList.weekly_count.ToString();
        }


        private void MaxCharacterStats(character_Script character)
        {
            GUILayout.Label(string.Format("Max {0}, {1}, {2}, {3}", CharStatList.Relationship, CharStatList.Interest, CharStatList.Assertiveness, CharStatList.Intoxication) + (character.Archtype.showFollowers ? ", " + CharStatList.Followers : ""), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Max", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.MaxCharacterStats(character);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditATCounter1(character_Script character)
        {
            GUILayout.Label((character.Archtype.UseATCounter1 ? character.Archtype.ATCounter1Message : "AT_Counter1Manual") + " (Current: " + character.getStat(CharStatList.AT_Counter1Manual) + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-1", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter1Manual, -1);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+1", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter1Manual, 1);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("-10", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter1Manual, -10);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+10", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter1Manual, 10);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditATCounter2(character_Script character)
        {
            GUILayout.Label((character.Archtype.UseATCounter2 ? character.Archtype.ATCounter2Message : "AT_Counter2Manual") + " (Current: " + character.getStat(CharStatList.AT_Counter2Manual) + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-1", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter2Manual, -1);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+1", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter2Manual, 1);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("-10", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter2Manual, -10);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("+10", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.AT_Counter2Manual, 10);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditATTransformationDelay(character_Script character)
        {
            GUILayout.Label("AT transformation delay" + " (Current: " + character.CharacterStats["AT_ChangeDelay"].Value + ")", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("-1", ModGUIStyles.BtnStyle))
            {
                character.CharacterStats["AT_ChangeDelay"].Value -= 1;
                if (character.CharacterStats["AT_ChangeDelay"].Value <= 0)
                    Mods.Instance.RefreshScene();
            }
            if (ModsGUI.CMButton("+1", ModGUIStyles.BtnStyle))
                character.CharacterStats["AT_ChangeDelay"].Value += 1;
        }

        private void InterestSelector(List<interestSO> filteredInterests)
        {
            characterInterestsFilter = ModsGUI.SearchTextField(characterInterestsFilter);
            GUILayout.Label("Selected interest: " + (filteredInterests.IsNullOrEmpty() || characterInterestSelected == -1 ? "none" : filteredInterests[characterInterestSelected].DisplayName), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle, clearFocus: false))
                characterInterestSelected = Mathf.Clamp(characterInterestSelected - 1, -1, filteredInterests.Count - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle, clearFocus: false))
                characterInterestSelected = Mathf.Clamp(characterInterestSelected + 1, -1, filteredInterests.Count - 1);
        }

        private void SetInterest1(character_Script character, List<interestSO> filteredInterests)
        {
            GUILayout.Label("Interest 1: " + (character.interest1 == null ? "none" : character.interest1.DisplayName), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Set selected", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.interest1, interest: filteredInterests.IsNullOrEmpty() || characterInterestSelected == -1 ? null : filteredInterests[characterInterestSelected]);
                Mods.Instance.RefreshButtons();
            }
        }

        private void SetInterest2(character_Script character, List<interestSO> filteredInterests)
        {
            GUILayout.Label("Interest 2: " + (character.interest2 == null ? "none" : character.interest2.DisplayName), ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Set selected", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.interest2, interest: filteredInterests.IsNullOrEmpty() || characterInterestSelected == -1 ? null : filteredInterests[characterInterestSelected]);
                Mods.Instance.RefreshButtons();
            }
        }

        private void EditCharacterPreferences(character_Script character, List<tags> relevantSkills)
        {
            GUILayout.Label("Topic: " + relevantSkills[characterSkillSelected].associatedXP.Moniker + " (Current: " + character.preferences[relevantSkills[characterSkillSelected].associatedXP.Moniker] + "%)", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("<<", ModGUIStyles.BtnStyle))
                characterSkillSelected = Mathf.Clamp(characterSkillSelected - 1, 0, relevantSkills.Count - 1);
            if (ModsGUI.CMButton(">>", ModGUIStyles.BtnStyle))
                characterSkillSelected = Mathf.Clamp(characterSkillSelected + 1, 0, relevantSkills.Count - 1);
            if (ModsGUI.CMButton("+10", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.AddCharacterStat(character, CharStatList.Preference, 10, tag: relevantSkills[characterSkillSelected]);
                Mods.Instance.RefreshButtons();
            }
            if (ModsGUI.CMButton("Max all", ModGUIStyles.BtnStyle))
            {
                Mods.Instance.MaxCharacterExp(character);
                Mods.Instance.RefreshButtons();
            }
        }

        private void ResetFlags(character_Script character)
        {
            GUILayout.Label("Reset flags (massage, photoshoot, homework etc.)", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Reset", ModGUIStyles.WideBtnStyle))
            {
                Mods.Instance.ResetNpcCharacterFlags(character);
                Mods.Instance.RefreshButtons();
            }
        }

        private void UnlockCharacter(character_Script character)
        {
            GUILayout.Label("Unlock character", ModGUIStyles.LabelStyle);
            if (ModsGUI.CMButton("Unlock", ModGUIStyles.WideBtnStyle))
            {
                character.CharacterStats["Unlocked"].Value = 1;
                Mods.Instance.RefreshScene();
            }
        }

        // Unify AT handling from enum to string
        private string[] GetPossibleATs(character_Script character)
        {
            if (character.Assigned == CharacterList.Becky)
                return Enum.GetNames(typeof(BeckyATs));

            if (character.Assigned == CharacterList.Charlie)
                return Enum.GetNames(typeof(CharlieATs));

            if (character.Assigned == CharacterList.Diane)
                return Enum.GetNames(typeof(DianeATs));

            if (character.Assigned == CharacterList.Gina)
                return Enum.GetNames(typeof(GinaATs));

            if (character.Assigned == CharacterList.Jay)
                return Enum.GetNames(typeof(JayATs));

            if (character.Assigned == CharacterList.Jesse)
                return Enum.GetNames(typeof(JesseATs));

            if (character.Assigned == CharacterList.Lisa)
                return Enum.GetNames(typeof(LisaATs));

            if (character.Assigned == CharacterList.Morgan)
                return Enum.GetNames(typeof(MorganATs));

            if (character.Assigned == CharacterList.Richard)
                return Enum.GetNames(typeof(RichardATs));

            if (character.Assigned == CharacterList.Sally)
                return Enum.GetNames(typeof(SallyATs));

            if (character.Assigned == CharacterList.Tilley)
                return Enum.GetNames(typeof(TilleyATs));

            return [];
        }
    }
}
