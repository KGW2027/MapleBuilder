using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MapleSymbolInfo : MapleInfo
{
    public MapleSymbolInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        SymbolLevels = new Dictionary<MapleSymbol.SymbolType, int>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.SYMBOL;
    }

    public readonly Dictionary<MapleSymbol.SymbolType, int> SymbolLevels;

    private protected override void ParseInfo()
    {
        if (!TryGet("symbol", out JsonArray? symbolData) || symbolData == null) return;
        
        foreach (var value in symbolData)
        {
            if (value is not JsonObject valueObject) continue;
    
            MapleSymbol.SymbolType symbolType = MapleSymbol.GetSymbolType(valueObject["symbol_name"]!.ToString());
            if (symbolType == MapleSymbol.SymbolType.UNKNOWN) continue;
    
            int level = int.Parse(valueObject["symbol_level"]!.ToString());
            SymbolLevels.Add(symbolType, level);
        }
    }
}