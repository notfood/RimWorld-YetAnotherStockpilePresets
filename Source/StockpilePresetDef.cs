using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace YetAnotherStockpilePresets
{
    public class StockpilePresetDef : Def
    {
        public ThingFilter filter = new ThingFilter();
        public bool filterPerishables;

        public override void ResolveReferences()
        {
            filter.ResolveReferences();
            if (filterPerishables) {
                foreach (var def in filter.AllowedThingDefs.ToList())
                {
                    if (def.EverHaulable
                        && def.CanEverDeteriorate
                        && def.GetStatValueAbstract(StatDefOf.DeteriorationRate) > 0f) {
                        continue;
                    }
                    filter.SetAllow(def, false);
                }
            }
        }
    }
}
