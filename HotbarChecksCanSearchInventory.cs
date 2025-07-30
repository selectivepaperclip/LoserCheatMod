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

    [HarmonyPatch(typeof(Reqs.topicReq), "processByTag")]
    class processByTag
    {
        static void Postfix(Reqs.topicReq __instance, Inventory inv, Reqs.requirements reqs, ref bool __result)
        {
            if (__result)
            {
                return;
            }

            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory() && !__instance.invert)
            {
                __result = InventoryHelperMethods._hasOwnedTopicByTag(inv, __instance.tag);
            }
        }

    }

    // hasHotbarTopicWithTag and hasItembarTopicWithTag currently return an int instead of a bool,
    // with the intent of destroying whatever they find afterward. The patched versions have to return
    // *some* non-neg1 int to be considered truthy, so I'm returning -2 and attempting to stop
    // any consequence of downstream code blowing up using that value in methods such as
    // removeTopicFromBarByTags by stubbing those methods too (in this case, to return false)
    // This has the consequence that more to
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.hasHotbarTopicWithTag))]
    class hasHotbarTopicWithTag
    {
        static void Postfix(Inventory __instance, tags tag, ref int __result)
        {
            if (__result != -1) {
                return;
            }
            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory()) {
                __result = InventoryHelperMethods._hasOwnedTopicByTag(__instance, tag) ? -2 : -1;
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.hasItembarTopicWithTag))]
    class hasItembarTopicWithTag
    {
        static void Postfix(Inventory __instance, tags tag, ref int __result)
        {
            if (__result != -1) {
                return;
            }
            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory()) {
                __result = InventoryHelperMethods._hasOwnedTopicByTag(__instance, tag) ? -2 : -1;
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.removeTopicFromBarByTags))]
    class removeTopicFromBarByTags
    {
        static bool Prefix(List<tags> _tags, topics.InventoryModReason Reason)
        {
            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory())
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.removeTopicFromBarByTag))]
    class removeTopicFromBarByTag
    {
        static bool Prefix(tags tag, topics.InventoryModReason Reason)
        {
            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory())
            {
                return false;
            }
            return true;
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
            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory()) {
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
            if (ModConfig.Instance.IsToggleHotbarChecksCanSearchInventory()) {
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
