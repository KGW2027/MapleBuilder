using System.Collections;
using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Web;

namespace MapleAPI.Info;

public class MapleSkillInfo : MapleInfo
{
    public MapleSkillInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        skillArgs = new[] {"0", "1", "1.5", "2", "2.5", "3", "4", "hyperpassive", "hyperactive", "5", "6"}
            .GetEnumerator();
        currentKey = "";
        Skills = new Dictionary<string, Dictionary<string, int>>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.SKILL;
    }

    private readonly IEnumerator skillArgs;
    private const string ArgKey = "character_skill_grade";
    private string currentKey;
    public readonly Dictionary<string, Dictionary<string, int>> Skills;

    private protected override bool GetAdditionalArgs(ArgBuilder args)
    {
        if (!skillArgs.MoveNext()) return false;
        currentKey = skillArgs.Current!.ToString()!;
        args.RemoveArg(ArgKey);
        args.Add(ArgKey, currentKey);
        return true;
    }

    private protected override void ParseInfo()
    {
        if (!TryGet("character_skill", out JsonArray? skillList) || skillList == null) return;
        
        Dictionary<string, int> inSkillData = new Dictionary<string, int>();
        foreach (var skillNode in skillList)
        {
            if (skillNode is not JsonObject skillData || skillData["skill_name"] == null) continue;
            string skillName = skillData["skill_name"]!.ToString();
            int skillLevel = int.Parse(skillData["skill_level"]!.ToString());
            inSkillData.TryAdd(skillName, skillLevel);
        }

        Skills.TryAdd(currentKey, inSkillData);
    }
}