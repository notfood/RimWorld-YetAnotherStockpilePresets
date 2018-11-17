using Verse;

namespace YetAnotherStockpilePresets
{
    public class StockpilePresetDef : Def
    {
        public ThingFilter filter;

        public override void ResolveReferences()
        {
            if (filter == null) {
                Log.Error("StockpilePresets :: " + defName + " has null filter");
                return;
            }
            filter.ResolveReferences();
        }
    }
}
