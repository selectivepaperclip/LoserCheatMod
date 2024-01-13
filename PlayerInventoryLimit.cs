using HarmonyLib;


namespace LoserCheatMod
{
    [HarmonyPatch(typeof(player))]
    [HarmonyPatch(nameof(player.GetInventoryBonus))]
    class PlayerInventoryLimit
    {
        static void Postfix(ref int __result)
        {
            if (Mods.Instance.IsInfiniteTopics())
                // 50 is baseline limit, this gets added to it (just for a nicer number)
                __result = 99999 - 50;
        }

    }
}
