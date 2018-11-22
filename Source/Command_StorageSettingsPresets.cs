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

        static Command_StorageSettingsPresets() {
            HarmonyInstance.Create("rimworld.notfood.yasp").PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        readonly ThingFilter filter;

        public Command_StorageSettingsPresets(Zone_Stockpile stockpile)
        {
            icon = StoragePresetsTex;

            defaultLabel = ResourceBank.Label;
            defaultDesc = ResourceBank.Description;

            filter = stockpile.settings.filter;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var list = new List<FloatMenuOption>();
            foreach (var def in DefDatabase<StockpilePresetDef>.AllDefs)
            {
                list.Add(new FloatMenuOption(def.label, delegate {
                    filter.CopyAllowancesFrom(def.filter);
                }));
            }
            Find.WindowStack.Add(new FloatMenu(list));
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
