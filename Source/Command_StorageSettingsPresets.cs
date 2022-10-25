using System.Collections.Generic;
using System.Reflection;
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

        readonly IStoreSettingsParent target;

        public Command_StorageSettingsPresets(IStoreSettingsParent target)
        {
            icon = StoragePresetsTex;

            defaultLabel = ResourceBank.Label;
            defaultDesc = ResourceBank.Description;

            this.target = target;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var list = new List<FloatMenuOption>();
            foreach (var def in DefDatabase<StockpilePresetDef>.AllDefs)
            {
                list.Add(new FloatMenuOption(def.label, delegate {
                    CopyIntoFilterFromTemplate(target, def);
                }));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        internal static void CopyIntoFilterFromTemplate(IStoreSettingsParent target, StockpilePresetDef def)
        {
            target.GetStoreSettings().filter.CopyAllowancesFrom(def.filter);
        }
    }

    [HarmonyPatch]
    public static class ISelectable_GetGizmos_Patch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Zone_Stockpile), nameof(ISelectable.GetGizmos));
            yield return AccessTools.Method(typeof(Building_Storage), nameof(ISelectable.GetGizmos));
        }

        public static void Postfix(IStoreSettingsParent __instance, ref IEnumerable<Gizmo> __result)
        {
            __result = __result.AddItem(new Command_StorageSettingsPresets(__instance));
        }
    }
}
