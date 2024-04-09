using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType.Item;

public class MapleTitleItem : MapleItemBase
{
    public MapleTitleItem(JsonObject data) : base(data)
    {
        internalData = data;
        IsEmpty = data["title_name"] == null;
        if (IsEmpty) return;
        
        Name = data["title_name"]!.ToString();
        DisplayName = Name;
        MaxUpgrade = 0;
        EquipType = MapleEquipType.EquipType.TITLE;

        string desc = data["title_description"]!.ToString();
        foreach (string lineText in desc.Split("\n"))
        {
            string line = lineText.Trim().Replace("- ", "");
            foreach (string sep in line.Split(","))
            {
                var trim = sep.Trim();
                int plusIndex = trim.IndexOf('+');
                if (plusIndex < 0) continue;
                string value = trim[plusIndex..].Trim();
                string key = trim[..plusIndex];
                foreach (string prefix in key.Split("/"))
                {
                    var pair = MaplePotentialOption.ParseOptionFromPotential($"{prefix.Trim()}{value}");
                    if(pair.Key != MapleStatus.StatusType.OTHER) Status[pair.Key] = pair.Value;
                }
            }
        }
    }
}