using System.Collections.Generic;

using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace YetAnotherStockpilePresets
{
    [StaticConstructorOnStartup]
    public class Command_StorageSettingsPresets : Command
    {
        static readonly Texture2D StoragePresetsTex = ContentFinder<Texture2D>.Get("UI/Commands/StorageSettingsPresets", true);

        static Command_StorageSettingsPresets() {
            new Harmony("rimworld.notfood.yasp").PatchAll();
        }

        readonly Zone_Stockpile stockpile;

        public Command_StorageSettingsPresets(Zone_Stockpile stockpile)
        {
            icon = StoragePresetsTex;

            defaultLabel = ResourceBank.Label;
            defaultDesc = ResourceBank.Description;

            this.stockpile = stockpile;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var list = new List<FloatMenuOption>();
            foreach (var def in DefDatabase<StockpilePresetDef>.AllDefs)
            {
                list.Add(new FloatMenuOption(def.label, delegate {
                    CopyIntoFilterFromTemplate(stockpile, def);
                }));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        internal static void CopyIntoFilterFromTemplate(Zone_Stockpile stockpile, StockpilePresetDef def)
        {
            stockpile.settings.filter.CopyAllowancesFrom(def.filter);
        }
    }

    [HarmonyPatch (typeof(Zone_Stockpile))]
    [HarmonyPatch (nameof(Zone_Stockpile.GetGizmos))]
    public static class Zone_Stockpile_GetGizmos_Patch
    {
        public static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            __result = __result.AddItem(new Command_StorageSettingsPresets(__instance));
        }
    }
}
