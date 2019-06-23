using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace YetAnotherStockpilePresets
{
    [StaticConstructorOnStartup]
    public class Dialog_ManagePresets : Window
    {
        static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        static Regex validNameRegex = new Regex("^[\\p{L}0-9 '\\-]*$");
        static ThingFilter globalFilter = new ThingFilter();

        public override Vector2 InitialSize => new Vector2(700f, 700f);


        public Dialog_ManagePresets() {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float num = 0f;
            Rect rect = new Rect(0f, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect, "SelectPreset".Translate(), true, false, true))
            {
                var list = new List<FloatMenuOption>();
                foreach (var preset in DefDatabase<StockpilePresetDef>.AllDefs)
                {
                    list.Add(new FloatMenuOption(preset.label, delegate {
                        globalFilter.CopyAllowancesFrom(preset.filter);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            num += 10f;
            Rect rect2 = new Rect(num, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect2, "NewPreset".Translate(), true, false, true))
            {
                globalFilter.SetAllowAll(null);
            }
            num += 10f;
            Rect rect3 = new Rect(num, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect3, "DeletePreset".Translate(), true, false, true))
            {
                List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                foreach (var preset in DefDatabase<StockpilePresetDef>.AllDefs)
                {
                    list2.Add(new FloatMenuOption(preset.label, delegate {
                        AcceptanceReport acceptanceReport = Current.Game.outfitDatabase.TryDelete(localOut);
                        if (!acceptanceReport.Accepted)
                        {
                            Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
                        }
                        else if (localOut == this.SelectedOutfit)
                        {
                            this.SelectedOutfit = null;
                        }
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list2));
            }
            Rect rect4 = new Rect(0f, 40f, inRect.width, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
            if (this.SelectedOutfit == null)
            {
                GUI.color = Color.grey;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, "NoOutfitSelected".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
                return;
            }
            GUI.BeginGroup(rect4);
            Rect rect5 = new Rect(0f, 0f, 200f, 30f);
            Dialog_ManageOutfits.DoNameInputRect(rect5, ref this.SelectedOutfit.label);
            Rect rect6 = new Rect(0f, 40f, 300f, rect4.height - 45f - 10f);
            Rect rect7 = rect6;
            ThingFilter filter = this.SelectedOutfit.filter;
            ThingFilter parentFilter = Dialog_ManageOutfits.apparelGlobalFilter;
            int openMask = 16;
            IEnumerable<SpecialThingFilterDef> forceHiddenFilters = this.HiddenSpecialThingFilters();
            ThingFilterUI.DoThingFilterConfigWindow(rect7, ref this.scrollPosition, filter, parentFilter, openMask, null, forceHiddenFilters, false, null, null);
            GUI.EndGroup();
        }

        static void DoAreaRow(Rect rect, StockpilePresetDef preset)
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                GUI.color = Color.white;
            }
            GUI.BeginGroup(rect);
            WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
            float width = rect.width - widgetRow.FinalX - 4f - Text.CalcSize("Rename".Translate()).x - 16f - 4f - Text.CalcSize("InvertArea".Translate()).x - 16f - 4f - 24f;
            widgetRow.Label(preset.label, width);
            if (widgetRow.ButtonText("Rename".Translate(), null, true, false))
            {
                Find.WindowStack.Add(new Dialog_RenameFilter(preset));
            }
            if (widgetRow.ButtonIcon(DeleteX, null, GenUI.SubtleMouseoverColor))
            {
                preset.Delete();
            }
            GUI.EndGroup();
        }
    }

    public class Dialog_RenameFilter : Dialog_Rename
    {
        //
        // Fields
        //
        StockpilePresetDef preset;

        //
        // Constructors
        //
        public Dialog_RenameFilter(StockpilePresetDef preset)
        {
            this.preset = preset;
            this.curName = preset.label;
        }

        //
        // Methods
        //
        protected override AcceptanceReport NameIsValid(string name)
        {
            AcceptanceReport result = base.NameIsValid(name);
            if (!result.Accepted)
            {
                return result;
            }
            if (DefDatabase<StockpilePresetDef>.AllDefs.Any(p => p != preset && p.label == name))
            {
                return "NameIsInUse".Translate();
            }
            return true;
        }

        protected override void SetName(string name)
        {
            preset.label = name;
        }
    }
}
