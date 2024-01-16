using HarmonyLib;
using System.Collections.Generic;
using BepInEx;

namespace LoserCheatMod
{
    class InventoryHelperMethods
    {
        public static bool _hasOwnedTopicByTag(Inventory inventory, tags tag) {
            dictionary_Tag dict = inventory.getDictTag(tag);
            // Mods.Log.LogInfo($"Looking in the inventory for tag {tag.name}. Associated dict has {dict.TopicsWithTag.Count} items.");
            // foreach (topics topic in dict.TopicsWithTag) {
            //     Mods.Log.LogInfo($"Dict has topic {topic.Name} {topic.ID}.");
            // }
            foreach (owned_topic owned_topic in inventory.TransientTopics)
            {
                // Mods.Log.LogInfo($"Looking for topic {owned_topic.Topic_id} in there...");
                if (dict.TopicsWithTag.Contains(inventory.getTopic(owned_topic.Topic_id)))
                {
                    return true;
                }
            }
            foreach (owned_topic owned_topic2 in inventory.ItemTopics)
            {
                // Mods.Log.LogInfo($"Looking for item {owned_topic2.Topic_id} in there...");
                if (dict.TopicsWithTag.Contains(inventory.getTopic(owned_topic2.Topic_id)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.hasHotbarTopicWithTag))]
    class hasHotbarTopicWithTag
    {
        static void Postfix(Inventory __instance, tags tag, ref bool __result)
        {
            if (__result == true) {
                return;
            }
            if (Mods.Instance.IsHotbarChecksCanSearchInventory()) {
                __result = InventoryHelperMethods._hasOwnedTopicByTag(__instance, tag);
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.hasItembarTopicWithTag))]
    class hasItembarTopicWithTag
    {
        static void Postfix(Inventory __instance, tags tag, ref bool __result)
        {
            if (__result == true) {
                return;
            }
            if (Mods.Instance.IsHotbarChecksCanSearchInventory()) {
                __result = InventoryHelperMethods._hasOwnedTopicByTag(__instance, tag);
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.hasHotbarTopic))]
    class hasHotbarTopic
    {
        static void Postfix(Inventory __instance, int topic, ref bool __result)
        {
            if (__result == true) {
                return;
            }
            if (Mods.Instance.IsHotbarChecksCanSearchInventory()) {
                // Mods.Log.LogInfo($"Looking in the inventory for {topic} {__instance.TopicIDReference[topic].Name}.");
                __result = __instance.hasTransientTopic(topic) || __instance.hasPermTopic(topic);
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.hasItembarTopic))]
    class hasItembarTopic
    {
        static void Postfix(Inventory __instance, int topic, ref bool __result)
        {
            if (__result == true) {
                return;
            }
            if (Mods.Instance.IsHotbarChecksCanSearchInventory()) {
                // Outfits are meant to be removed from the inventory forever
                // once given. It would be tough to patch everything to ensure
                // that outfits get removed when they aren't in the itembar,
                // so the workaround is to just require outfits to be in the itembar.
                foreach (topics outfit_topic in __instance.Outfit_TopicList)
                {
                    // Mods.Log.LogInfo($"Checking whether topic {topic} {__instance.TopicIDReference[topic].Name} is outfit {outfit_topic.ID} {__instance.TopicIDReference[outfit_topic.ID].Name}");
                    if (outfit_topic.ID == topic) {
                        __result = false;
                        return;
                    }
                }
                __result = __instance.hasItemTopic(topic);
            }
        }
    }
}
