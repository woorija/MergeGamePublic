using System.Collections.Generic;

public enum AtlasName
{
    None = 0,
    Common_1 = 1,
    Common_2,
    Rare_1 = 20,
    Rare_2,
    Gift_1 = 50,
    Reward = 100,
}
public static class AltasExtensions
{
    private static readonly Dictionary<AtlasName, string> table = new()
    {
        {AtlasName.Common_1, "Common_1" },
        {AtlasName.Common_2, "Common_2" },
        {AtlasName.Rare_1, "Rare_1" },
        {AtlasName.Rare_2, "Rare_2" },
        {AtlasName.Gift_1, "Gift_1" },
        {AtlasName.Reward, "Reward" },
    };
    public static string ToStr(this AtlasName name) => table[name];
}
