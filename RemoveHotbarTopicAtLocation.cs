using HarmonyLib;



namespace LoserCheatMod
{

    [HarmonyPatch(typeof(interactionUI))]
    [HarmonyPatch("Start")]
    class StartConversation
    {
        static void Postfix(interactionUI __instance)
        {
            Mods.Instance.ClearRememberedPermTopics();
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.RemoveHotbarTopicAtLocation))]
    class RemoveHotbarTopicAtLocation
    {
        static bool Prefix(Inventory __instance, int location)
        {
            if (Mods.Instance.IsKeepPermTopics() && __instance.getTopic(__instance.HotbarTopics[location]).isPerm())
                Mods.Instance.RememberTopicLocation(location, __instance.getTopic(__instance.HotbarTopics[location]));
            return true;
        }
    }

    [HarmonyPatch(typeof(interactionUI))]
    [HarmonyPatch("FinishCleanup")]
    class EndConversation
    {
        static void Postfix(interactionUI __instance)
        {
            if (Mods.Instance.IsKeepPermTopics())
                Mods.Instance.RestorePermTopics();
        }
    }

}
