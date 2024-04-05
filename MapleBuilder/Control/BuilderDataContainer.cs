using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private static HashSet<string> RegisteredItemHashes = new();
    public static ObservableCollection<MapleItem> RegisterItems = new();
    // public static List<MapleItem> RegisterItems = new();

    private static void UpdateCharacterInfo()
    {
        if (charInfo == null) return;
        foreach (MapleItem newItem in charInfo.Items)
        {
            if (!RegisteredItemHashes.Add(newItem.Hash)) continue;
            RegisterItems.Add(newItem);
        }
        
        RenderOverview.Update(charInfo);
    }
}