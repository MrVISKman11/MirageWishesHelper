using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Elements;
using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace MirageWishesHelper
{
    public class MirageWishesHelperCore : BaseSettingsPlugin<Settings>
    {
        public override bool Initialise()
        {
            Name = "MirageWishesHelper";
            return true;
        }

        public override void Render()
        {
            if (!Settings.Enable) return;
            
            var ingameUi = GameController.IngameState.IngameUi;
            if (ingameUi == null) return;
            
            var miragePanel = FindMirageWishesPanel(ingameUi);
            if (miragePanel == null || !miragePanel.IsVisible) return;

            var textElements = GetAllTextElements(miragePanel).ToList();
            
            Element bestChoice = null;
            int highestWeight = Settings.MinWeightToHighlight.Value - 1;

            foreach (var elem in textElements)
            {
                var text = elem.Text ?? elem.TextNoTags;
                if (string.IsNullOrWhiteSpace(text)) continue;

                foreach (var kvp in Settings.WishWeights)
                {
                    if (text.Contains(kvp.Key, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (kvp.Value > highestWeight)
                        {
                            highestWeight = kvp.Value;
                            
                            // Highlight the parent box of the text for a clearer UI box
                            bestChoice = elem.Parent ?? elem; 
                        }
                    }
                }
            }

            if (bestChoice != null)
            {
                // Draw a nice green frame around the best option
                Graphics.DrawFrame(bestChoice.GetClientRectCache, Settings.HighlightColor.Value, Settings.FrameThickness.Value);
            }
        }

        private Element FindMirageWishesPanel(IngameUIElements ui)
        {
            // Attempt 1: Strongly typed property
            var prop = ui.GetType().GetProperty("MirageWishesPanel");
            if (prop != null)
            {
                var val = prop.GetValue(ui) as Element;
                if (val != null) return val;
            }

            // Attempt 2: Search children by property or string name
            // Often, if the property isn't explicitly defined in IngameUIElements,
            // we can grab it if it's the last few visible panels, or we could just 
            // search all visible root children for ones containing text options.
            // But since the user knows the name, the Reflection above will hit if ExileCore maps it.
            // Otherwise, we gracefully return null.
            
            return null;
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
