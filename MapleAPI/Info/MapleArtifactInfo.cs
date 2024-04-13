using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MapleArtifactInfo : MapleInfo
{
    public MapleArtifactInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        ArtifactPanels = new List<MapleArtifact.ArtifactPanel>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.UNION_ARTIFACT;
    }

    public readonly List<MapleArtifact.ArtifactPanel> ArtifactPanels;

    private protected override void ParseInfo()
    {
        if (!TryGet("union_artifact_crystal", out JsonArray? crystals) || crystals == null) return;
        
        foreach (var artifactCrystal in crystals)
        {
            if (artifactCrystal is not JsonObject crystal) continue;

            int level = int.Parse(crystal["level"]!.ToString());
            MapleArtifact.ArtifactPanel panel = new MapleArtifact.ArtifactPanel(level);
            for (int idx = 1; idx <= 3; idx++)
                panel.StatusTypes[idx - 1] =
                    MapleArtifact.GetArtifactType(crystal[$"crystal_option_name_{idx}"]!.ToString());
            ArtifactPanels.Add(panel);
        }
    }
}