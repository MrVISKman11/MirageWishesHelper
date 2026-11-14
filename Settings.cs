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

        [JsonIgnore]
        public Dictionary<string, int> WishWeights { get; set; } = new Dictionary<string, int>
        {
            // ═══ Tier S — Top Priority (Ridan #1, The Feared #2, Fishes #3) ═══
            { "Wish for Rust", 100000 },
            { "Ridan, of the Afarud", 100000 },
            { "Breaking the Astral Chain in the Mirage Area will spawn Ridan, of the Afarud.", 100000 },

            { "Wish for Terror", 95000 },
            { "The Feared", 95000 },
            { "Pinnacle Atlas Boss from The Feared", 95000 },
            { "Breaking the Astral Chain in the Mirage Area will spawn a Pinnacle Atlas Boss from the Feared.", 95000 },

            { "Wish for Fishes", 92000 },
            { "Claim a Fishing Rod", 92000 },
            { "Fishing Rod", 92000 },

            { "Wish for Reflection", 90000 },
            { "Reflecting Mist", 90000 },
            { "A Reflecting Mist will appear on breaking the Astral Chain in the Mirage Area", 90000 },

            { "Wish for Providence", 85000 },
            { "The Nameless Seer", 85000 },
            { "The Nameless Seer will appear on breaking the Astral Chain in the Mirage Area", 85000 },

            // ═══ Tier A — Caches de Alto Valor y Modificadores Fuertes ═══
            { "Wish for Strands", 75000 },
            { "reward a cache of Threads", 75000 },
            { "cache of Threads", 75000 },

            { "Wish for Threads", 74000 },
            { "more Threads found in the Mirage Area", 74000 },

            { "Wish for Foreknowledge", 72000 },
            { "more Divination Cards found in the Mirage Area", 72000 },

            { "Wish for Wealth", 70000 },
            { "more Currency found in the Mirage Area", 70000 },

            { "Wish for Augury", 68000 },
            { "reward a cache of Stacked Decks", 68000 },
            { "cache of Stacked Decks", 68000 },

            { "Wish for Fortune", 66000 },
            { "reward a cache of Currency", 66000 },
            { "cache of Currency", 66000 },

            { "Wish for Titans", 64000 },
            { "additional pack of Atlas Bosses", 64000 },
            { "additional packs of Atlas Bosses", 64000 },

            { "Wish for Risk", 62000 },
            { "packs of difficult and rewarding monsters", 62000 },

            { "Wish for Troves", 60000 },
            { "Unique Strongbox", 60000 },

            { "Wish for Jewels", 58000 },
            { "Jewel Cache", 58000 },

            // ═══ Tier B — Modificadores de Área, Mapas y Packs ═══
            { "Wish for Treasures", 50000 },
            { "increased Rarity of items found in the Mirage Area", 50000 },

            { "Wish for Hordes", 48000 },
            { "increased Pack Size in the Mirage Area", 48000 },

            { "Wish for Horizons", 46000 },
            { "more Maps found in the Mirage Area", 46000 },

            { "Wish for Distant Horizons", 44000 },
            { "reward a cache of Maps", 44000 },

            { "Wish for Strange Horizons", 42000 },
            { "reward a Unique Map", 42000 },

            { "Wish for Eminence", 40000 },
            { "reward an additional Unique Jewel", 40000 },

            { "Wish for Meddling", 38000 },
            { "packs of Astral monsters", 38000 },

            { "Wish for Wisps", 36000 },
            { "Wildwood Wisps", 36000 },

            { "Wish for Betrayal", 34000 },
            { "Syndicate members", 34000 },

            { "Wish for Glyphs", 32000 },
            { "drop as other Currencies", 32000 },

            { "Wish for Uncertainty", 30000 },
            { "random Scarab modifiers", 30000 },

            // ═══ Tier C — Oro y Farm Económico ═══
            { "Wish for Gold", 25000 },
            { "more Gold from slain Enemies", 25000 },

            { "Wish for Avarice", 23000 },
            { "convert dropped Equipment to Gold", 23000 },

            { "Wish for Prosperity", 21000 },
            { "fountain of wealth", 21000 },

            { "Wish for Pursuit", 19000 },
            { "Golden Volatile", 19000 },

            // ═══ Tier D — Poder y Buffs Temporales para el Personaje ═══
            { "Wish for Godhood", 15000 },
            { "Echoing Shrine and Divine Shrine", 15000 },

            { "Wish for Souls", 14000 },
            { "Soul Eater", 14000 },

            { "Wish for Power", 13000 },
            { "explode on death", 13000 },

            { "Wish for Momentum", 12000 },
            { "Onslaught and Adrenaline", 12000 },

            { "Wish for Elements", 11000 },
            { "Blessing of the Storm", 11000 },

            { "Wish for Knowledge", 10000 },
            { "increased Experience", 10000 },

            { "Wish for Glittering", 9000 },
            { "random amount of Quality", 9000 },

            { "Wish for Oases", 8000 },
            { "Oasis Ground", 8000 },

            { "Wish for Hindrance", 7000 },
            { "Chilled and Hindered", 7000 },

            { "Wish for Foes", 6000 },
            { "Rare Monsters in the Mirage Area will have two additional modifiers", 6000 },

            { "Wish for Rebirth", 5000 },
            { "revive when slain", 5000 },

            { "Wish for Phantoms", 4000 },
            { "cannot drop Equipment", 4000 },

            // ═══ Tier F — Recompensas Menores de Equipo/Currency ═══
            { "Wish for Binding", 2000 },
            { "Orbs of Binding", 2000 },

            { "Wish for Regency", 1900 },
            { "Regal Orbs", 1900 },

            { "Wish for Connections", 1800 },
            { "Fusing or Jewellers Orbs", 1800 },

            { "Wish for Mosaics", 1700 },
            { "Chromatic Orbs", 1700 },

            { "Wish for Ancient Protection", 1500 },
            { "Unique Armour", 1500 },

            { "Wish for Ancient Armaments", 1400 },
            { "Unique Weapon", 1400 },

            { "Wish for Ancient Curios", 1300 },
            { "Unique Jewellery", 1300 },

            { "Wish for Craftsmanship", 1200 },
            { "Five-Linked Body Armour", 1200 },

            { "Wish for Trinkets", 1000 },
            { "Jewellery found in Mirage Area will instead drop as Jewels", 1000 },
            { "reward Rare Jewellery", 1000 },

            { "Wish for Swiftness", 800 },
            { "reward Rare Boots", 800 },

            { "Wish for Helms", 800 },
            { "reward Rare Helms", 800 },
            { "reward Rare Helmets", 800 },

            { "Wish for Mitts", 800 },
            { "reward Rare Gloves", 800 },

            { "Wish for Protection", 800 },
            { "reward Rare Body Armours", 800 },

            { "Wish for Blades", 800 },
            { "reward Rare Melee Weapons", 800 },

            { "Wish for Missiles", 800 },
            { "reward Rare Ranged Weapons", 800 },

            { "Wish for Bastions", 800 },
            { "reward Rare Shields", 800 },

            // ═══ Memes ═══
            { "Wish for Croaks", 100 },
            { "contains additional frogs", 100 }
        };
    }
}
