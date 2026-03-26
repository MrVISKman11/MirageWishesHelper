using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MirageWishesHelper
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        
        public ColorNode HighlightColor { get; set; } = new ColorNode(SharpDX.Color.LightGreen);
        public RangeNode<int> FrameThickness { get; set; } = new RangeNode<int>(3, 1, 10);
        public RangeNode<int> MinWeightToHighlight { get; set; } = new RangeNode<int>(1, 0, 1000000);

        // Edit this dictionary to add the actual text of the wishes and their corresponding scores
        [JsonIgnore]
        public Dictionary<string, int> WishWeights { get; set; } = new Dictionary<string, int>
        {
            { "Breaking the Astral Chain in the Mirage Area will spawn Ridan, of the Afarud.", 80000 },
            { "Coin of Knowledge", 70000 },
            { "Coin of Power", 70000 },
            { "Coin of Skill", 70000 },
            { "Mirage Area will contain an additional fountain of wealth.", 50000 },
            { "The Nameless Seer will appear on breaking the Astral Chain in the Mirage Area", 40000 },
            { "A Reflecting Mist will appear on breaking the Astral Chain in the Mirage Area", 30000 },
            { "Breaking the Astral Chain in the Mirage Area will spawn a Pinnacle Atlas Boss from the Feared.", 20000 }
        };
    }
}
