using Verse;

namespace YetAnotherStockpilePresets
{
    [StaticConstructorOnStartup]
    public static class ResourceBank
    {
        static string TL(string s) => s.Translate();

        public static readonly string Label = TL("CommandStorageSettingsPresetsLabel");
        public static readonly string Description = TL("CommandStorageSettingsPresetsDesc");
    }
}
