using System;
using Multiplayer.API;
using Verse;

namespace YetAnotherStockpilePresets
{
    [StaticConstructorOnStartup]
    static class Multiplayer
    {
        static Multiplayer()
        {
            if (!MP.enabled) return;

            MP.RegisterSyncMethod(typeof(Command_StorageSettingsPresets), nameof(Command_StorageSettingsPresets.CopyIntoFilterFromTemplate));
        }
    }
}
