using StardewValley;

namespace StarAttribute;

public class AttributeData
{
    private List<AttributeStat> _stats = new ();

    public Item Item;
    
    public AttributeData(Item item)
    {
        Item = item;
        for (int i = 0; i < Random.Shared.Next(1, 6); i++)
        {
            _stats.Add(new AttributeStat(
                Random.Shared.NextEnum<AttributeStat.AttributeType>(),
                (float)Random.Shared.NextDouble() * 10f));
        }
        
        Active();
    }

    public void DeActive()
    {
        foreach (var stat in _stats)
        {
            stat.DeActive();
        }
    }

    public void Active()
    {
        foreach (var stat in _stats)
        {
            stat.Active();
        }
    }
    
    public string GetDisplayDesc()
    {
        var text = "";
        
        foreach (var stat in _stats)
        {
            text = (string.IsNullOrEmpty(text) ? "" : text) + 
                   (!string.IsNullOrEmpty(text) ? Environment.NewLine : "") + 
                   stat.GetDisplayDesc();
        }

        return text;
    }
}