﻿namespace StarAttribute;

public static class EnumExtensions
{
    public static T NextEnum<T>(this Random random)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        return values[random.Next(values.Length)];
    }
}