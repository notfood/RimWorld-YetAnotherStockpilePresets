using System.Collections.Generic;

using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace YetAnotherStockpilePresets
{
    [StaticConstructorOnStartup]
    public class Command_StorageSettingsPresets : Command
    {
        static readonly Texture2D StoragePresetsTex = ContentFinder<Texture2D>.Get("UI/Commands/StorageSettingsPresets", true);
        static readonly Dictionary<string, ThingFilter> dictionary;

        static Command_StorageSettingsPresets() {
            
            dictionary = new Dictionary<string, ThingFilter>();
            foreach(var def in DefDatabase<StockpilePresetDef>.AllDefs) {
                dictionary.Add(def.label, def.filter);
            }

            HarmonyInstance.Create("rimworld.notfood.yasp").PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        readonly FloatMenu menu;

        public Command_StorageSettingsPresets(Zone_Stockpile stockpile)
        {
            icon = StoragePresetsTex;

            defaultLabel = ResourceBank.Label;
            defaultDesc = ResourceBank.Description;

            menu = BuildFloatMenu(stockpile.settings.filter);
        }

        static FloatMenu BuildFloatMenu(ThingFilter filter) {
            var list = new List<FloatMenuOption>();
            foreach (var key in dictionary.Keys) {
                list.Add(new FloatMenuOption(key, delegate {
                    filter.CopyAllowancesFrom(dictionary[key]);
                }));
            }
            return new FloatMenu(list);
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            Find.WindowStack.Add(menu);
        }
    }

    [HarmonyPatch (typeof(Zone_Stockpile))]
    [HarmonyPatch (nameof(Zone_Stockpile.GetGizmos))]
    public class Zone_Stockpile_GetGizmos_Patch
    {
        public static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            __result = __result.Add(new Command_StorageSettingsPresets(__instance));
        }
    }
}
