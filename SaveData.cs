using StardewValley;

namespace StarAttribute;

public sealed class SaveData
{
    public Dictionary<Item, AttributeData> Items { get; set; } = new();
}