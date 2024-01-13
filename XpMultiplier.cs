using HarmonyLib;
using System;
using UnityEngine;

namespace LoserCheatMod
{
    [HarmonyPatch(typeof(character_Script))]
    [HarmonyPatch(nameof(character_Script.gainPreference), new Type[] { typeof(tags), typeof(int), typeof(bool) })]
    class XpMultiplierNPC
    {
        static void Prefix(tags tag, ref int num, bool inConvo)
        {
            num *= Mods.Instance.getNPCXpMultiplier();
        }
    }

    [HarmonyPatch(typeof(PlayerSkills))]
    [HarmonyPatch(nameof(PlayerSkills.GainXP), new Type[] { typeof(Skill), typeof(int), typeof(bool) })]
    class XpMultiplierPlayer1
    {
        static void Prefix(Skill skill, ref int amount, bool inConvo)
        {
            amount *= Mods.Instance.getPlayerXpMultiplier();
            amount = Mathf.Clamp(amount, 0, 100);
        }
    }

    [HarmonyPatch(typeof(PlayerSkills))]
    [HarmonyPatch(nameof(PlayerSkills.gainPassiveXP), new Type[] { typeof(Skill), typeof(int) })]
    class XpMultiplierPlayer2
    {
        static void Prefix(Skill skill, ref int amount)
        {
            amount *= Mods.Instance.getPlayerXpMultiplier();
            amount = Mathf.Clamp(amount, 0, 100);
        }
    }
    
}
