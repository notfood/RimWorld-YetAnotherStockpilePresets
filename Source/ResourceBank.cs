using Verse;

namespace YetAnotherStockpilePresets
{
    public static class ResourceBank
    {
        static string TL(string s) => s.Translate();

        public static readonly string Label = TL("CommandStorageSettingsPresetsLabel");
        public static readonly string Description = TL("CommandStorageSettingsPresetsDesc");
    }
}
