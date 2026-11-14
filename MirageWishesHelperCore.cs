using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Elements;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MirageWishesHelper
{
    public class MirageWishesHelperCore : BaseSettingsPlugin<Settings>
    {
        private int _cachedPanelIndex = -1;

        public override bool Initialise()
        {
            Name = "MirageWishesHelper";
            return true;
        }

        public override void Render()
        {
            if (!Settings.Enable) return;
            
            var ingameUi = GameController.IngameState?.IngameUi;
            if (ingameUi == null) return;
            
            var wishesPanel = FindMirageWishesPanel(ingameUi);
            if (wishesPanel == null || !wishesPanel.IsVisible) return;

            // Try to extract individual options if panel follows the standard container structure
            var options = GetWishOptions(wishesPanel);
            if (options.Count > 0)
            {
                Element bestOption = null;
                int highestWeight = Settings.MinWeightToHighlight.Value - 1;

                foreach (var opt in options)
                {
                    if (opt == null || !opt.IsVisible) continue;

                    var texts = GetAllTextElements(opt)
                        .Select(t => t.Text ?? t.TextNoTags)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();

                    int optBestWeight = -1;

                    foreach (var text in texts)
                    {
                        foreach (var kvp in Settings.WishWeights)
                        {
                            if (text.Contains(kvp.Key, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (kvp.Value > optBestWeight)
                                {
                                    optBestWeight = kvp.Value;
                                }
                            }
                        }
                    }

                    if (optBestWeight > highestWeight)
                    {
                        highestWeight = optBestWeight;
                        bestOption = opt;
                    }
                }

                if (bestOption != null)
                {
                    Graphics.DrawFrame(bestOption.GetClientRectCache, Settings.HighlightColor.Value, Settings.FrameThickness.Value);
                    return;
                }
            }

            // Fallback: search text elements strictly within the panel (never root ingameUi)
            var textElements = GetAllTextElements(wishesPanel).ToList();
            Element bestChoice = null;
            int maxWeight = Settings.MinWeightToHighlight.Value - 1;

            foreach (var elem in textElements)
            {
                var text = elem.Text ?? elem.TextNoTags;
                if (string.IsNullOrWhiteSpace(text)) continue;

                foreach (var kvp in Settings.WishWeights)
                {
                    if (text.Contains(kvp.Key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (kvp.Value > maxWeight)
                        {
                            maxWeight = kvp.Value;
                            bestChoice = elem.Parent ?? elem;
                        }
                    }
                }
            }

            if (bestChoice != null)
            {
                Graphics.DrawFrame(bestChoice.GetClientRectCache, Settings.HighlightColor.Value, Settings.FrameThickness.Value);
            }
        }

        private Element FindMirageWishesPanel(IngameUIElements ui)
        {
            // Note: In ExileCore.dll, IngameUIElements.MirageWishesPanel is stubbed to return 'ui' (this).
            // Only accept reflection if it resolves to a distinct, non-root child element.
            try
            {
                var prop = ui.GetType().GetProperty("MirageWishesPanel");
                if (prop != null)
                {
                    var val = prop.GetValue(ui) as Element;
                    if (val != null && val.Address != ui.Address && val.Address != 0 && val.IsVisible)
                    {
                        return val;
                    }
                }
            }
            catch
            {
                // Fall through to structure discovery
            }

            // Check cached child index first for high performance
            if (_cachedPanelIndex >= 0 && _cachedPanelIndex < ui.ChildCount)
            {
                var cached = ui.GetChildAtIndex(_cachedPanelIndex);
                if (cached != null && cached.IsVisible && IsWishesStructure(cached))
                {
                    return cached;
                }
            }

            // Scan IngameUi children dynamically for the wishes panel
            var childCount = (int)ui.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = ui.GetChildAtIndex(i);
                if (child == null || !child.IsVisible) continue;
                if (IsWishesStructure(child))
                {
                    _cachedPanelIndex = i;
                    return child;
                }
            }

            return null;
        }

        private static bool IsWishesStructure(Element panel)
        {
            if (panel == null || !panel.IsVisible || panel.ChildCount < 5) return false;
            var container = panel.GetChildAtIndex(4);
            if (container == null || container.ChildCount < 7) return false;

            // Verify wish options exist at expected indices (3, 4, 5)
            for (int i = 3; i <= 5; i++)
            {
                var option = container.GetChildAtIndex(i);
                if (option == null || option.ChildCount < 3) return false;
            }

            return true;
        }

        private static List<Element> GetWishOptions(Element panel)
        {
            var result = new List<Element>();
            if (panel == null) return result;

            // Standard layout: container is child 4, options are 3, 4, 5
            if (panel.ChildCount >= 5)
            {
                var container = panel.GetChildAtIndex(4);
                if (container != null && container.ChildCount >= 7)
                {
                    for (int i = 3; i <= 5; i++)
                    {
                        var opt = container.GetChildAtIndex(i);
                        if (opt != null && opt.IsVisible)
                        {
                            result.Add(opt);
                        }
                    }
                }
            }
            return result;
        }

        private IEnumerable<Element> GetAllTextElements(Element parent)
        {
            if (parent == null || !parent.IsVisible) yield break;
            
            if (!string.IsNullOrEmpty(parent.Text) || !string.IsNullOrEmpty(parent.TextNoTags))
                yield return parent;

            if (parent.Children == null) yield break;

            foreach (var child in parent.Children)
            {
                foreach (var textElem in GetAllTextElements(child))
                    yield return textElem;
            }
        }
    }
}
