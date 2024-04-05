using MapleAPI.DataType;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.Control;

public class BuilderDataContainer
{
    private static CharacterInfo? charInfo;
    public static CharacterInfo? CharacterInfo
    {
        get => charInfo;
        set {
            charInfo = value;
            UpdateCharacterInfo();
        }
    }

    private static void UpdateCharacterInfo()
    {
        if (charInfo == null) return;
        RenderOverview.Update(charInfo);
    }
}