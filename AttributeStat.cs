using StardewValley;

namespace StarAttribute;

public class AttributeStat
{
    public enum AttributeType
    {
        Health,
        Stamina,
    }
    
    public AttributeType Type { private set; get; }
    public float Value { private set; get; }

    public AttributeStat(AttributeStat.AttributeType type, float value)
    {
        Type = type;
        Value = value;
    }

    public void Active()
    {
        switch (Type)
        {
            case AttributeType.Health:
                Game1.player.maxHealth += (int)Value;
                break;
            case AttributeType.Stamina:
                Game1.player.Stamina += Value;
                break;
        }
    }

    public void DeActive()
    {
        switch (Type)
        {
            case AttributeType.Health:
                Game1.player.maxHealth -= (int)Value;
                break;
            case AttributeType.Stamina:
                Game1.player.Stamina -= Value;
                break;
        }
    }

    public string GetDisplayDesc()
    {
        switch (Type)
        {
            case AttributeType.Health:
                return $"+ Health: {GetValueString()}";
            case AttributeType.Stamina:
                return $"+ Stamina: {GetValueString()}";
        }

        return "";
    }

    private string GetValueString() => Value.ToString("F2");
}