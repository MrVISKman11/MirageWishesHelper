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

            // Strategy 1: Extract individual option cards (arch cards)
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

            // Strategy 2 (Fallback): Match any text element strictly inside the wishes panel
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
                if (cached != null && cached.IsVisible && IsWishesPanel(cached))
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
                if (IsWishesPanel(child))
                {
                    _cachedPanelIndex = i;
                    return child;
                }
            }

            return null;
        }

        private static bool IsWishesPanel(Element elem)
        {
            if (elem == null || !elem.IsVisible || elem.ChildCount < 2) return false;

            // Check A: Standard AutoExile structure (child 4 has 7+ children)
            if (elem.ChildCount >= 5)
            {
                var container = elem.GetChildAtIndex(4);
                if (container != null && container.ChildCount >= 7)
                {
                    return true;
                }
            }

            // Check B: Header or child text contains "Wishes" or "Wish for "
            if (CheckDirectOrChildText(elem, "Wishes") || 
                CheckDirectOrChildText(elem, "Choose what you wish") ||
                CheckDirectOrChildText(elem, "Wish for "))
            {
                return true;
            }

            return false;
        }

        private static bool CheckDirectOrChildText(Element elem, string query)
        {
            if (elem == null || !elem.IsVisible) return false;

            var direct = elem.Text ?? elem.TextNoTags;
            if (!string.IsNullOrEmpty(direct) && direct.Contains(query, StringComparison.OrdinalIgnoreCase))
                return true;

            if (elem.Children != null)
            {
                foreach (var child in elem.Children)
                {
                    if (child == null || !child.IsVisible) continue;
                    var t = child.Text ?? child.TextNoTags;
                    if (!string.IsNullOrEmpty(t) && t.Contains(query, StringComparison.OrdinalIgnoreCase))
                        return true;

                    // Check second level children
                    if (child.Children != null)
                    {
                        foreach (var sub in child.Children)
                        {
                            if (sub == null || !sub.IsVisible) continue;
                            var st = sub.Text ?? sub.TextNoTags;
                            if (!string.IsNullOrEmpty(st) && st.Contains(query, StringComparison.OrdinalIgnoreCase))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static List<Element> GetWishOptions(Element panel)
        {
            var result = new List<Element>();
            if (panel == null) return result;

            // Strategy A: Standard layout (container is child 4, options are 3, 4, 5)
            if (panel.ChildCount >= 5)
            {
                var container = panel.GetChildAtIndex(4);
                if (container != null && container.ChildCount >= 7)
                {
                    for (int i = 3; i <= 5; i++)
                    {
                        var opt = container.GetChildAtIndex(i);
                        if (opt != null && opt.IsVisible && opt.GetClientRectCache.Width > 40)
                        {
                            result.Add(opt);
                        }
                    }
                    if (result.Count == 3) return result;
                }
            }

            // Strategy B (Fallback): Find elements whose text starts with "Wish for "
            // Climb up to the card container (width > 100, height > 150)
            result.Clear();
            var allTexts = GetAllTextElementsStatic(panel);
            foreach (var textElem in allTexts)
            {
                var t = (textElem.Text ?? textElem.TextNoTags ?? "").Trim();
                if (t.StartsWith("Wish for ", StringComparison.OrdinalIgnoreCase))
                {
                    Element card = textElem;
                    Element curr = textElem.Parent;
                    while (curr != null && curr != panel)
                    {
                        var rect = curr.GetClientRectCache;
                        if (rect.Width > 100 && rect.Height > 150)
                        {
                            card = curr;
                            break;
                        }
                        card = curr;
                        curr = curr.Parent;
                    }

                    if (!result.Contains(card))
                    {
                        result.Add(card);
                    }
                }
            }

            return result;
        }

        private IEnumerable<Element> GetAllTextElements(Element parent)
        {
            return GetAllTextElementsStatic(parent);
        }

        private static List<Element> GetAllTextElementsStatic(Element parent)
        {
            var list = new List<Element>();
            CollectTexts(parent, list);
            return list;
        }

        private static void CollectTexts(Element parent, List<Element> list)
        {
            if (parent == null || !parent.IsVisible) return;

            if (!string.IsNullOrEmpty(parent.Text) || !string.IsNullOrEmpty(parent.TextNoTags))
                list.Add(parent);

            if (parent.Children == null) return;

            foreach (var child in parent.Children)
            {
                CollectTexts(child, list);
            }
        }
    }
}
